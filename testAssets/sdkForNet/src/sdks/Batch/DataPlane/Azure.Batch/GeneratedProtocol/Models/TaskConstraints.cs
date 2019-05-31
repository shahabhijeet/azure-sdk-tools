// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.Batch.Protocol.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Execution constraints to apply to a task.
    /// </summary>
    public partial class TaskConstraints
    {
        /// <summary>
        /// Initializes a new instance of the TaskConstraints class.
        /// </summary>
        public TaskConstraints()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the TaskConstraints class.
        /// </summary>
        /// <param name="maxWallClockTime">The maximum elapsed time that the
        /// task may run, measured from the time the task starts. If the task
        /// does not complete within the time limit, the Batch service
        /// terminates it.</param>
        /// <param name="retentionTime">The minimum time to retain the task
        /// directory on the compute node where it ran, from the time it
        /// completes execution. After this time, the Batch service may delete
        /// the task directory and all its contents.</param>
        /// <param name="maxTaskRetryCount">The maximum number of times the
        /// task may be retried. The Batch service retries a task if its exit
        /// code is nonzero.</param>
        public TaskConstraints(System.TimeSpan? maxWallClockTime = default(System.TimeSpan?), System.TimeSpan? retentionTime = default(System.TimeSpan?), int? maxTaskRetryCount = default(int?))
        {
            MaxWallClockTime = maxWallClockTime;
            RetentionTime = retentionTime;
            MaxTaskRetryCount = maxTaskRetryCount;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the maximum elapsed time that the task may run,
        /// measured from the time the task starts. If the task does not
        /// complete within the time limit, the Batch service terminates it.
        /// </summary>
        /// <remarks>
        /// If this is not specified, there is no time limit on how long the
        /// task may run.
        /// </remarks>
        [JsonProperty(PropertyName = "maxWallClockTime")]
        public System.TimeSpan? MaxWallClockTime { get; set; }

        /// <summary>
        /// Gets or sets the minimum time to retain the task directory on the
        /// compute node where it ran, from the time it completes execution.
        /// After this time, the Batch service may delete the task directory
        /// and all its contents.
        /// </summary>
        /// <remarks>
        /// The default is 7 days, i.e. the task directory will be retained for
        /// 7 days unless the compute node is removed or the job is deleted.
        /// </remarks>
        [JsonProperty(PropertyName = "retentionTime")]
        public System.TimeSpan? RetentionTime { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of times the task may be retried.
        /// The Batch service retries a task if its exit code is nonzero.
        /// </summary>
        /// <remarks>
        /// Note that this value specifically controls the number of retries
        /// for the task executable due to a nonzero exit code. The Batch
        /// service will try the task once, and may then retry up to this
        /// limit. For example, if the maximum retry count is 3, Batch tries
        /// the task up to 4 times (one initial try and 3 retries). If the
        /// maximum retry count is 0, the Batch service does not retry the task
        /// after the first attempt. If the maximum retry count is -1, the
        /// Batch service retries the task without limit.
        /// </remarks>
        [JsonProperty(PropertyName = "maxTaskRetryCount")]
        public int? MaxTaskRetryCount { get; set; }

    }
}
