// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Registry.Models {
    using Microsoft.Azure.IIoT.Hub;
    using Microsoft.Azure.IIoT.Hub.Models;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Base device registration
    /// </summary>
    public static class DeviceRegistrationEx {

        /// <summary>
        /// Convert twin to registration information.
        /// </summary>
        /// <param name="twin"></param>
        /// <param name="onlyServerState"></param>
        /// <returns></returns>
        public static DeviceRegistration ToRegistration(this DeviceTwinModel twin,
            bool onlyServerState = false) {
            if (twin == null) {
                return null;
            }
            var type = twin.Tags.GetValueOrDefault<string>(nameof(BaseRegistration.DeviceType), null);
            if (string.IsNullOrEmpty(type) && twin.Properties.Reported != null) {
                type = twin.Properties.Reported.GetValueOrDefault<string>(TwinProperty.Type, null);
                if (string.IsNullOrEmpty(type)) {
                    type = twin.Tags.GetValueOrDefault<string>(TwinProperty.Type, null);
                }
            }
            switch (type?.ToLowerInvariant() ?? "") {
                case IdentityType.Gateway:
                    return twin.ToGatewayRegistration();
                case IdentityType.Endpoint:
                    return twin.ToEndpointRegistration(onlyServerState);
                case IdentityType.Application:
                    return twin.ToApplicationRegistration();
                case IdentityType.Supervisor:
                    return twin.ToSupervisorRegistration(onlyServerState);
                case IdentityType.Publisher:
                    return twin.ToPublisherRegistration(onlyServerState);
            }
            return null;
        }
    }
}
