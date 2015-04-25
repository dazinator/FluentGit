using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{
    public class RemotesBuilder : FluentEnumerable<IRemoteInfo>, IRemotesBuilder
    {
        private FluentRepo _repo;

        public RemotesBuilder(FluentRepo repo, IEnumerator<IRemoteInfo> enumerator)
            : base(enumerator)
        {
            _repo = repo;
        }

       

        IEnumerator<IRemoteInfo> IEnumerable<IRemoteInfo>.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }
    }
}
