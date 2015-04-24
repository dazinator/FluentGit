using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{
    public class RemotesBuilder : FluentEnumerable<IRemoteBuilder>, IRemotesBuilder
    {
        private FluentRepo _repo;

        public RemotesBuilder(FluentRepo repo, IEnumerator<IRemoteBuilder> enumerator)
            : base(enumerator)
        {
            _repo = repo;
        }

       

        IEnumerator<IRemoteBuilder> IEnumerable<IRemoteBuilder>.GetEnumerator()
        {
            return base.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }
    }
}
