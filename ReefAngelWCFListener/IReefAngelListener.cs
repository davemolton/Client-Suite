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
using System.Reflection;

namespace ReefAngelWCFListener
{
   
    [ServiceContract(CallbackContract= typeof(IReefAngelCallback))]
    public interface IReefAngelListener
    {

        [OperationContract]
        Guid Subscribe();

        [OperationContract(IsOneWay = true)]
        void Unsubscribe(Guid clientId);

        [OperationContract(IsOneWay = true)]
        void KeepConnection();

        [OperationContract]
        void SendParams(ReefAngelParams paras);

        [OperationContract]
        void SendRelayData(ReefAngelRelayData relayData);
    }

    public interface IReefAngelCallback
    {
        [OperationContract]
        void HandleParams(ReefAngelParams paras);

        [OperationContract]
        void HandleRelayData(ReefAngelRelayData relayData);
    }
}
