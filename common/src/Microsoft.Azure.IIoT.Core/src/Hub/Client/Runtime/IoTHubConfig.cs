// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Hub.Client.Runtime {
    using Microsoft.Azure.IIoT.Utils;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// IoT hub services runtime configuration
    /// </summary>
    public class IoTHubConfig : ConfigBase, IIoTHubConfig {

        /// <summary>
        /// Service configuration
        /// </summary>
        private const string kIoTHubConnectionStringKey = "IoTHubConnectionString";

        /// <summary>IoT hub connection string</summary>
        public string IoTHubConnString => GetStringOrDefault(kIoTHubConnectionStringKey,
                GetStringOrDefault("PCS_IOTHUB_CONNSTRING", GetStringOrDefault("_HUB_CS", null)));
        /// <summary>Resource Id</summary>
        public string IoTHubResourceId { get; set; }

        /// <summary>
        /// Configuration constructor
        /// </summary>
        /// <param name="configuration"></param>
        public IoTHubConfig(IConfigurationRoot configuration) :
            base(configuration) {
        }
    }
}
