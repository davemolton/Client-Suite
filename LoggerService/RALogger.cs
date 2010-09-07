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
using System.Xml.Linq;
using System.Timers;
using System.Data.SqlServerCe;
using System.Configuration;

namespace LoggerService
{
  
    /// <summary>
    /// This is an optional Subscriber to the Listener 
    /// </summary>
    public class RALogger
    {
     
        private Guid clientId;
        private InstanceContext context;
        ClientCallback callback;
        ReefAngelService.ReefAngelListenerClient raService;
        SqlCeConnection conn;
        SqlCeCommand cmd;
        Timer t;
        Timer ReconnectTimer;
        bool timerUp = true;
        bool connected = false;

        public RALogger()
        {

            string dbLoc = ConfigurationManager.AppSettings.Get("DBLocation").ToString();
            conn = new SqlCeConnection("Data Source=" + dbLoc);
            cmd = new SqlCeCommand();

            //Timer Info
            t = new Timer();
            t.Interval = int.Parse(ConfigurationManager.AppSettings.Get("TimerInterval").ToString()) * 1000;           
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.Start();

            //wire up received notifcations and store params...
            callback = new ClientCallback();
            callback.ClientNotifiedNewParams += ParamsReceived;
            callback.ClientNotifiedNewRelayData += RelayDataReceived;


            context = new InstanceContext(callback);
            raService = new ReefAngelService.ReefAngelListenerClient(context);

            //This timer used to automatically try to reconnect to endpoint every 5mins should subsribe throw an error;
            ReconnectTimer = new Timer();
            ReconnectTimer.Interval = 300000;
            ReconnectTimer.Elapsed += new ElapsedEventHandler(ReconnectTimer_Elapsed);

            try
            {
                clientId = raService.Subscribe(); //This client now a subscriber to Listener Service and will receive updates 
                connected = true;
            }
            catch (Exception ex)
            {
                connected = false;
                ReconnectTimer.Start();
            }
        }

        void ReconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!connected)
            {
                try
                {
                    clientId = raService.Subscribe();
                    connected = true;
                    ReconnectTimer.Stop();
                }
                catch (Exception ex)
                {
                    ReconnectTimer.Stop();
                    ReconnectTimer.Start();
                }
            }
            else
                connected = false;
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {            
            timerUp = true;
        }

        private void ParamsReceived(object sender, ClientNotifiedNewParamsEventArgs e)
        {
           
            if (timerUp)
            {
                timerUp = false;
                StoreParams(e.RAParams);
            }
        }

        private void RelayDataReceived(object sender, ClientNotifiedNewRelayDataEventArgs e)
        { 
            //TODO: Implement relay data storing..
        }

        private void StoreParams(ReefAngelService.ReefAngelParams paras)
        {
           
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO RAParams(Temp1, Temp2, Temp3, Ph, Timestamp) VALUES(@T1, @T2, @T3, @Ph, @tstamp)";
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@T1", paras.Temp1);
            cmd.Parameters.Add("@T2", paras.Temp2);
            cmd.Parameters.Add("@T3", paras.Temp3);
            cmd.Parameters.Add("@Ph", paras.PH);
            cmd.Parameters.Add("@tstamp", paras.ParamTime);
           
            conn.Open();        
            cmd.ExecuteNonQuery();           
            conn.Close();           
        }
    }
}
