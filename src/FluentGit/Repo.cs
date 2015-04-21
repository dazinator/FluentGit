using FluentGit.Logging;
using LibGit2Sharp;
using System;
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

    public class Repo : IRepoFactoryBuilder,
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

        public IRepoInstanceBuilder Load(string gitFolderPath)
        {
            var repoPath = Repository.Discover(gitFolderPath);
            if (string.IsNullOrEmpty(repoPath))
            {
                throw new ArgumentException("invalid repository path");
            }
            _repository = new Repository(repoPath);
            return this;
        }

        public ICloneSourceOptionsBuilder Clone()
        {
            //  _cloneOptions = new CloneOptions();
            _cloneSetupArgs = new CloneArgs();

            return this;
        }

        //// CONVERSION OPERATOR
        //public static implicit operator Team(TeamBuilder tb)
        //{
        //    return new Team(
        //        tb.name,
        //        tb.nickName,
        //        tb.shirtColor,
        //        tb.homeTown,
        //        tb.ground);
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
                this._repository = new Repository(repoPath);
            }
            catch (Exception)
            {
                throw;
            }

            return this;

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


    }

    public interface IRepoBranchBuilder
    {


    }
}
