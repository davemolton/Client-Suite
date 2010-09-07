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
    //This parameter object that will be serialized and sent to indivudal subsribed clients.
    [DataContract]
    public class ReefAngelParams
    {
        [DataMember]
        public DateTime ParamTime { get; set; }
        [DataMember]
        public string Temp1 { get; set; }
        [DataMember]
        public string Temp2 { get; set; }
        [DataMember]
        public string Temp3 { get; set; }
        [DataMember]
        public string PH { get; set; }
      
    }
}
