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



        [SetUp]
        public virtual void Setup()
        {
            GitRepoPath = GitRepoUtils.GetUniqueTempFolder("unitTest");
            var testOriginRepo = GitRepoUtils.CreateEmptyTestRepo(GitRepoPath);            
        }


        [TearDown]
        public virtual void TearDown()
        {
            GitRepoUtils.DeleteGitDirectory(GitRepoPath);
            Directory.Delete(GitRepoPath);
        }

        public string GitRepoPath { get; set; }

    }
}
