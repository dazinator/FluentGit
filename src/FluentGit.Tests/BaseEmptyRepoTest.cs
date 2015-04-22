using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FluentGit.Tests
{
    public abstract class BaseEmptyRepoTest
    {

        protected const string ParentTempFolderName = "unitTest";


        [SetUp]
        public virtual void Setup()
        {
            GitRepoPath = TestUtils.GetUniqueTempFolder(ParentTempFolderName);
            var testOriginRepo = TestUtils.CreateEmptyTestRepo(GitRepoPath);            
        }


        [TearDown]
        public virtual void TearDown()
        {
            TestUtils.DeleteGitDirectory(GitRepoPath);
            Directory.Delete(GitRepoPath);
        }

        public string GitRepoPath { get; set; }

    }
}
