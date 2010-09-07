/*
 *   Copyright (C) 2010 David Molton
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.IO;
using System.Xml.Linq;
using System.Configuration;

namespace ReefAngelWCFListener
{
   
    /// <summary>
    /// This class will run forever as Windows Service.  Only purpose is to listen in on serial port and attempt to format received xml data from ReefAngel.  It maintains
    /// its own list of subscribed clients and raises their received parameter events.  Notifying listeners and passing them received parameters allows 
    /// individual clients to handle the received data in their own ways.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ReefAngelListener : IReefAngelListener
    {
        //Individual clients are identified by guid
        private readonly Dictionary<Guid, IReefAngelCallback> clients = new Dictionary<Guid, IReefAngelCallback>();
        string input;
        SerialPort s1 = new SerialPort();
        ReefAngelParams currentParams;
        ReefAngelRelayData currentRelayData;
        TextReader rdr;

        public ReefAngelListener()
        {
            //Open serial port and handle DataReceived event to format data and notify subscribed clients.

            s1.BaudRate = int.Parse(ConfigurationManager.AppSettings.Get("BaudRate").ToString());
            s1.PortName = ConfigurationManager.AppSettings.Get("ComPort").ToString();
            s1.DataReceived += new SerialDataReceivedEventHandler(s1_DataReceived);
            s1.Open();
        }

        void s1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(250);
            input = s1.ReadExisting();
            if (GenerateCurrentParams(input) != null)
            {
                SendParams(currentParams);
                SendRelayData(currentRelayData);
            }            
        }

        /// <summary>
        /// Parse the XML data sent from ReefAngel Controller and build a ReefAngelParams object that will be sent to individual subscribers.
        /// </summary>
        /// <param name="xml">The Received XML Data</param>
        /// <returns>The Current Parameter values from the controller as a ReefAngelParam object.</returns>
        private ReefAngelParams GenerateCurrentParams(string xml)
        {
            try
            {              
                rdr = new StringReader(xml);
                XDocument xdoc = XDocument.Load(rdr, LoadOptions.None);
                XElement root = xdoc.Element("RA");

                currentParams = new ReefAngelParams();
                currentParams.ParamTime = DateTime.Now;  //TODO: For simplicity I use the systems date/time.  However in the future this value should probably come from controller for consistency.
                currentParams.Temp1 = FormatTemp(root.Element("T1").Value.ToString());
                currentParams.Temp2 = FormatTemp(root.Element("T2").Value.ToString());
                currentParams.Temp3 = FormatTemp(root.Element("T3").Value.ToString());
                currentParams.PH = FormatPH(root.Element("PH").Value.ToString());

                BuildRelayStatuses(ToBinary(int.Parse(root.Element("R").Value.ToString())), 
                                   ToBinary(int.Parse(root.Element("RON").Value.ToString())), 
                                   ToBinary(int.Parse(root.Element("ROFF").Value.ToString())));
                return currentParams;
            }
            catch (Exception ex) 
            {
                // For some reason, controller will sometimes send partial data and incomplete xml causing xmlparse to fail.  Ignore it for now and send null object.
                //TODO: Hanlde the correct XMLParse Exception instead of generic Exception.  Log exceptions.
                currentParams = null;
                return currentParams;
            }
        }

        /// <summary>
        /// Build RelayData objects to send to clients.  Show the on/off status of indivudal relays.  Also the on/off masks associated with them.
        /// </summary>
        /// <param name="binaryOnOffStatus">The string representation of the on/off status of all 8 relays in reverse order.  (ex. 11010011)</param>
        /// <param name="binaryMaskOnStatuses">The string representation of the Mask On status of all 8 relays in reverse order.  (ex. 11010011)</param>
        /// <param name="binaryMaskOffStatuses">The string representation of the Mask Off status of all 8 relays in reverse order.  (ex. 11010011)</param>
        private void BuildRelayStatuses(string binaryOnOffStatus,string binaryMaskOnStatuses, string binaryMaskOffStatuses)
        {
            currentRelayData = new ReefAngelRelayData();
            string orderedBinaryRelayData, orderedBinaryMaskOnData, orderedBinaryMaskOffData;
            char[] arr = binaryOnOffStatus.ToCharArray();
            Array.Reverse(arr);
            orderedBinaryRelayData = new string(arr);

            currentRelayData.Relay1On = orderedBinaryRelayData[0] == '0' ? false : true;
            currentRelayData.Relay2On = orderedBinaryRelayData[1] == '0' ? false : true;
            currentRelayData.Relay3On = orderedBinaryRelayData[2] == '0' ? false : true;
            currentRelayData.Relay4On = orderedBinaryRelayData[3] == '0' ? false : true;
            currentRelayData.Relay5On = orderedBinaryRelayData[4] == '0' ? false : true;
            currentRelayData.Relay6On = orderedBinaryRelayData[5] == '0' ? false : true;
            currentRelayData.Relay7On = orderedBinaryRelayData[6] == '0' ? false : true;
            currentRelayData.Relay8On = orderedBinaryRelayData[7] == '0' ? false : true;

            //TODO: Incomplete binary data for the On/Off masks is coming back from ReefAngel Controller.  Need to identify how the data
            //is sent from controller - parse it and populte the following code belowing.  

            //arr = binaryMaskOnStatuses.ToCharArray();
            //Array.Reverse(arr);
            //orderedBinaryMaskOnData = new string(arr);
            //currentRelayData.Relay1MaskOn = orderedBinaryMaskOnData[0] == '0' ? false : true;
            //currentRelayData.Relay2MaskOn = orderedBinaryMaskOnData[1] == '0' ? false : true;
            //currentRelayData.Relay3MaskOn = orderedBinaryMaskOnData[2] == '0' ? false : true;
            //currentRelayData.Relay4MaskOn = orderedBinaryMaskOnData[3] == '0' ? false : true;
            //currentRelayData.Relay5MaskOn = orderedBinaryMaskOnData[4] == '0' ? false : true;
            //currentRelayData.Relay6MaskOn = orderedBinaryMaskOnData[5] == '0' ? false : true;
            //currentRelayData.Relay7MaskOn = orderedBinaryMaskOnData[6] == '0' ? false : true;
            //currentRelayData.Relay8MaskOn = orderedBinaryMaskOnData[7] == '0' ? false : true;

            //arr = binaryMaskOffStatuses.ToCharArray();
            //Array.Reverse(arr);
            //orderedBinaryMaskOffData = new string(arr);
            //currentRelayData.Relay1MaskOff = orderedBinaryMaskOffData[0] == '0' ? false : true;
            //currentRelayData.Relay2MaskOff = orderedBinaryMaskOffData[1] == '0' ? false : true;
            //currentRelayData.Relay3MaskOff = orderedBinaryMaskOffData[2] == '0' ? false : true;
            //currentRelayData.Relay4MaskOff = orderedBinaryMaskOffData[3] == '0' ? false : true;
            //currentRelayData.Relay5MaskOff = orderedBinaryMaskOffData[4] == '0' ? false : true;
            //currentRelayData.Relay6MaskOff = orderedBinaryMaskOffData[5] == '0' ? false : true;
            //currentRelayData.Relay7MaskOff = orderedBinaryMaskOffData[6] == '0' ? false : true;
            //currentRelayData.Relay8MaskOff = orderedBinaryMaskOffData[7] == '0' ? false : true;

        }

        #region Private Helper Functions
        private string FormatPH(string ph)
        {
            return ph.Substring(0, ph.Length - 2) + "." + ph.Substring(ph.Length - 2, 2);
        }
        private string FormatTemp(string temp)
        {
            return temp.Substring(0, temp.Length - 1) + "." + temp.Substring(temp.Length - 1, 1);
        }
        private string ToBinary(Int64 Decimal)
        {

            // Declare a few variables we're going to need
            Int64 BinaryHolder;
            char[] BinaryArray;

            string BinaryResult = "";
            while (Decimal > 0)
            {
                BinaryHolder = Decimal % 2;
                BinaryResult += BinaryHolder;
                Decimal = Decimal / 2;
            }

            // The algoritm gives us the binary number in reverse order (mirrored)
            // We store it in an array so that we can reverse it back to normal

            BinaryArray = BinaryResult.ToCharArray();
            Array.Reverse(BinaryArray);
            BinaryResult = new string(BinaryArray);
            return BinaryResult;

        }
        #endregion

        #region Contract Functions

        //The following methods are the necessary implementation of the IReefAngelListener Interface.  This is the functionality that is exposed to clients 

        /// <summary>
        /// Adds a client the notifiy list to receive updates about params when they are received from the controller.
        /// </summary>
        /// <returns>A Unique ID that will later be used to identify individual clients and necessary to unsubscribe.</returns>
        public Guid Subscribe()
        {
            IReefAngelCallback callback = OperationContext.Current.GetCallbackChannel<IReefAngelCallback>();
            Guid clientId = Guid.NewGuid();
            if (callback != null)
            {
                lock (clients)
                    clients.Add(clientId, callback);
            }
            return clientId;
        }

        /// <summary>
        /// Removes a client from the notify list.
        /// </summary>
        /// <param name="clientId">The unique ID that assigned to the client via Subscribe()</param>
        public void Unsubscribe(Guid clientId)
        {
            lock (clients)
            { 
                if(clients.ContainsKey(clientId))
                    clients.Remove(clientId);
            }
        }

        /// <summary>
        /// Every once inawhile a call can be made here to ensure the publisher/subscriber timeout has not been reached
        /// </summary>
        public void KeepConnection()
        {
            //Empty method - no functionality needs to exists here.  Just keeps the connection alive.
        }

        /// <summary>
        /// Iterate through the current list of subscribed clients and invoke their HandleParameters event.
        /// </summary>
        /// <param name="paras">The ReefANgel parameters to send to individual clients</param>
        public void SendParams(ReefAngelParams paras)
        {
           
            ThreadPool.QueueUserWorkItem
            (
                delegate
                {
                    lock (clients)
                    {
                        List<Guid> disconnectedClientGuids = new List<Guid>();

                        foreach (KeyValuePair<Guid, IReefAngelCallback> client in clients)
                        {
                            try
                            {
                                client.Value.HandleParams(paras);
                            }
                            catch (Exception)
                            {
                                
                                // There is a number of reasons an exception can occur here.
                                // The server might not be able to connect to client b/c of network error,
                                // early client terminiation w/out being able to unsubscribe first or comms object is killed.
                                // No matter the case - mark the client key for removal.
                                disconnectedClientGuids.Add(client.Key);
                            }
                        }

                        foreach (Guid clientGuid in disconnectedClientGuids)
                        {
                            clients.Remove(clientGuid);
                        }
                    }
                }
            );
        }
        /// <summary>
        /// Iterate through current client list and invoke their HandleRelayData with latest relay parameters.
        /// </summary>
        /// <param name="relayData">The current relay data params</param>
        public void SendRelayData(ReefAngelRelayData relayData)
        {           
            ThreadPool.QueueUserWorkItem
            (
                delegate
                {
                    lock (clients)
                    {
                        List<Guid> disconnectedClientGuids = new List<Guid>();

                        foreach (KeyValuePair<Guid, IReefAngelCallback> client in clients)
                        {
                            try
                            {
                                client.Value.HandleRelayData(relayData);
                            }
                            catch (Exception)
                            {
                                // There is a number of reasons an exception can occur here.
                                // The server might not be able to connect to client b/c of network error,
                                // early client terminiation w/out being able to unsubscribe first or comms object is killed.
                                // No matter the case - mark the client key for removal.
                                disconnectedClientGuids.Add(client.Key);
                            }
                        }

                        foreach (Guid clientGuid in disconnectedClientGuids)
                        {
                            clients.Remove(clientGuid);
                        }
                    }
                }
            );
        }
#endregion
    }
}
