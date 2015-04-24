using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{
    public interface IRefSpecBuilder
    {
        IRefSpecBuilder Source(string source);
        IRefSpecBuilder Destination(string destination);
        IRefSpecBuilder ForceUpdateIfFastForwardNotPossible();
        IRefSpecBuilder ForceUpdateIfFastForwardNotPossible(bool forceUpdate);

        RefSpecInfo ToRefSpec();

    }
}
