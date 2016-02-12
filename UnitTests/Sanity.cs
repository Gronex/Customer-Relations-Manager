using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace UnitTests
{
    public class Sanity
    {
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(42, 42);
        }

        [Fact(Skip = "Only to make sure tests can fail")]
        public void FailingTest()
        {
            Assert.Equal(42, 40);
        }
    }
}
