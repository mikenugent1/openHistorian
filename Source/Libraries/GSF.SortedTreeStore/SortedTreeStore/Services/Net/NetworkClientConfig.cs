//******************************************************************************************************
//  NetworkClientConfig.cs - Gbtc
//
//  Copyright � 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/8/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//       
//
//******************************************************************************************************

namespace GSF.SortedTreeStore.Services.Net
{
    /// <summary>
    /// The options that are passed to a <see cref="NetworkClient"/>.
    /// </summary>
    public class NetworkClientConfig
    {
        /// <summary>
        /// The port number to connect to.
        /// </summary>
        public int NetworkPort = 38402;

        /// <summary>
        /// The name of the server to connect to, or the IP address to use.
        /// </summary>
        public string ServerNameOrIp = "localhost";

        /// <summary>
        /// Gets if integrated security will be used, or if the default user will be used.
        /// </summary>
        public bool UseIntegratedSecurity;

    }
}