// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.Opc.Twin.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Request node browsing continuation
    /// </summary>
    public partial class BrowseNextRequestApiModel
    {
        /// <summary>
        /// Initializes a new instance of the BrowseNextRequestApiModel class.
        /// </summary>
        public BrowseNextRequestApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the BrowseNextRequestApiModel class.
        /// </summary>
        /// <param name="continuationToken">Continuation token from previews
        /// browse request.
        /// (mandatory)</param>
        /// <param name="abort">Whether to abort browse and release.
        /// (default: false)</param>
        /// <param name="targetNodesOnly">Whether to collapse all references
        /// into a set of
        /// unique target nodes and not show reference
        /// information.
        /// (default is false)</param>
        /// <param name="readVariableValues">Whether to read variable values on
        /// target nodes.
        /// (default is false)</param>
        /// <param name="header">Optional request header</param>
        public BrowseNextRequestApiModel(string continuationToken, bool? abort = default(bool?), bool? targetNodesOnly = default(bool?), bool? readVariableValues = default(bool?), RequestHeaderApiModel header = default(RequestHeaderApiModel))
        {
            ContinuationToken = continuationToken;
            Abort = abort;
            TargetNodesOnly = targetNodesOnly;
            ReadVariableValues = readVariableValues;
            Header = header;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets continuation token from previews browse request.
        /// (mandatory)
        /// </summary>
        [JsonProperty(PropertyName = "continuationToken")]
        public string ContinuationToken { get; set; }

        /// <summary>
        /// Gets or sets whether to abort browse and release.
        /// (default: false)
        /// </summary>
        [JsonProperty(PropertyName = "abort")]
        public bool? Abort { get; set; }

        /// <summary>
        /// Gets or sets whether to collapse all references into a set of
        /// unique target nodes and not show reference
        /// information.
        /// (default is false)
        /// </summary>
        [JsonProperty(PropertyName = "targetNodesOnly")]
        public bool? TargetNodesOnly { get; set; }

        /// <summary>
        /// Gets or sets whether to read variable values on target nodes.
        /// (default is false)
        /// </summary>
        [JsonProperty(PropertyName = "readVariableValues")]
        public bool? ReadVariableValues { get; set; }

        /// <summary>
        /// Gets or sets optional request header
        /// </summary>
        [JsonProperty(PropertyName = "header")]
        public RequestHeaderApiModel Header { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ContinuationToken == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ContinuationToken");
            }
        }
    }
}
