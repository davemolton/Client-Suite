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
using System.Text;


namespace LoggerService
{
    public class ClientCallback: ReefAngelService.IReefAngelListenerCallback
    {
        public delegate void ClientNotifiedNewParamsEventHandler(object sender, ClientNotifiedNewParamsEventArgs e);
        public event ClientNotifiedNewParamsEventHandler ClientNotifiedNewParams;
        public delegate void ClientNotifiedNewRelayDataEventHandler(object sender, ClientNotifiedNewRelayDataEventArgs e);
        public event ClientNotifiedNewRelayDataEventHandler ClientNotifiedNewRelayData;


        public void HandleParams(ReefAngelService.ReefAngelParams paras)
        {
            if (ClientNotifiedNewParams != null)
            {
                ClientNotifiedNewParams(this, new ClientNotifiedNewParamsEventArgs(paras));   
            }
        }

        public void HandleRelayData(ReefAngelService.ReefAngelRelayData relayData)
        {
            if (ClientNotifiedNewRelayData != null)
            {
                ClientNotifiedNewRelayData(this, new ClientNotifiedNewRelayDataEventArgs(relayData));
            }
        }
    }

    public class ClientNotifiedNewParamsEventArgs : EventArgs
    {
        private readonly ReefAngelService.ReefAngelParams paras;
        public ClientNotifiedNewParamsEventArgs(ReefAngelService.ReefAngelParams paras)
        {
            this.paras = paras;
        }

        public ReefAngelService.ReefAngelParams RAParams { get { return paras; } }
    }

    public class ClientNotifiedNewRelayDataEventArgs : EventArgs
    {
        private readonly ReefAngelService.ReefAngelRelayData relayData;
        public ClientNotifiedNewRelayDataEventArgs(ReefAngelService.ReefAngelRelayData relayData)
        {
            this.relayData = relayData;
        }

        public ReefAngelService.ReefAngelRelayData RARelayData { get { return relayData; } }
    }
}
