// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Diagnostics {
    using Microsoft.ApplicationInsights;
    using System.Collections.Generic;

    /// <summary>
    /// Metric logger
    /// </summary>
    public sealed class MetricLogger : IMetricLogger  {

        /// <summary>
        /// Create metric logger
        /// </summary>
        /// <param name="config"></param>
        public MetricLogger(IApplicationInsightsConfig config) {
            _telemetryClient = new TelemetryClient(config.TelemetryConfiguration);
        }

         /// <inheritdoc/>
        public void TrackEvent(string name) {
            _telemetryClient.GetMetric("trackEvent-" + name).TrackValue(1);
        }

        /// <inheritdoc/>
        public void TrackValue(string name, int value) {
            _telemetryClient.GetMetric("trackValue-" + name).TrackValue(value);
        }

        /// <inheritdoc/>
        public void TrackDuration(string name, double milliseconds) {
            var metrics = new Dictionary<string, double>
                {{"processingTime-" + name, milliseconds}};
            _telemetryClient.TrackEvent("processingTime-" + name, null, metrics);
        }

        private readonly TelemetryClient _telemetryClient;
    }
}