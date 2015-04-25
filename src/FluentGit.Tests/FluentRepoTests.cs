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
            Assert.IsFalse(repository.IsBareRepo());

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
            Assert.IsTrue(repository.IsBareRepo());

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
            Assert.IsFalse(repository.IsBareRepo());
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
            Assert.IsTrue(repository.IsBareRepo());

        }

        [Test]
        public void Can_Iterate_Branches()
        {
            // add a branch tot he local repo and then test we can access it
            var repository = new Repository(GitRepoPath);
            string branchName = "testing";
            repository.AddBranch(branchName);


            var branch = new FluentRepo().Load(GitRepoPath)
                                         .Branches.First(a => a.Name == "testing");

            Assert.That(branch, Is.Not.Null);
            Assert.That(branch.Name, Is.EqualTo(branchName));

        }

        [Test]
        public void Can_Iterate_Remotes()
        {
            // add a remote to the local repo and then test we can access it
            var repository = new Repository(GitRepoPath);
            repository.Network.Remotes.Add("fluentgit", "https://github.com/dazinator/FluentGit.git");

            //

            // Act
            var remote = new FluentRepo().Load(GitRepoPath)
                                         .Remotes.First(a => a.Name == "fluentgit");

            // Assert
            Assert.That(remote, Is.Not.Null);
            Assert.That(remote.Name, Is.EqualTo("fluentgit"));
            Assert.That(remote.Url, Is.EqualTo("https://github.com/dazinator/FluentGit.git"));
        }

        [Test]
        public void Can_Add_Remote()
        {
            // Act
            var remote = new FluentRepo().Load(GitRepoPath)
                                         .AddRemote(a =>
                                             a.WithName("fluentgit")
                                              .WithUrl("https://github.com/dazinator/FluentGit.git"));


            var repository = new Repository(GitRepoPath);
            var addedRemote = repository.Network.Remotes.Single(a => a.Name == "fluentgit" && a.Url == "https://github.com/dazinator/FluentGit.git");

        }


        [Test]
        public void Can_Add_Multliple_Remotes_And_Specify_Fecth_RefSpec()
        {
            // Act
            var fluentRepo = new FluentRepo().Load(GitRepoPath)
                                          .AddRemote(a =>
                                             a.WithName("fluentgit")
                                              .WithUrl("https://github.com/dazinator/FluentGit.git")
                                              .WithFetchRefSpec(r =>
                                                  r.Source("refs/heads/master")
                                                   .Destination("refs/remotes/fluentgit/master")
                                                   .ForceUpdateIfFastForwardNotPossible()))
                                         .AddRemote(a =>
                                             a.WithName("libgit2sharp")
                                              .WithUrl("https://github.com/libgit2/libgit2sharp.git")
                                              .WithFetchRefSpec("refs/heads/master:refs/remotes/libgit2sharp/master"));


            var repository = new Repository(GitRepoPath);
            var addedRemote = repository.Network.Remotes.Single(a => a.Name == "fluentgit" && a.Url == "https://github.com/dazinator/FluentGit.git");
            var refSpec = addedRemote.FetchRefSpecs.Single();

            Assert.That(refSpec.Source == "refs/heads/master");
            Assert.That(refSpec.Destination == "refs/remotes/fluentgit/master");
            Assert.That(refSpec.ForceUpdate);

            var anotherAddedRemote = repository.Network.Remotes.Single(a => a.Name == "libgit2sharp" && a.Url == "https://github.com/libgit2/libgit2sharp.git");
            refSpec = anotherAddedRemote.FetchRefSpecs.Single();

            Assert.That(refSpec.Source == "refs/heads/master");
            Assert.That(refSpec.Destination == "refs/remotes/libgit2sharp/master");
            Assert.IsFalse(refSpec.ForceUpdate);
        }


        [Test]
        public void Can_Update_Remote()
        {
            // Arrange
            // add a remote to the local repo and then test we can access it

            using (var repository = new Repository(GitRepoPath))
            {
                repository.Network.Remotes.Add("fluentgit", "https://github.com/dazinator/FluentGit.git", "+refs/heads/*:refs/remotes/fluentgit/*");
            }


            // Act
            var repo = new FluentRepo()
                                .Load(GitRepoPath)
                                .UpdateRemote(r => r.Name == "fluentgit",
                                    u =>
                                        u.ChangeUrlTo("https://github.com/libgit2/libgit2sharp.git")
                                         .ChangeRefSpecs()
                                            .Add("+refs/heads/master:refs/remotes/fluentgit/master")
                                        // can alternatively use RefSpecBuilder to build refspec strings:
                                            .Add(b =>
                                             b.Source("refs/heads/master")
                                              .Destination("refs/other/fluentgit/master")
                                              .ForceUpdateIfFastForwardNotPossible())

                                            .AddIfNotExists("+refs/heads/*:refs/remotes/fluentgit/*")
                                            .Remove("+refs/heads/somebranch:refs/remotes/somebranch"))

                                .UpdateRemoteIfExists(r => r.Url == "non existing url",
                                    u =>
                                        // as the remote doesn't exist in this case, none of the below updates will execute.
                                        u.ChangeUrlTo("http://someurl.com")
                                         .ChangeRefSpecs()
                                            .Add("+whatever/*:whatever/*")
                                            .Remove("+blah/*:blah/*"));

            //Assert

            using (var repository = new Repository(GitRepoPath))
            {
                // remote url should have been changed.
                var updatedRemote = repository.Network.Remotes.Single(a => a.Name == "fluentgit" && a.Url == "https://github.com/libgit2/libgit2sharp.git");

                // refspec should be added:     "+refs/heads/master:refs/remotes/fluentgit/master"
                updatedRemote.FetchRefSpecs.Single(a => a.Specification.ToLower() == "+refs/heads/master:refs/remotes/fluentgit/master");

                // refspec should not exist twice:     "+refs/heads/*:refs/remotes/fluentgit/*"
                updatedRemote.FetchRefSpecs.Single(a => a.Specification.ToLower() == "+refs/heads/*:refs/remotes/fluentgit/*");

                // refspec should not be present: "+refs/heads/somebranch:refs/remotes/somebranch"
                Assert.IsFalse(updatedRemote.FetchRefSpecs.Any(a => a.Specification.ToLower() == "+refs/heads/somebranch:refs/remotes/somebranch"));

                // remote should not exist.
                Assert.IsFalse(repository.Network.Remotes.Any(a => a.Url == "non existing url"));

            }
        }


        public override void TearDown()
        {
            base.TearDown();
            if (!string.IsNullOrEmpty(_CloneDir))
            {
                var cloneDir = new DirectoryInfo(_CloneDir);
                if (cloneDir.Exists)
                {
                    TestUtils.DeleteDirectory(cloneDir);
                }
            }


        }

    }
}
