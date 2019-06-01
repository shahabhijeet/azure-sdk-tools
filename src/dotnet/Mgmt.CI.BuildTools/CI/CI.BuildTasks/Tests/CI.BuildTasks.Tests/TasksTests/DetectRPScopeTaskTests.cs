// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Tests.CI.BuildTasks.TasksTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Tests.CI.Common.Base;
    using Xunit.Abstractions;

    public class DetectRPScopeTaskTests : BuildTasksTestBase
    {
        #region CONST
        const int SDKNET_REPOID = 
        #endregion
        #region field
        internal string rootDir = string.Empty;
        internal string sourceRootDir = string.Empty;
        readonly ITestOutputHelper OutputTrace;
        #endregion

        public DetectRPScopeTaskTests(ITestOutputHelper output)
        {
            //create an env. variable 'testAssetdir' and point to a directory that will host multiple repos
            // e.g. sdkfornet directory structure as well as Fluent directory structure
            // basically test asset directory will be the root for all other repos that can be used for testing directory structure
            rootDir = this.TestAssetsDirPath;
            rootDir = Path.Combine(rootDir, "sdkForNet");
            sourceRootDir = rootDir;

            this.OutputTrace = output;
        }

        public void SingleScope()
        {

        }
    }
}
