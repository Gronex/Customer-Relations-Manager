using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainModels.Comments;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;
using Infrastructure.DataAccess.Repositories;
using NSubstitute;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Repositories
{
    public class ActivityCommentRepositoryTest
    {
        private readonly IActivityCommentRepository _repo;
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<ActivityComment> _generic;

        public ActivityCommentRepositoryTest()
        {
            _context = new AppContextStub();
            _generic = Substitute.For<IGenericRepository<ActivityComment>>();

            _repo = new ActivityCommentRepository(_context, _generic);

            _context.Users.Add(new User {UserName = "1"});
            _context.Activities.Add(new Activity {Id = 1});
        }

        [Fact]
        public void CreateInserts()
        {
            _repo.Create(1, "1", "test");
            _generic.ReceivedWithAnyArgs().Insert(Arg.Any<ActivityComment>());
        }

        [Theory]
        [InlineData(1, "2")]
        [InlineData(2, "1")]
        [InlineData(2, "2")]
        public void CreateFailsOnBadArgs(int activityId, string userName)
        {
            Assert.Throws<NotFoundException>(() => _repo.Create(activityId, userName, "test"));
        }

        [Fact]
        public void GetAllGetsAll()
        {
            _context.ActivityComments.Add(new ActivityComment {Text = "test2", ActivityId = 1});
            _generic.Get().ReturnsForAnyArgs(_context.ActivityComments);
            var result = _repo.GetAll(1);
            Assert.Equal(1, result.Count());
        }
    }
}
