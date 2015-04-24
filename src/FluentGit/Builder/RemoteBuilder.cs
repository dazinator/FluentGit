using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{   

    public class RemoteBuilder : IRemoteBuilder
    {
        private Remote _remote;
        private FluentRepo _repo;

        private RemoteBuilder(Remote remote, FluentRepo repo)
        {
            _remote = remote;
            _repo = repo;
        }

        internal static RemoteBuilder FromRemote(Remote remote, FluentRepo repo)
        {
            var remoteInfo = new RemoteBuilder(remote, repo);
            return remoteInfo;
        }

        string IRemoteBuilder.Name
        {
            get { return this._remote.Name; }
        }

        string IRemoteBuilder.Url
        {
            get { return this._remote.Url; }
        }

    }
}
