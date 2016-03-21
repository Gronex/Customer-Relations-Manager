using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using customer_relations_manager.Controllers;
using Xunit;

namespace UnitTests.Controllers
{
    public class CrmApiControllerTest
    {
        private readonly Ctrl _ctrl;

        public CrmApiControllerTest()
        {
            _ctrl = new Ctrl();
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(0, 0)]
        [InlineData(1, 10)]
        [InlineData(-100, 10)]
        [InlineData(1, -100)]
        [InlineData(-100, -100)]
        public void CorrectPageInfoUpdatesPage(int? page, int? pageSize)
        {
            _ctrl.CorrectPageInfoCall(ref page, ref pageSize);
            Assert.Equal(new {Page = 1, PageSize = 10}, new { Page = page.Value, PageSize = pageSize.Value });
        }

        [Fact]
        public void CorrectPageInfoDoesNothingOnNull()
        {
            int? page = null;
            int? pageSize = null;

            _ctrl.CorrectPageInfoCall(ref page, ref pageSize);
            Assert.Equal(new int?[] {null, null}, new int?[]{page, pageSize });
        }

        private class Ctrl : CrmApiController
        {
            public void CorrectPageInfoCall(ref int? page, ref int? pageSize)
            {
                CorrectPageInfo(ref page, ref pageSize);
            }
            
        }
    }
}
