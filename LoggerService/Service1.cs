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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;

namespace LoggerService
{
    public partial class ReefAngelLogger : ServiceBase
    {
        ServiceHost serviceHost = null;

        public ReefAngelLogger()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //if (serviceHost != null)
            //    serviceHost.Close();
            //serviceHost = new ServiceHost(typeof(RALogger));
            //serviceHost.Open();
            RALogger logger = new RALogger();
        }

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
