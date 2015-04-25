using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{  

    public interface IUpdateRemoteBuilder
    {       
        IUpdateRemoteBuilder ChangeUrlTo(string url);
        IUpdateRemoteFetchRefSpecs ChangeRefSpecs();    
    }

    public interface IUpdateRemoteFetchRefSpecs
    {
        IUpdateRemoteFetchRefSpecs Add(Action<IRefSpecBuilder> refSpecBuilder);
        IUpdateRemoteFetchRefSpecs Add(string fetchRefSpec);

        IUpdateRemoteFetchRefSpecs AddIfNotExists(Action<IRefSpecBuilder> refSpecBuilder);
        IUpdateRemoteFetchRefSpecs AddIfNotExists(string fetchRefSpec);

        IUpdateRemoteFetchRefSpecs RemoveAll();
        IUpdateRemoteFetchRefSpecs Remove(Action<IRefSpecBuilder> refSpecBuilder);
        IUpdateRemoteFetchRefSpecs Remove(string refSpecBuilder);
       
    }






    // repo.UpdateRemote(a=> a.WithName("origin"))

}
