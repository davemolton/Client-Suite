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
using System.Runtime.Serialization;

namespace ReefAngelWCFListener
{
    //This is the RelayData object that will be serialized and handled in client's HandleRelayData()
    [DataContract]
    public class ReefAngelRelayData
    {
        [DataMember]
        public bool Relay1On { get; set; }
        [DataMember]
        public bool Relay2On { get; set; }
        [DataMember]
        public bool Relay3On { get; set; }
        [DataMember]
        public bool Relay4On { get; set; }
        [DataMember]
        public bool Relay5On { get; set; }
        [DataMember]
        public bool Relay6On { get; set; }
        [DataMember]
        public bool Relay7On { get; set; }
        [DataMember]
        public bool Relay8On { get; set; }

        [DataMember]
        public bool Relay1MaskOn { get; set; }
        [DataMember]
        public bool Relay2MaskOn { get; set; }
        [DataMember]
        public bool Relay3MaskOn { get; set; }
        [DataMember]
        public bool Relay4MaskOn { get; set; }
        [DataMember]
        public bool Relay5MaskOn { get; set; }
        [DataMember]
        public bool Relay6MaskOn { get; set; }
        [DataMember]
        public bool Relay7MaskOn { get; set; }
        [DataMember]
        public bool Relay8MaskOn { get; set; }

        [DataMember]
        public bool Relay1MaskOff { get; set; }
        [DataMember]
        public bool Relay2MaskOff { get; set; }
        [DataMember]
        public bool Relay3MaskOff { get; set; }
        [DataMember]
        public bool Relay4MaskOff { get; set; }
        [DataMember]
        public bool Relay5MaskOff { get; set; }
        [DataMember]
        public bool Relay6MaskOff { get; set; }
        [DataMember]
        public bool Relay7MaskOff { get; set; }
        [DataMember]
        public bool Relay8MaskOff { get; set; }
    }
}
