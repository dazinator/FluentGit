using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{  

    public class NewRemoteBuilder : INewRemoteBuilder
    {      

        internal NewRemoteBuilder()
        {
           // _repo = repo;
        }

        INewRemoteBuilder INewRemoteBuilder.WithName(string name)
        {
            Name = name;
            return this;
        }

        INewRemoteBuilder INewRemoteBuilder.WithUrl(string url)
        {
            Url = url;
            return this;
        }

        INewRemoteBuilder INewRemoteBuilder.WithFetchRefSpec(Action<IRefSpecBuilder> refSpecBuilder)
        {
            var builder = (IRefSpecBuilder)new RefSpecBuilder("", "", false);
            refSpecBuilder(builder);
            this.FetchRefSpec = builder.ToRefSpec();
            return this;
        }

        INewRemoteBuilder INewRemoteBuilder.WithFetchRefSpec(string fetchRefSpec)
        {
            this.FetchRefSpec = RefSpecInfo.Parse(fetchRefSpec);
            return this;
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public RefSpecInfo FetchRefSpec { get; set; }

    }
}
