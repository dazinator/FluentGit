using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit
{
    public class BranchBuilder : IBranchBuilder
    {
        private Branch _branch;
        private FluentRepo _repo;

        private BranchBuilder(Branch branch, FluentRepo repo)
        {
            _branch = branch;
            _repo = repo;
        }

        string IBranchBuilder.Name
        {
            get { return _branch.Name; }
        }

        internal static BranchBuilder FromBranch(Branch branch, FluentRepo repo)
        {
            var branchInfo = new BranchBuilder(branch, repo);
            return branchInfo;
        }

    }
}
