﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Hub {
    using Microsoft.Azure.IIoT.Hub.Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Twin services
    /// </summary>
    public interface IIoTHubJobServices {

        /// <summary>
        /// Create new job
        /// </summary>
        /// <param name="job"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<JobModel> CreateAsync(JobModel job,
            CancellationToken ct = default);

        /// <summary>
        /// Returns job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<JobModel> RefreshAsync(string jobId,
            CancellationToken ct = default);

        /// <summary>
        /// Delete job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task CancelAsync(string jobId,
            CancellationToken ct = default);
    }
}