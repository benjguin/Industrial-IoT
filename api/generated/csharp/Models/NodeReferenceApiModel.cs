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
    /// reference model
    /// </summary>
    public partial class NodeReferenceApiModel
    {
        /// <summary>
        /// Initializes a new instance of the NodeReferenceApiModel class.
        /// </summary>
        public NodeReferenceApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the NodeReferenceApiModel class.
        /// </summary>
        /// <param name="target">Target node</param>
        /// <param name="referenceTypeId">Reference Type identifier</param>
        /// <param name="direction">Browse direction of reference. Possible
        /// values include: 'Forward', 'Backward', 'Both'</param>
        public NodeReferenceApiModel(NodeApiModel target, string referenceTypeId = default(string), BrowseDirection? direction = default(BrowseDirection?))
        {
            ReferenceTypeId = referenceTypeId;
            Direction = direction;
            Target = target;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets reference Type identifier
        /// </summary>
        [JsonProperty(PropertyName = "referenceTypeId")]
        public string ReferenceTypeId { get; set; }

        /// <summary>
        /// Gets or sets browse direction of reference. Possible values
        /// include: 'Forward', 'Backward', 'Both'
        /// </summary>
        [JsonProperty(PropertyName = "direction")]
        public BrowseDirection? Direction { get; set; }

        /// <summary>
        /// Gets or sets target node
        /// </summary>
        [JsonProperty(PropertyName = "target")]
        public NodeApiModel Target { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Target == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Target");
            }
            if (Target != null)
            {
                Target.Validate();
            }
        }
    }
}
