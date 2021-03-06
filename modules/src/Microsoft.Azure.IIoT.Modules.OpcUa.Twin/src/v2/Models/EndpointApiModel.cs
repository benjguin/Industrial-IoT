// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Modules.OpcUa.Twin.v2.Models {
    using Microsoft.Azure.IIoT.OpcUa.Registry.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Endpoint model for edgeservice api
    /// </summary>
    public class EndpointApiModel {

        /// <summary>
        /// Default constructor
        /// </summary>
        public EndpointApiModel() { }

        /// <summary>
        /// Create api model from service model
        /// </summary>
        /// <param name="model"></param>
        public EndpointApiModel(EndpointModel model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }
            Url = model.Url;
            AlternativeUrls = model.AlternativeUrls;
            User = model.User == null ? null :
                new CredentialApiModel(model.User);
            Certificate = model.Certificate;
            SecurityMode = model.SecurityMode;
            SecurityPolicy = model.SecurityPolicy;
        }

        /// <summary>
        /// Create service model from api model
        /// </summary>
        public EndpointModel ToServiceModel() {
            return new EndpointModel {
                Url = Url,
                AlternativeUrls = AlternativeUrls,
                User = User?.ToServiceModel(),
                Certificate = Certificate,
                SecurityMode = SecurityMode,
                SecurityPolicy = SecurityPolicy,
            };
        }

        /// <summary>
        /// Endpoint url to use to connect with
        /// </summary>
        [JsonProperty(PropertyName = "Url")]
        public string Url { get; set; }

        /// <summary>
        /// Alternative endpoint urls that can be used for
        /// accessing and validating the server
        /// </summary>
        [JsonProperty(PropertyName = "AlternativeUrls",
            NullValueHandling = NullValueHandling.Ignore)]
        public HashSet<string> AlternativeUrls { get; set; }

        /// <summary>
        /// User Authentication
        /// </summary>
        [JsonProperty(PropertyName = "User",
            NullValueHandling = NullValueHandling.Ignore)]
        public CredentialApiModel User { get; set; }

        /// <summary>
        /// Security Mode to use for communication
        /// default to best.
        /// </summary>
        [JsonProperty(PropertyName = "SecurityMode",
            NullValueHandling = NullValueHandling.Ignore)]
        public SecurityMode? SecurityMode { get; set; }

        /// <summary>
        /// Security policy uri to use for communication
        /// default to best.
        /// </summary>
        [JsonProperty(PropertyName = "SecurityPolicy",
            NullValueHandling = NullValueHandling.Ignore)]
        public string SecurityPolicy { get; set; }

        /// <summary>
        /// Certificate to validate against or null to trust none.
        /// </summary>
        [JsonProperty(PropertyName = "Certificate",
            NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Certificate { get; set; }
    }
}
