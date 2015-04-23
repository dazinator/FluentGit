using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit.Tests
{

    [TestFixture]
    public class RefSpecBuilderTests
    {

        [Test]
        public void Can_Create_RefSpecBuilder()
        {
            var builder = new RefSpecBuilder();
        }

        [Test]
        public void Can_Create_RefSpecBuilder_With_Arguments()
        {
            var builder = new RefSpecBuilder("refs/heads/*", "refs/remotes/origin/*", true);
        }

        [Test]
        public void Can_Create_RefSpecBuilder_With_RefSpec_Arguments()
        {
            var refSpec = new RefSpecInfo()
            {
                Source = "refs/heads/*",
                Destination = "refs/remotes/origin/*",
                ForceUpdateWhenFastForwardNotAllowed = true
            };

            var builder = new RefSpecBuilder(refSpec);
        }

        [Test]
        public void Can_Build_RefSpec()
        {
            IRefSpecBuilder builder = new RefSpecBuilder();

            var spec = builder.Source("refs/heads/*")
                        .Destination("refs/remotes/origin/*")
                        .ForceUpdateIfFastForwardNotPossible()
                        .ToRefSpec();

            Assert.That(spec.Source, Is.EqualTo("refs/heads/*"));
            Assert.That(spec.Destination, Is.EqualTo("refs/remotes/origin/*"));
            Assert.That(spec.ForceUpdateWhenFastForwardNotAllowed);
        }     

    }

}
