using FluentGit.Tests.Logging;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FluentGit.Tests
{
    public static class GitTestExtensions
    {
        static int pad = 1;
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static void DumpGraph(this IRepository repository)
        {
            var output = new StringBuilder();

            ProcessHelper.Run(
                o => output.AppendLine(o),
                e => output.AppendLine(string.Format("ERROR: {0}", e)),
                null,
                "git",
                @"log --graph --abbrev-commit --decorate --date=relative --all",
                repository.Info.Path);

            Trace.Write(output.ToString());
        }

        public static IRepository AddBranch(this IRepository repo, string branchName)
        {

            // Let's move the HEAD to this branch to be created
            var branchInfo = new GitBranchName(branchName);

            repo.Refs.UpdateTarget("HEAD", branchInfo.GetCanonicalBranchName());
            // Create a commit against HEAD
            var c = GenerateCommit(repo);
            var branch = repo.Branches[branchName];
            if (branch == null)
            {
                Log.InfoFormat("Branch was NULL!");
            }

            return repo;
        }

        public static Commit GenerateCommit(this IRepository repository, string comment = null)
        {
            var randomFile = Path.Combine(repository.Info.WorkingDirectory, Guid.NewGuid().ToString());
            File.WriteAllText(randomFile, string.Empty);
            comment = comment ?? "Test generated commit.";
            return CommitFile(repository, randomFile, comment);
        }

        public static Commit CommitFile(this IRepository repo, string filePath, string comment)
        {
            repo.Stage(filePath);
            return repo.Commit(comment, SignatureNow(), SignatureNow());
        }

        public static Commit MakeACommit(this IRepository repository)
        {
            return CreateFileAndCommit(repository, Guid.NewGuid().ToString());
        }

        public static void MergeNoFF(this IRepository repository, string branch)
        {
            MergeNoFF(repository, branch, SignatureNow());
        }

        public static void MergeNoFF(this IRepository repository, string branch, Signature sig)
        {
            // Fixes a race condition
            repository.Merge(repository.FindBranch(branch), sig, new MergeOptions
            {
                FastForwardStrategy = FastForwardStrategy.NoFastFoward
            });
        }

        public static Branch FindBranch(this IRepository repository, string branchName)
        {
            var exact = repository.Branches.FirstOrDefault(x => x.Name == branchName);
            if (exact != null)
            {
                return exact;
            }

            return repository.Branches.FirstOrDefault(x => x.Name == "origin/" + branchName);
        }

        public static Commit[] MakeCommits(this IRepository repository, int numCommitsToMake)
        {
            return Enumerable.Range(1, numCommitsToMake)
                .Select(x => repository.MakeACommit())
                .ToArray();
        }

        public static Commit CreateFileAndCommit(this IRepository repository, string relativeFileName)
        {
            var randomFile = Path.Combine(repository.Info.WorkingDirectory, relativeFileName);
            if (File.Exists(randomFile))
            {
                File.Delete(randomFile);
            }

            var totalWidth = 36 + (pad++ % 10);
            var contents = Guid.NewGuid().ToString().PadRight(totalWidth, '.');
            File.WriteAllText(randomFile, contents);

            repository.Stage(randomFile);

            return repository.Commit(string.Format("Test Commit for file '{0}'", relativeFileName),
                SignatureNow(), SignatureNow());
        }

        public static Tag MakeATaggedCommit(this IRepository repository, string tag)
        {
            var commit = repository.MakeACommit();
            var existingTag = repository.Tags.SingleOrDefault(t => t.Name == tag);
            if (existingTag != null)
                return existingTag;
            return repository.Tags.Add(tag, commit);
        }

        public static Branch CreatePullRequest(this IRepository repository, string from, string to, int prNumber = 2, bool isRemotePr = true)
        {
            repository.Checkout(to);
            repository.MergeNoFF(from);
            var branch = repository.CreateBranch("pull/" + prNumber + "/merge");
            repository.Checkout(branch);
            repository.Checkout(to);
            repository.Reset(ResetMode.Hard, "HEAD~1");
            var pullBranch = repository.Checkout("pull/" + prNumber + "/merge");
            if (isRemotePr)
            {
                // If we delete the branch, it is effectively the same as remote PR
                repository.Branches.Remove(from);
            }

            return pullBranch;
        }

        public static bool IsBareRepo(this IRepository repository)
        {
            if (repository != null)
            {
                // verify there is no seperate .git directory (bare clone)
                var directoryInfo = new DirectoryInfo(repository.Info.Path);
                if (directoryInfo.Name == ".git")
                {
                    return false;
                }
                var gitFolders = directoryInfo.GetDirectories("*.git", SearchOption.AllDirectories);
                return gitFolders == null || gitFolders.Count() == 0;
            }

            return false;

        }

        public static Signature SignatureNow()
        {
            var dateTimeOffset = DateTimeOffset.Now;
            return Signature(dateTimeOffset);
        }

        public static Signature Signature(DateTimeOffset dateTimeOffset)
        {
            return new Signature("Billy", "billy@thesundancekid.com", dateTimeOffset);
        }
    }
}
