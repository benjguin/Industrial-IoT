// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.Opc.History.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Historic event
    /// </summary>
    public partial class HistoricEventApiModel
    {
        /// <summary>
        /// Initializes a new instance of the HistoricEventApiModel class.
        /// </summary>
        public HistoricEventApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the HistoricEventApiModel class.
        /// </summary>
        /// <param name="eventFields">The selected fields of the event</param>
        public HistoricEventApiModel(IList<object> eventFields = default(IList<object>))
        {
            EventFields = eventFields;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the selected fields of the event
        /// </summary>
        [JsonProperty(PropertyName = "eventFields")]
        public IList<object> EventFields { get; set; }

    }
}
