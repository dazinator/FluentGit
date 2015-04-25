using FluentGit.Logging;
using LibGit2Sharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{

    internal class CloneArgs
    {
        internal string CloneSourceUrl { get; set; }
        internal string CloneSourceDirectory { get; set; }
        internal string CloneDestinationDirectory { get; set; }
        internal string Username { get; set; }
        internal string Password { get; set; }
        internal bool Bare { get; set; }

        internal bool HasCredentials()
        {
            return Username != null || Password != null;
        }

        internal string GetSourcePath()
        {
            return CloneSourceUrl ?? CloneSourceDirectory;
        }

    }

    public class FluentRepo : IRepoFactoryBuilder,
                        IRepoInstanceBuilder,
                        ICloneSourceOptionsBuilder,
                        ICloneFromUrlOptions,
                        ICloneFromLocalDirectoryOptions,
                        ICloneAdditionalOptionsBuilder
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private IRepository _repository;
        private CloneOptions _clonedUsingOptions;

        private CloneArgs _cloneSetupArgs;

        private FluentEnumerable<IBranchBuilder> _branches;
        private RemotesBuilder _remotes;


        public IRepoInstanceBuilder Load(string gitFolderPath)
        {
            var repoPath = Repository.Discover(gitFolderPath);
            if (string.IsNullOrEmpty(repoPath))
            {
                throw new ArgumentException("invalid repository path");
            }
            InitialiseRepo(repoPath);
            return this;
        }

        public ICloneSourceOptionsBuilder Clone()
        {
            //  _cloneOptions = new CloneOptions();
            _cloneSetupArgs = new CloneArgs();

            return this;
        }

        //// CONVERSION OPERATOR
        //public static implicit operator IRepository(Repo repo)
        //{
        //    return repo._repository;
        //}

        ICloneFromUrlOptions ICloneSourceOptionsBuilder.FromUrl(string url)
        {
            _cloneSetupArgs.CloneSourceUrl = url;
            return this;
        }

        ICloneFromLocalDirectoryOptions ICloneSourceOptionsBuilder.FromDirectory(string directory)
        {
            _cloneSetupArgs.CloneSourceDirectory = directory;
            return this;
        }

        ICloneAdditionalOptionsBuilder ICloneFromUrlOptions.ToDirectory(string directory)
        {
            _cloneSetupArgs.CloneDestinationDirectory = directory;
            return this;
        }

        ICloneAdditionalOptionsBuilder ICloneFromLocalDirectoryOptions.ToDirectory(string directory)
        {
            _cloneSetupArgs.CloneDestinationDirectory = directory;
            return this;
        }

        ICloneAdditionalOptionsBuilder ICloneAdditionalOptionsBuilder.WithCredentials(string userName, string password)
        {
            _cloneSetupArgs.Username = userName;
            _cloneSetupArgs.Password = password;
            return this;
        }

        ICloneAdditionalOptionsBuilder ICloneAdditionalOptionsBuilder.Bare()
        {
            _cloneSetupArgs.Bare = true;
            return this;
        }

        IRepoInstanceBuilder ICloneAdditionalOptionsBuilder.Obtain()
        {
            Log.InfoFormat("Obtaining a clone of git repository from source: '{0}'", _cloneSetupArgs.GetSourcePath());

            var cloneOptions = new CloneOptions();
            cloneOptions.IsBare = _cloneSetupArgs.Bare;
            cloneOptions.Checkout = false;

            if (_cloneSetupArgs.HasCredentials())
            {
                UsernamePasswordCredentials creds = new UsernamePasswordCredentials();
                creds.Username = _cloneSetupArgs.Username;
                creds.Password = _cloneSetupArgs.Password;
                cloneOptions.CredentialsProvider = (url, usernameFromUrl, types) => creds;
            }

            try
            {
                var repoPath = Repository.Clone(_cloneSetupArgs.GetSourcePath(), _cloneSetupArgs.CloneDestinationDirectory, cloneOptions);
                this._clonedUsingOptions = cloneOptions;
                InitialiseRepo(repoPath);

            }
            catch (Exception)
            {
                throw;
            }

            return this;

        }

        private void InitialiseRepo(string repoPath)
        {
            var repo = new Repository(repoPath);
            if (this._repository != null)
            {
                this._repository.Dispose();
            }
            this._repository = repo;
            InitialiseEnumerables();

            // custom enumerator, that wraps the libgitsharp one, but casts each item our own builder type - IRemoteInstanceBuilder
            return;
        }

        private void InitialiseEnumerables()
        {
            // We will wrap the underlying libgitsharp enumerators, with our own that implicitly cast types.
            var remotesEnumerator = this._repository.Network.Remotes.GetEnumerator();
            var castingEnumerator = new TransformItemEnumerator<Remote, IRemoteInfo>(remotesEnumerator, f => RemoteInfo.FromRemote(f));
            this._remotes = new RemotesBuilder(this, castingEnumerator);

            var branchesEnumerator = this._repository.Branches.GetEnumerator();
            var castingBranchesEnumerator = new TransformItemEnumerator<Branch, IBranchBuilder>(branchesEnumerator, f => BranchBuilder.FromBranch(f, this));
            this._branches = new FluentEnumerable<IBranchBuilder>(castingBranchesEnumerator);
        }

        IFluentEnumerable<IBranchBuilder> IRepoInstanceBuilder.Branches
        {
            get { return this._branches; }
        }

        IRemotesBuilder IRepoInstanceBuilder.Remotes
        {
            get { return this._remotes; }
        }

        IRepoInstanceBuilder IRepoInstanceBuilder.AddRemote(Action<INewRemoteBuilder> remote)
        {
            var newRemoteBuilder = new NewRemoteBuilder();
            remote(newRemoteBuilder);
            this.AddRemote(newRemoteBuilder);
            return this;
        }

        IRepoInstanceBuilder IRepoInstanceBuilder.AddRemoteIfDoesNotExist(Action<INewRemoteBuilder> remote)
        {
            var newRemoteBuilder = new NewRemoteBuilder();
            remote(newRemoteBuilder);
            var existingRemote = this._remotes.FirstOrDefault(r => r.Name.ToLowerInvariant() == newRemoteBuilder.Name.ToLower());
            if (existingRemote != null)
            {
                this.AddRemote(newRemoteBuilder);
            }
            return this;
        }

        internal void AddRemote(NewRemoteBuilder newRemoteBuilder)
        {
            string remoteName = newRemoteBuilder.Name;
            string url = newRemoteBuilder.Url;
            if (newRemoteBuilder.FetchRefSpec != null)
            {
                this._repository.Network.Remotes.Add(remoteName, url, newRemoteBuilder.FetchRefSpec.ToString());
            }
            else
            {
                this._repository.Network.Remotes.Add(remoteName, url);
            }

        }

        //private Remote EnsureSingleRemoteIsDefined()
        //{
        //    var remotes = Repository.Network.Remotes;
        //    var howMany = remotes.Count();

        //    if (howMany == 1)
        //    {
        //        var remote = remotes.Single();

        //        Log.InfoFormat("One remote found ({0} -> '{1}').", remote.Name, remote.Url);
        //        return remote;
        //    }

        //    var message = string.Format("{0} remote(s) have been detected. When being run on a TeamCity agent, the Git repository is expected to bear one (and no more than one) remote.", howMany);
        //    throw new InvalidOperationException(message);
        //}

        //private void NormalizeGitDirectory()
        //{
        //    var remote = EnsureSingleRemoteIsDefined();
        //    EnsureRepoHasRefSpecs(remote);

        //    Log.InfoFormat("Fetching from remote '{0}' using the following refspecs: {1}.", remote.Name, string.Join(", ", remote.FetchRefSpecs.Select(r => r.Specification)));

        //    var fetchOptions = new FetchOptions();
        //    fetchOptions.CredentialsProvider = (url, user, types) => _credentials;
        //    Repository.Network.Fetch(remote, fetchOptions);

        //    CreateMissingLocalBranchesFromRemoteTrackingOnes(remote.Name);
        //    var headSha = Repository.Refs.Head.TargetIdentifier;

        //    if (!Repository.Info.IsHeadDetached)
        //    {
        //        Log.InfoFormat("HEAD points at branch '{0}'.", headSha);
        //        return;
        //    }

        //    Log.InfoFormat("HEAD is detached and points at commit '{0}'.", headSha);

        //    // In order to decide whether a fake branch is required or not, first check to see if any local branches have the same commit SHA of the head SHA.
        //    // If they do, go ahead and checkout that branch
        //    // If no, go ahead and check out a new branch, using the known commit SHA as the pointer
        //    var localBranchesWhereCommitShaIsHead = Repository.Branches.Where(b => !b.IsRemote && b.Tip.Sha == headSha).ToList();

        //    if (localBranchesWhereCommitShaIsHead.Count > 1)
        //    {
        //        var names = string.Join(", ", localBranchesWhereCommitShaIsHead.Select(r => r.CanonicalName));
        //        var message = string.Format("Found more than one local branch pointing at the commit '{0}'. Unable to determine which one to use ({1}).", headSha, names);
        //        throw new InvalidOperationException(message);
        //    }

        //    if (localBranchesWhereCommitShaIsHead.Count == 0)
        //    {
        //        Log.InfoFormat("No local branch pointing at the commit '{0}'. Fake branch needs to be created.", headSha);
        //        CreateFakeBranchPointingAtThePullRequestTip();
        //    }
        //    else
        //    {
        //        Log.InfoFormat("Checking out local branch 'refs/heads/{0}'.", localBranchesWhereCommitShaIsHead[0].Name);
        //        var branch = Repository.Branches[localBranchesWhereCommitShaIsHead[0].Name];
        //        Repository.Checkout(branch);
        //    }
        //}

        //private void EnsureRepoHasRefSpecs(Remote remote)
        //{
        //    if (remote.FetchRefSpecs.Any(r => r.Source == "refs/heads/*"))
        //        return;

        //    var allBranchesFetchRefSpec = string.Format("+refs/heads/*:refs/remotes/{0}/*", remote.Name);
        //    Log.InfoFormat("Adding refspec: {0}", allBranchesFetchRefSpec);
        //    Repository.Network.Remotes.Update(remote, r => r.FetchRefSpecs.Add(allBranchesFetchRefSpec));
        //}                          

        IRepoInstanceBuilder IRepoInstanceBuilder.UpdateRemote(Func<IRemoteInfo, bool> selector, Action<IUpdateRemoteBuilder> updater)
        {
            var existingRemote = this._remotes.FirstOrDefault(selector);
            if (existingRemote != null)
            {
                var remote = ((RemoteInfo)existingRemote).Remote;
                var newRemoteBuilder = new UpdateRemoteBuilder(this, remote);
                updater(newRemoteBuilder);
                var updaters = newRemoteBuilder.GetRemoteUpdaters();
                UpdateRemote(remote, updaters);
            }
            else
            {
                throw new InvalidOperationException("Could not update remote, as no remote was found with the criteria specified.");
            }
            return this;
        }

        IRepoInstanceBuilder IRepoInstanceBuilder.UpdateRemoteIfExists(Func<IRemoteInfo, bool> selector, Action<IUpdateRemoteBuilder> updater)
        {
            var existingRemote = this._remotes.FirstOrDefault(selector);
            if (existingRemote != null)
            {
                var remote = ((RemoteInfo)existingRemote).Remote;
                var newRemoteBuilder = new UpdateRemoteBuilder(this, remote);
                updater(newRemoteBuilder);
                var updaters = newRemoteBuilder.GetRemoteUpdaters();
                UpdateRemote(remote, updaters);
            }
            return this;
        }

        IRepoInstanceBuilder IRepoInstanceBuilder.UpdateRemotes(Func<IRemoteInfo, bool> filter, Action<IUpdateRemoteBuilder> updater)
        {
            var existingRemotes = this._remotes.Where(filter);
            foreach (var existingRemote in existingRemotes)
            {
                var remote = ((RemoteInfo)existingRemote).Remote;
                var newRemoteBuilder = new UpdateRemoteBuilder(this, remote);
                updater(newRemoteBuilder);
                var updaters = newRemoteBuilder.GetRemoteUpdaters();
                UpdateRemote(remote, updaters);
            }
            return this;
        }

        internal Remote UpdateRemote(Remote remote, IList<Action<RemoteUpdater>> updaters)
        {
            return this._repository.Network.Remotes.Update(remote, updaters.ToArray());
        }

    }

    public interface IRepoFactoryBuilder
    {
        IRepoInstanceBuilder Load(string gitFolderPath);
        ICloneSourceOptionsBuilder Clone();
    }

    public interface ICloneSourceOptionsBuilder
    {
        ICloneFromUrlOptions FromUrl(string url);
        ICloneFromLocalDirectoryOptions FromDirectory(string directory);
    }

    public interface ICloneFromUrlOptions
    {
        ICloneAdditionalOptionsBuilder ToDirectory(string directory);
    }

    public interface ICloneFromLocalDirectoryOptions
    {
        ICloneAdditionalOptionsBuilder ToDirectory(string directory);
    }

    public interface ICloneAdditionalOptionsBuilder
    {
        ICloneAdditionalOptionsBuilder WithCredentials(string userName, string password);
        ICloneAdditionalOptionsBuilder Bare();
        IRepoInstanceBuilder Obtain();
    }

    public interface IRepoInstanceBuilder
    {
        IFluentEnumerable<IBranchBuilder> Branches { get; }
        IRemotesBuilder Remotes { get; }
        IRepoInstanceBuilder AddRemote(Action<INewRemoteBuilder> adder);
        IRepoInstanceBuilder AddRemoteIfDoesNotExist(Action<INewRemoteBuilder> adder);

        IRepoInstanceBuilder UpdateRemoteIfExists(Func<IRemoteInfo, bool> selector, Action<IUpdateRemoteBuilder> updater);
        IRepoInstanceBuilder UpdateRemote(Func<IRemoteInfo, bool> selector, Action<IUpdateRemoteBuilder> updater);
        IRepoInstanceBuilder UpdateRemotes(Func<IRemoteInfo, bool> filter, Action<IUpdateRemoteBuilder> updater);

    }

    public interface IRemotesBuilder : IFluentEnumerable<IRemoteInfo>
    {
        //IRemotesBuilder Add(Action<INewRemoteBuilder> remote);
    }

    public interface IRemoteInfo
    {
        string Url { get; }
        string Name { get; }
    }

    public interface IBranchBuilder
    {
        string Name { get; }

    }


}
