using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentGit.Tests
{

    [TestFixture]
    public class RefSpecInfoTests
    {

        [Test]
        public void Can_Create_RefSpecInfo()
        {
            var refSpecInfo = new RefSpecInfo();
        }

        [Test]
        [TestCase("refs/heads/*", "refs/remotes/origin/*", true)]
        [TestCase("refs/heads/*", "refs/remotes/origin/*", false)]
        [TestCase("refs/heads/master", "refs/remotes/origin/master", false)]
        public void Can_ToString_RefSpec(string source, string destination, bool forceUpdate)
        {
            var refSpecInfo = new RefSpecInfo();
            refSpecInfo.Destination = destination;
            refSpecInfo.Source = source;
            refSpecInfo.ForceUpdateWhenFastForwardNotAllowed = forceUpdate;

            string expect;
            if (forceUpdate)
            {
                expect = string.Format("+{0}:{1}", source, destination);               
            }
            else
            {
                expect = string.Format("{0}:{1}", source, destination);
            }

            Assert.AreEqual(refSpecInfo.ToString(), expect);
        }

        [Test]
        [TestCase("+refs/heads/*:refs/remotes/origin/*")]
        [TestCase("+refs/heads/master:refs/remotes/origin/master")]
        [TestCase("refs/heads/master:refs/remotes/origin/master")]
        public void Can_Parse_Valid_RefSpec(string refSpec)
        {
            var refSpecInfo = RefSpecInfo.Parse(refSpec);
            Assert.IsNotNull(refSpecInfo);
            Assert.IsNotNull(refSpecInfo.Source);
            Assert.IsNotNull(refSpecInfo.Destination);
            Assert.That(refSpecInfo.ToString(), Is.EqualTo(refSpec));
        }

        [Test]
        [TestCase("+refs/heads/*:refs/remotes/origin/:*", ExpectedException = typeof(FormatException))]
        [TestCase("+refs/heads/master", ExpectedException = typeof(FormatException))]
        [TestCase("refs/heads/master", ExpectedException = typeof(FormatException))]
        public void Cannot_Parse_Invalid_RefSpec(string refSpec)
        {
            var refSpecInfo = RefSpecInfo.Parse(refSpec);
        }
        
        [Test]
        [TestCase("+refs/heads/*:refs/remotes/origin/:*")]
        [TestCase("+refs/heads/master")]
        [TestCase("refs/heads/master")]
        public void Can_Try_Parse_Invalid_RefSpec(string refSpec)
        {
            RefSpecInfo refSpecInfo;
            var isSuccess = RefSpecInfo.TryParse(refSpec, out refSpecInfo);
            Assert.IsFalse(isSuccess);
            Assert.That(refSpecInfo, Is.Null);
        }      

    }

}
