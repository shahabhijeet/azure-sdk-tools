// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.


namespace MS.Az.Mgmt.CI.BuildTasks.BuildTasks.PreBuild
{
    using MS.Az.Mgmt.CI.BuildTasks.Common.Base;
    using MS.Az.Mgmt.CI.BuildTasks.Common.Utilities;
    using MS.Az.Mgmt.CI.Common.ExtensionMethods;
    using MS.Az.Mgmt.CI.Common.Services;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class DetectRPScopeTask : NetSdkBuildTask
    {
        #region const

        #endregion

        #region fields
        GitHubService _ghSvc;
        #endregion

        #region Properties
        #region task input properties
        public string GH_PRNumber { get; set; }

        public string GH_RepositoryId { get; set; }

        #endregion

        #region task output properties

        string[] MultipleScopes { get; set; }
        #endregion


        public override string NetSdkTaskName => "DetectRPScopeTask";

        GitHubService GHSvc
        {
            get
            {
                if(_ghSvc == null)
                {
                    _ghSvc = new GitHubService(TaskLogger);
                }

                return _ghSvc;
            }
        }

        long RepoId { get; set; }

        long PrNumber { get; set; }
        #endregion

        #region Constructor
        public DetectRPScopeTask()
        {
            Init();
        }

        public DetectRPScopeTask(string RepoId, string PrNumber)
        {
            GH_RepositoryId = RepoId;
            GH_PRNumber = PrNumber;
            Init();
        }

        void Init()
        {
            Check.NotEmptyNotNull(GH_RepositoryId, "GH_RepositoryId");
            Check.NotEmptyNotNull(GH_PRNumber, "GH_PRNumber");

            GH_RepositoryId = GH_RepositoryId.Trim();
            GH_PRNumber = GH_PRNumber.Trim();

            Check.NonNegativeNumber(GH_PRNumber, "GH_PRNumber");
            Check.NonNegativeNumber(GH_RepositoryId, "GH_RepositoryId");

            RepoId = Convert.ToInt64(GH_RepositoryId);
            PrNumber = Convert.ToInt64(GH_PRNumber);
        }
        #endregion

        #region Public Functions
        public override bool Execute()
        {
            base.Execute();
            List<string> validScopes = GetRPScopes();

            if(validScopes.NotNullOrAny<string>())
            {
                MultipleScopes = validScopes.ToArray<string>();
            }

            return TaskLogger.TaskSucceededWithNoErrorsLogged;
        }


        /// <summary>
        /// Detect valid scope based on the change list in the PR
        /// Get affected files and find scope based on directory that contains .sln file
        /// </summary>
        /// <returns></returns>
        List<string> GetRPScopes()
        {
            TaskLogger.LogInfo("Trying to get Pr info for PrNumber:'{0}'", PrNumber.ToString());
            FileSystemUtility fileSysUtil = new FileSystemUtility();
            IEnumerable<string> prFileList = GHSvc.PR.GetPullRequestFileList(RepoId, PrNumber);
            TaskLogger.LogInfo("List of files from PR", prFileList);

            Dictionary<string, string> RPDirs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach(string filePath in prFileList)
            {
                string slnDirPath = fileSysUtil.TraverUptoRootWithFileExtension(filePath);

                if (Directory.Exists(slnDirPath))
                {
                    if(!RPDirs.ContainsKey(slnDirPath))
                    {
                        RPDirs.Add(slnDirPath, slnDirPath);
                    }
                }
                else
                {
                    TaskLogger.LogWarning("RPScope Detection: '{0}' does not exists", slnDirPath);
                }
            }

            TaskLogger.LogInfo("Number of RPs detected", RPDirs);

            return RPDirs.Select<KeyValuePair<string, string>, string>((item) => item.Key).ToList<string>();
        }
        #endregion

        #region private functions
        //List<string> GetRPDirList(IE)
        #endregion

    }
}
