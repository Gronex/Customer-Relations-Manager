using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class Sanity
    {
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(42, 42);
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(42, 40);
        }
    }
}
