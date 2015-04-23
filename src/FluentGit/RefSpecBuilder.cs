using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FluentGit
{
    public class RefSpecBuilder : IRefSpecBuilder
    {
        private RefSpecInfo _refSpecInfo;

        public RefSpecBuilder()
            : this("", "", true)
        {

        }

        public RefSpecBuilder(string source, string destination, bool forceUpdateIfFastForwardNotPossible = true)
            : this(new RefSpecInfo() { Source = source, Destination = destination, ForceUpdateWhenFastForwardNotAllowed = forceUpdateIfFastForwardNotPossible })
        {

        }

        public RefSpecBuilder(RefSpecInfo refSpecInfo)
        {
            _refSpecInfo = refSpecInfo;
        }

        RefSpecInfo IRefSpecBuilder.ToRefSpec()
        {
            return _refSpecInfo;
        }

        public override string ToString()
        {
            return _refSpecInfo.ToString();
        }

        IRefSpecBuilder IRefSpecBuilder.Source(string source)
        {
            _refSpecInfo.Source = source;
            return this;
        }

        IRefSpecBuilder IRefSpecBuilder.Destination(string destination)
        {
            _refSpecInfo.Destination = destination;
            return this;
        }

        IRefSpecBuilder IRefSpecBuilder.ForceUpdateIfFastForwardNotPossible()
        {
            _refSpecInfo.ForceUpdateWhenFastForwardNotAllowed = true;
            return this;
        }

        IRefSpecBuilder IRefSpecBuilder.ForceUpdateIfFastForwardNotPossible(bool forceUpdate)
        {
            _refSpecInfo.ForceUpdateWhenFastForwardNotAllowed = forceUpdate;
            return this;
        }
    }

    public interface IRefSpecBuilder
    {
        IRefSpecBuilder Source(string source);
        IRefSpecBuilder Destination(string destination);
        IRefSpecBuilder ForceUpdateIfFastForwardNotPossible();
        IRefSpecBuilder ForceUpdateIfFastForwardNotPossible(bool forceUpdate);

        RefSpecInfo ToRefSpec();

    }
}
