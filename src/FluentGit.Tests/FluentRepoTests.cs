using LibGit2Sharp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentGit;
using System.IO;

namespace FluentGit.Tests
{
    [TestFixture]
    public class FluentRepoTests : BaseEmptyRepoTest
    {

        [Test]
        public void Can_Load_Local_Repo_By_Folder_Path()
        {
            var repo = new Repo().Load(GitRepoPath);
        }

        [Test]
        public void Can_Clone_Repo()
        {

            // TODO: in test setup, create a local git repo to use a source for the clone.
            // In test tear down, remove the source git repo, and the newly created clone repo.
            var repo = new Repo().Clone()
                                       .FromUrl("http://somegiturl.git")
                                       .ToDirectory("some folder")
                                       .Bare()
                                       .WithCredentials("username", "password")
                                       .Obtain();

        }

    }
}
