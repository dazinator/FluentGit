using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{

    public class UpdateRemoteBuilder : IUpdateRemoteBuilder
    {
        private Remote _remote;
        private FluentRepo _repo;
        private UpdateRefSpecsBuilder _refSpecsBuilder;

        private IList<Action<RemoteUpdater>> _queuedUpdates;

        internal UpdateRemoteBuilder(FluentRepo repo, Remote remote)
        {
            _remote = remote;
            _repo = repo;
            _queuedUpdates= new List<Action<RemoteUpdater>>();
            _refSpecsBuilder = new UpdateRefSpecsBuilder(remote.FetchRefSpecs, _queuedUpdates);
        }

        IUpdateRemoteBuilder IUpdateRemoteBuilder.ChangeUrlTo(string url)
        {
            _queuedUpdates.Add(a => a.Url = url);
            return this;
        }

        IUpdateRemoteFetchRefSpecs IUpdateRemoteBuilder.ChangeRefSpecs()
        {
            return new UpdateRefSpecsBuilder(_remote.FetchRefSpecs, _queuedUpdates);
        }

        public IList<Action<RemoteUpdater>> GetRemoteUpdaters()
        {
            return _queuedUpdates;
        }

    }

    public class UpdateRefSpecsBuilder : IUpdateRemoteFetchRefSpecs
    {

        private IList<RefSpec> _existingRefSpecs;
        private IList<Action<RemoteUpdater>> _updaters;      


        public IList<Action<RemoteUpdater>> GetRefSpecUpdaters()
        {
            return _updaters;
        }

        public UpdateRefSpecsBuilder(IEnumerable<RefSpec> existingRefSpecs, IList<Action<RemoteUpdater>> updaters)
        {
            _existingRefSpecs = existingRefSpecs.ToList();
            _updaters = updaters;
        }

        IUpdateRemoteFetchRefSpecs IUpdateRemoteFetchRefSpecs.Add(Action<IRefSpecBuilder> buildRefSpec)
        {
            var refSpecBuilder = new RefSpecBuilder();
            buildRefSpec(refSpecBuilder);
            var refSpec = ((IRefSpecBuilder)refSpecBuilder).ToRefSpec();
            ((IUpdateRemoteFetchRefSpecs)this).Add(refSpec.ToString());
            return this;
        }

        IUpdateRemoteFetchRefSpecs IUpdateRemoteFetchRefSpecs.Add(string fetchRefSpec)
        {
            _updaters.Add(add => add.FetchRefSpecs.Add(fetchRefSpec));
            return this;
        }

        IUpdateRemoteFetchRefSpecs IUpdateRemoteFetchRefSpecs.AddIfNotExists(Action<IRefSpecBuilder> buildRefSpec)
        {
            var refSpecBuilder = new RefSpecBuilder();
            buildRefSpec(refSpecBuilder);
            var refSpec = ((IRefSpecBuilder)refSpecBuilder).ToRefSpec();
            ((IUpdateRemoteFetchRefSpecs)this).AddIfNotExists(refSpec.ToString());
            return this;
        }

        IUpdateRemoteFetchRefSpecs IUpdateRemoteFetchRefSpecs.AddIfNotExists(string fetchRefSpec)
        {
            var exists = _existingRefSpecs.Any(r => r.Specification.ToLowerInvariant() == fetchRefSpec.ToLowerInvariant());
            if (!exists)
            {
                _updaters.Add(add => add.FetchRefSpecs.Add(fetchRefSpec));
            }
            return this;
        }

        IUpdateRemoteFetchRefSpecs IUpdateRemoteFetchRefSpecs.RemoveAll()
        {
            _updaters.Add(add => add.FetchRefSpecs.Clear());
            return this;
        }

        IUpdateRemoteFetchRefSpecs IUpdateRemoteFetchRefSpecs.Remove(Action<IRefSpecBuilder> buildRefSpec)
        {
            var refSpecBuilder = new RefSpecBuilder();
            buildRefSpec(refSpecBuilder);
            var refSpec = ((IRefSpecBuilder)refSpecBuilder).ToRefSpec().ToString();
            ((IUpdateRemoteFetchRefSpecs)this).Remove(refSpec);
            return this;
        }

        IUpdateRemoteFetchRefSpecs IUpdateRemoteFetchRefSpecs.Remove(string refSpec)
        {
            _updaters.Add(remove => remove.FetchRefSpecs.Remove(refSpec));
            return this;
        }

    }
}
