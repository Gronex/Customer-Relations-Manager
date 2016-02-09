using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AngularJSWebApiEmpty.Controllers;
using Core.DomainModels.Activities;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Controllers
{
    public class ActivityControllerTest
    {
        private readonly ActivitiesController _controller;
        public ActivityControllerTest()
        {
            _controller = new ActivitiesController(new UnitOfWorkStub(), new ActivityRepoStub());
        }

        [Fact]
        public void GetAllActivities()
        {
            var activities = _controller.GetActivities();
            Assert.Equal(activities.Count(), 3);
        }
        
        [Fact]
        public async void GetSingleActivities()
        {
            var result = await _controller.GetActivity(1) as OkNegotiatedContentResult<Activity>;
            var activity = result?.Content;
            Assert.Equal(activity?.Name, "Do that one thing");
        }

        [Fact]
        public async void GetSingleActivitiesNotFound()
        {
            var result = await _controller.GetActivity(4);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
