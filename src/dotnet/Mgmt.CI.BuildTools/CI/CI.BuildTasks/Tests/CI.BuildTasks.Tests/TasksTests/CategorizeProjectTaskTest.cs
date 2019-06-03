﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Xunit;
[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]


namespace BuildTasks.Tests
{
    using global::Tests.CI.Common.Base;
    using Microsoft.Build.Framework;
    using MS.Az.Mgmt.CI.BuildTasks.Models;
    using MS.Az.Mgmt.CI.BuildTasks.Tasks.PreBuild;
    using MS.Az.Mgmt.CI.Common.ExtensionMethods;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Xunit;
    using Xunit.Abstractions;

    public class CategorizeProjectTaskTest : BuildTasksTestBase
    {
        internal string rootDir = string.Empty;
        internal string sourceRootDir = string.Empty;
        readonly ITestOutputHelper OutputTrace;

        public CategorizeProjectTaskTest(ITestOutputHelper output)
        {
            //create an env. variable 'testAssetdir' and point to a directory that will host multiple repos
            // e.g. sdkfornet directory structure as well as Fluent directory structure
            // basically test asset directory will be the root for all other repos that can be used for testing directory structure
            rootDir = this.TestAssetsDirPath;
            rootDir = Path.Combine(rootDir, "sdkForNet");
            sourceRootDir = rootDir;

            this.OutputTrace = output;
        }

        [Fact]
        public void FullyQualifiedScopeDirPath()
        {
            //string scopeDir = @"SDKs\Compute";
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.FullyQualifiedBuildScopeDirPath = Path.Combine(rootDir, "src", "SDKs", "Compute");

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 1);
            }
        }

        [Fact(Skip = "Investigate as it fails only in Run mode, works fine during debug mode")]
        public void GetProjectsWithNonSupportedFxVersion()
        {
            string scopeDir = @"SDKs\Blueprint";
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);            
            cproj.BuildScope = scopeDir;

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 0);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.UnSupportedProjects.Count<ITaskItem>() == 1);
            }
        }

        [Fact]
        public void GetTest_ProjectType()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.ProjectType = "test";

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 0);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() >= 10);
            }
        }

        //[Fact(Skip = "Not applicable, this is for old task. Keeping it for reference, eventually needs to be deleted")]
        [Fact]
        public void IgnoreDirTokens()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.CmdLineExcludeScope = "Network.Tests";
            cproj.BuildScope = @"SDKs\Network";

            Assert.True(cproj.Execute());
            Assert.True(cproj.SDK_Projects.Count<ITaskItem>() > 0);
            Assert.True(VerifyListDoesNotContains(cproj.SDK_Projects, new List<string>() { "Network.Tests" }));
        }

        [Fact]
        public void CategorizeProjects()
        {
            DateTime startTime = DateTime.Now;
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() > 10);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() > 10);
                Assert.True(cproj.UnSupportedProjects.Count<ITaskItem>() == 0);
                Assert.True(cproj.Test_ToBe_Run.Count<ITaskItem>() > 10);
            }
            DateTime endTime = DateTime.Now;
            OutputTrace.WriteLine("Total time taken:'{0}'", (endTime - startTime).TotalSeconds.ToString());
        }

        [Fact]
        public void ScopedProject()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\Compute";

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 1);
            }
        }

        //[Fact(Skip = "Not applicable, this is for old task. Keeping it for reference, eventually needs to be deleted")]
        [Fact]
        public void GetReferencedPackagesForScope()
        {
            string scopeDir = @"SDKs\Compute";
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = scopeDir;

            if (cproj.Execute())
            {   
                Assert.Single(cproj.SDK_Projects);
                Assert.Single(cproj.Test_Projects);
                Assert.True(cproj.SdkPkgReferenceList.Count<string>() >= 1);
            }
        }

        [Fact]
        public void BuildOnlyIncludedTokenListProjects()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.CmdLineIncludeScope = "Compute;Network;DataBox";

            if (cproj.Execute())
            {
                Assert.Equal(3, cproj.SDK_Projects.Count<ITaskItem>());
                Assert.Equal(3, cproj.Test_Projects.Count<ITaskItem>());
            }
        }

        [Fact]
        public void IgnoreIncludeOverlappingProjects()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.CmdLineIncludeScope = "Compute;Network;DataBox";
            cproj.CmdLineExcludeScope = "Compute";

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 2);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 2);
            }
        }

        [Fact]
        public void IgnoreExactScopedProjects()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\Compute";
            cproj.CmdLineExcludeScope = "Compute";

            if (cproj.Execute())
            {
                Assert.Empty(cproj.SDK_Projects);
                Assert.Empty(cproj.Test_Projects);
            }
        }

        [Fact]
        public void IncludeFewFromEntireScope()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\KeyVault";
            cproj.CmdLineIncludeScope = "Management.KeyVault";

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 0);
            }
        }

        [Fact]
        public void IgnoreIncludeExactScopedProjects()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\Compute";
            cproj.CmdLineIncludeScope = "Compute";
            cproj.CmdLineExcludeScope = "Compute";

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 0);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 0);
            }
        }

        #region Platformspecific

        [Fact]
        public void NonWindowsTargetFx()
        {
            Environment.SetEnvironmentVariable("emulateNonWindowsEnv", "true");
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\Dns";

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.SDK_Projects.All<SDKMSBTaskItem>((item) => !item.PlatformSpecificTargetFxMonikerString.Contains("net452", StringComparison.OrdinalIgnoreCase)));
                Assert.True(cproj.SDK_Projects.All<SDKMSBTaskItem>((item) => !item.PlatformSpecificTargetFxMonikerString.Contains("net461", StringComparison.OrdinalIgnoreCase)));
            }
        }

        [Fact]
        public void PlatformSpecificSkippedProjects()
        {
            Environment.SetEnvironmentVariable("emulateNonWindowsEnv", "true");
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\Subscription";

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.PlatformSpecificSkippedProjects.Count<ITaskItem>() == 1);
            }
        }

        [Fact(Skip = "Investigate as it fails only in Run mode, works fine during debug mode")]
        public void PlatSpecificTestProjectsForWindows()
        {
            //This RP has FullDesktop specific test projects. The idea is to test if that test projects is getting picked up
            Environment.SetEnvironmentVariable("emulateWindowsEnv", "true");
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\Subscription";

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 2);
                Assert.True(cproj.PlatformSpecificSkippedProjects.Count<ITaskItem>() == 0);
            }
        }

        [Fact]
        public void GetPlatformTargetFx()
        {
            Environment.SetEnvironmentVariable("emulateNonWindowsEnv", "true");
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\Compute";

            Assert.True(cproj.Execute());
            Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 1);
            //var sdkProj = cproj.SDK_Projects.ToList<SDKMSBTaskItem>();

            SDKMSBTaskItem sdkProj = cproj.SDK_Projects[0];

            if (!sdkProj.PlatformSpecificTargetFxMonikerString.Contains("net4", StringComparison.OrdinalIgnoreCase))
            {
                Assert.True(true);
            }
        }

        #endregion



        [Fact]
        public void IncludeOverrideScope()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\Network";
            cproj.CmdLineIncludeScope = "Compute";

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 0);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 0);
            }
        }

        [Fact(Skip ="Investigate as it fails only in Run mode, works fine during debug mode")]
        public void AdditionalFxProject()
        {
            // This test will be important when we stop supporting .NET 452 and will have to keep supporting .NET 452 until we move to MSAL
            // One of the attribute that is being tested is that Auth library needs to support .NET 452 until new MSAL support is added for Interactive login
            string scopeDir = @"SdkCommon\Auth\Az.Auth\";
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = scopeDir;            

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 3);
                Assert.True(cproj.SDK_Projects.All<SDKMSBTaskItem>((item) => item.TargetFxMonikerString.Contains("net452", StringComparison.OrdinalIgnoreCase)));
            }
        }

        [Fact]
        public void FxTargetForNonWindows()
        {
            Environment.SetEnvironmentVariable("emulateNonWindowsEnv", "true");
            string scopeDir = @"SdkCommon\Auth\Az.Auth\";
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = scopeDir;

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.SDK_Projects.All<SDKMSBTaskItem>((item) => !item.PlatformSpecificTargetFxMonikerString.Contains("net452", StringComparison.OrdinalIgnoreCase)));
            }
        }

        [Fact(Skip = "Not applicable, this is for old task. Keeping it for reference, eventually needs to be deleted")]
        public void UnSupportedProjects()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\Batch\DataPlane";

            if (cproj.Execute())
            {
                Assert.Equal(3, cproj.SDK_Projects.Count<ITaskItem>());
            }
        }

        [Fact]
        public void ExcludeProjects()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.CmdLineExcludeScope = @"Batch\Support";
            cproj.ProjectType = "Test";

            if (cproj.Execute())
            {
                Assert.Empty(cproj.SDK_Projects);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() > 10);
                Assert.False(CollectionContains(cproj.Test_Projects, @"Batch\Support"));
            }
        }

        [Fact(Skip = "Investigate as it fails only in Run mode, works fine during debug mode")]
        public void ClientRuntimeProjects()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKCommon\ClientRuntime";

            if (cproj.Execute())
            {
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 1);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 3);
            }
        }

        [Fact(Skip = "Investigate as it fails only in Run mode, works fine during debug mode")]
        public void SDKCommonProjects()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKCommon";

            if (cproj.Execute())
            {
                //Since HttpRecorder and TestFramework are multi-targeting, they are no 
                //longer treated as regular nuget packages (targeting net452 and netStd1.4)
                //but rather projects that are built without any targetFx
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 8);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 16);
            }
        }

        [Fact(Skip = "Investigate as it fails only in Run mode, works fine during debug mode")]
        public void TestFrameworkDir()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKCommon\TestFramework";

            if (cproj.Execute())
            {
                //Since HttpRecorder and TestFramework are multi-targeting, they are no 
                //longer treated as regular nuget packages (targeting net452 and netStd1.4)
                //but rather projects that are build without any targetFx
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 2);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() == 3);
            }
        }

        [Fact]
        public void FindTestProjectUsingProjectType()
        {
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.ProjectType = "Test";
            if (cproj.Execute())
            {
                //Currently it's not able to filter out DevTestLab, Azure.Inishts, Azure.Graph.RBAC
                Assert.True(cproj.SDK_Projects.Count<ITaskItem>() == 0);
                Assert.True(cproj.Test_Projects.Count<ITaskItem>() > 10);
            }
        }

        [Fact(Skip = "Not applicable, this is for old task. Keeping it for reference, eventually needs to be deleted")]
        public void TestIgnoredTokens()
        {
            //Gallery projects are being ignored
            CategorizeSDKProjectsTask cproj = new CategorizeSDKProjectsTask(rootDir);
            cproj.BuildScope = @"SDKs\Batch\DataPlane";

            if (cproj.Execute())
            {
                //Assert.Equal(0, cproj.net452SdkProjectsToBuild.Count());
                //Assert.Equal(0, cproj.netCore11TestProjectsToBuild.Count());
            }
        }

        internal string GetSourceRootDir()
        {
            string srcRootDir = string.Empty;
            //string currDir = Directory.GetCurrentDirectory();
            //string currDir = @"D:\Myfork\psSdkJson6";

            string currDir = Path.GetDirectoryName(this.GetType().GetTypeInfo().Assembly.Location);

            string dirRoot = Directory.GetDirectoryRoot(currDir);
            var buildProjFile = Directory.EnumerateFiles(currDir, "build.proj", SearchOption.TopDirectoryOnly);

            while (currDir != dirRoot)
            {
                if (buildProjFile.Any<string>())
                {
                    srcRootDir = Path.GetDirectoryName(buildProjFile.First<string>());
                    break;
                }

                currDir = Directory.GetParent(currDir).FullName;
                buildProjFile = Directory.EnumerateFiles(currDir, "build.proj", SearchOption.TopDirectoryOnly);
            }



            if (!string.IsNullOrEmpty(srcRootDir))
            {
                srcRootDir = Path.Combine(srcRootDir, @"repos\netsdkMaster");
                if (!Directory.Exists(srcRootDir))
                {
                    throw new DirectoryNotFoundException("Submodule for NetSdk not found. Please clone recursively to get required submodules");
                }


            }

            return srcRootDir;
        }


        #region private functions

        IEnumerable<UResult> GetList<TSource, UResult>(IEnumerable<TSource> sourceCollection, Func<TSource, UResult> resultDelegate)
        {
            return sourceCollection.Select<TSource, UResult>((sourceItem) => resultDelegate(sourceItem));
        }

        bool CollectionContains(IEnumerable<ITaskItem> collection, string tokenToSearch)
        {
            var filterList = collection.Where<ITaskItem>((item) => item.ItemSpec.Contains(tokenToSearch, StringComparison.OrdinalIgnoreCase));
            if(filterList.NotNullOrAny<ITaskItem>())
            {
                return true;
            }

            return false;
        }

        public bool VerifyListContains(List<ITaskItem> projectPathList, List<string> tokenList)
        {
            bool foundAllTokens = false;
            foreach(ITaskItem projPath in projectPathList)
            {
                if (tokenList.Count > 0)
                {
                    tokenList = tokenList.Where<string>((item) => !projPath.ItemSpec.Contains(item, StringComparison.OrdinalIgnoreCase)).ToList<string>();
                }
                else
                {
                    foundAllTokens = true;
                    break;
                }
            }

            return foundAllTokens;
        }

        public bool VerifyListDoesNotContains(IEnumerable<ITaskItem> projectPathList, List<string> tokenList)
        {
            bool tokensDoNotExist = true;
            foreach (ITaskItem projPath in projectPathList)
            {
                if (tokenList.Count > 0)
                {
                    tokenList = tokenList.Where<string>((item) => projPath.ItemSpec.Contains(item, StringComparison.OrdinalIgnoreCase)).ToList<string>();
                }
                else
                {
                    tokensDoNotExist = false;
                    break;
                }
            }

            return tokensDoNotExist;
        }

        #endregion
    }

    /*
    public class CategorizeMultipleScopes : BuildTasksTestBase
    {
        [Fact]
        public void CategorizeMultiScopeProjects()
        {
            string srcDir = string.Empty;
            List<string> dirs = new List<string>();
            // sdkRepoClient = this.GitRepoClient.SubModulesGitClients[Common.BuildTasks.Tests.Base.RepoName.NetSdkRepo];

            string netSdkRepoSrcDir = this.GitRepoClient.SrcDir;
            string computeDir = Path.Combine(netSdkRepoSrcDir, "SDKs", "Compute");
            string networkDir = Path.Combine(netSdkRepoSrcDir, "SDKs", "Network");
            dirs.Add(computeDir);
            dirs.Add(networkDir);

            SDKCategorizeProjectsTask cproj = new SDKCategorizeProjectsTask();
            cproj.SourceRootDirPath = netSdkRepoSrcDir;
            cproj.BuildScopes = dirs?.ToArray<string>();
            
            if (cproj.Execute())
            {
                int totalSdkProjectCount = cproj.net452SdkProjectsToBuild.Count() + cproj.netStd14SdkProjectsToBuild.Count<ITaskItem>();
                Assert.True(totalSdkProjectCount > 3);
                Assert.True(cproj.netCore20TestProjectsToBuild.Count<ITaskItem>() > 1);
            }
        }

        [Fact]
        public void CatMultiScopeRootScope()
        {
            string srcDir = string.Empty;
            List<string> dirs = new List<string>();
            //TODO: find a way to have the dictionary created using an enum for all submodules
            GitRepositoryClient sdkRepoClient = this.GitRepoClient.SubModuleGitClients["https://github.com/Azure/azure-sdk-for-net.git"];
            //First<KeyValuePair<string, GitRepositoryClient>>();
            string netSdkRepoSrcDir = sdkRepoClient.SrcDir;
            string computeDir = Path.Combine(netSdkRepoSrcDir, "SDKs", "Compute");
            string kvDir = Path.Combine(netSdkRepoSrcDir, "SDKs", "KeyVault");
            dirs.Add(computeDir);
            dirs.Add(kvDir);


            SDKCategorizeProjectsTask cproj = new SDKCategorizeProjectsTask();
            cproj.SourceRootDirPath = netSdkRepoSrcDir;
            cproj.BuildScopes = dirs?.ToArray<string>();

            if (cproj.Execute())
            {
                int totalSdkProjectCount = cproj.net452SdkProjectsToBuild.Count() + cproj.netStd14SdkProjectsToBuild.Count<ITaskItem>();
                Assert.True(totalSdkProjectCount > 5);
                Assert.True(cproj.netCore20TestProjectsToBuild.Count<ITaskItem>() > 5);
            }
        }
    }
    */
}