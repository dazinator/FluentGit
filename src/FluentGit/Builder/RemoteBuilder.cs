using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{

    public class RemoteInfo : IRemoteInfo
    {
        private Remote _remote;
      //  private FluentRepo _repo;

        private RemoteInfo(Remote remote)
        {
            _remote = remote;
            //_repo = repo;
        }

        internal static RemoteInfo FromRemote(Remote remote)
        {
            var remoteInfo = new RemoteInfo(remote);
            return remoteInfo;
        }

        string IRemoteInfo.Name
        {
            get { return this._remote.Name; }
        }

        string IRemoteInfo.Url
        {
            get { return this._remote.Url; }
        }

        internal Remote Remote { get; set; }
    }
}
