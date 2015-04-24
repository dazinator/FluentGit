using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{
    public interface INewRemoteBuilder
    {
        INewRemoteBuilder WithName(string name);
        INewRemoteBuilder WithUrl(string url);
        INewRemoteBuilder WithFetchRefSpec(Action<IRefSpecBuilder> refSpecBuilder);
        INewRemoteBuilder WithFetchRefSpec(string fetchRefSpec);
    }
}
