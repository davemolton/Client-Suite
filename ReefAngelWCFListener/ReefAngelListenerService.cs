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
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceProcess;

namespace ReefAngelWCFListener
{
    
    class ReefAngelListenerService : ServiceBase
    {
        public ServiceHost serviceHost = null;

        public ReefAngelListenerService()
        {
            ServiceName = "ReefAngelListener";

        }

        //Entry point for the Listener Service 
        public static void Main()
        {
            ServiceBase.Run(new ReefAngelListenerService());          
        }

        /// <summary>
        /// Open Service Listener
        /// </summary>
        /// <param name="args">Command Args</param>
        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
                serviceHost.Close();
            serviceHost = new ServiceHost(typeof(ReefAngelListener));
            serviceHost.Open();
        }

        /// <summary>
        /// Kill the host
        /// </summary>
        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }
}
