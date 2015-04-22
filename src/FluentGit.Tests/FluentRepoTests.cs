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

        private string _CloneDir;

        [Test]
        public void Can_Load_Local_Repo_By_Folder_Path()
        {
            var repo = new FluentRepo().Load(GitRepoPath);
        }

        [Test]
        public void Can_Clone_Local_Repo()
        {

            // TODO: in test setup, create a local git repo to use a source for the clone.
            // In test tear down, remove the source git repo, and the newly created clone repo.
            _CloneDir = TestUtils.GetUniqueTempFolder(ParentTempFolderName);

            // Act
            var repo = new FluentRepo().Clone()
                                       .FromDirectory(GitRepoPath)
                                       .ToDirectory(_CloneDir)
                                       .Obtain();

            // Assert
            // 1. verify the clone now exists.
            var repository = new Repository(_CloneDir);
            Assert.That(repository, Is.Not.Null);
            Assert.IsFalse(TestUtils.IsBareRepo(repository));

        }

        [Test]
        public void Can_Bare_Clone_Local_Repo()
        {

            // TODO: in test setup, create a local git repo to use a source for the clone.
            // In test tear down, remove the source git repo, and the newly created clone repo.
            _CloneDir = TestUtils.GetUniqueTempFolder(ParentTempFolderName);

            // Act
            var repo = new FluentRepo().Clone()
                                       .FromDirectory(GitRepoPath)
                                       .ToDirectory(_CloneDir)
                                       .Bare()
                                       .Obtain();

            // Assert
            // 1. verify the clone now exists, and its a bare repo.
            var repository = new Repository(_CloneDir);
            Assert.IsTrue(TestUtils.IsBareRepo(repository));

        }

        [Test]
        public void Can_Clone_Remote_Repo()
        {

            // TODO: in test setup, create a local git repo to use a source for the clone.
            // In test tear down, remove the source git repo, and the newly created clone repo.
            _CloneDir = TestUtils.GetUniqueTempFolder(ParentTempFolderName);

            // Act
            var repo = new FluentRepo().Clone()
                                       .FromUrl("https://github.com/dazinator/FluentGit.git")
                                       .ToDirectory(_CloneDir)
                                       .Obtain();

            // Assert
            // 1. verify the clone now exists.
            var repository = new Repository(_CloneDir);
            Assert.That(repository, Is.Not.Null);
            Assert.IsFalse(TestUtils.IsBareRepo(repository));
        }

        [Test]
        public void Can_Bare_Clone_Remote_Repo()
        {

            // TODO: in test setup, create a local git repo to use a source for the clone.
            // In test tear down, remove the source git repo, and the newly created clone repo.
            _CloneDir = TestUtils.GetUniqueTempFolder(ParentTempFolderName);

            // Act
            var repo = new FluentRepo().Clone()
                                       .FromUrl("https://github.com/dazinator/FluentGit.git")
                                       .ToDirectory(_CloneDir)
                                       .Bare()
                                       .Obtain();

            // Assert
            // 1. verify the clone now exists.
            var repository = new Repository(_CloneDir);
            Assert.IsTrue(TestUtils.IsBareRepo(repository));

        }

        [Test]
        public void Can_Iterate_Branches()
        {
            // add a branch tot he local repo and then test we can access it

            var repository = new Repository(GitRepoPath);
            string branchName = "testing";
            TestUtils.AddBranch(repository, branchName);


            var branch = new FluentRepo().Load(GitRepoPath)
                                       .Branches.First(a => a.Name == branchName);

            Assert.That(branch, Is.Not.Null);
            Assert.That(branch.Name, Is.EqualTo(branchName));

        }


        public override void TearDown()
        {
            base.TearDown();
            var cloneDir = new DirectoryInfo(_CloneDir);
            if (cloneDir.Exists)
            {
                TestUtils.DeleteDirectory(cloneDir);
            }

        }

    }
}
