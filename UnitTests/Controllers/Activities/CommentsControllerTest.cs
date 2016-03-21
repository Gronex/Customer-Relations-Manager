using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers.Activities;
using Core.DomainModels.Comments;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using NSubstitute;
using Xunit;

namespace UnitTests.Controllers
{
    public class CommentsControllerTest
    {
        private readonly CommentsController _ctrl;
        private readonly IActivityCommentRepository _repo;
        private readonly IUnitOfWork _uow;

        public CommentsControllerTest()
        {
            var mapper = AutomapperConfig.ConfigMappings().CreateMapper();
            _repo = Substitute.For<IActivityCommentRepository>();
            _uow = Substitute.For<IUnitOfWork>();

            _ctrl = new CommentsController(_repo, _uow, mapper);
        }

        [Fact]
        public void GetAllReturnsAll()
        {
            var data = new List<ActivityComment>
            {
                new ActivityComment {Text = "1"},
                new ActivityComment {Text = "2"}
            };

            _repo.GetAll(1, Arg.Any<Func<IQueryable<ActivityComment>, IOrderedQueryable<ActivityComment>>>())
                .ReturnsForAnyArgs(new PaginationEnvelope<ActivityComment> {Data = data});

            var result = _ctrl.GetAll(1);

            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public void PostSaves()
        {
            _repo.Create(1, "", "test").Returns(new ActivityComment {Id = 1, Text = "test"});
            _ctrl.Post(1, "test");
            _uow.ReceivedWithAnyArgs().Save();
        }

        [Fact]
        public void PostFailsOnNoComment()
        {
            var result = _ctrl.Post(1, "");
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void PostFailsOnBadId()
        {
            var result = _ctrl.Post(1, "test");
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
