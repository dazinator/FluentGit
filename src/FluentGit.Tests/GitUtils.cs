using FluentGit.Tests.Logging;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FluentGit.Tests
{
    public static class TestUtils
    {

        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static string GetUniqueTempFolder(string parentFolderName)
        {
            var currentDir = Path.GetTempPath();
            var repoDir = Path.Combine(currentDir, parentFolderName, Guid.NewGuid().ToString("N"));
            return repoDir;
        }

        public static void DeleteGitDirectory(string path)
        {
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };
            if (directory.Name != ".git")
            {
                // Does this folder contain the git directory?
                var gitFolderDirectory = directory.GetDirectories("*.git").FirstOrDefault();
                if (gitFolderDirectory != null)
                {

                    directory = gitFolderDirectory;
                }
                else
                {
                    throw new ArgumentException("Cannot delete a directory that isn't a git repository.");
                }
            }

            DeleteDirectory(directory);

        }

        public static void DeleteDirectory(DirectoryInfo directory)
        {
            // delete all files within the directory
            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }
            // delete the directory itself.
            directory.Delete(true);
        }

        public static IRepository CreateRepoWithBranch(string path, string branchName)
        {
            Repository.Init(path);
            Log.InfoFormat("Created git repository at '{0}'", path);

            var repo = new Repository(path);

            // Let's move the HEAD to this branch to be created
            var branchInfo = new GitBranchName(branchName);

            repo.Refs.UpdateTarget("HEAD", branchInfo.GetCanonicalBranchName());
            // Create a commit against HEAD
            var c = repo.GenerateCommit();
            var branch = repo.Branches[branchName];
            if (branch == null)
            {
                Log.InfoFormat("Branch was NULL!");
            }

            return repo;
        }

        public static IRepository CreateEmptyTestRepo(string path)
        {
            Repository.Init(path);
            Log.InfoFormat("Created git repository at '{0}'", path);
            return new Repository(path);
        }


    }
}
