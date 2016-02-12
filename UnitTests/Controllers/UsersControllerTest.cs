using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoMapper;
using customer_relations_manager;
using customer_relations_manager.App_Start;
using customer_relations_manager.Controllers;
using Core.DomainModels.Users;
using Microsoft.AspNet.Identity;
using NSubstitute;
using UnitTests.Stubs;
using Xunit;

namespace UnitTests.Controllers
{
    public class UsersControllerTest
    {
        private readonly UsersController _controller;
        private readonly IEnumerable<UserRole> _allRoles;
        private readonly IMapper _mapper;

        public UsersControllerTest()
        {
            _mapper = AutomapperConfig.ConfigMappings().CreateMapper();
            _controller = new UsersController(null, new UnitOfWorkStub(), _mapper);
            
            _allRoles = new List<UserRole>()
            {
                UserRole.Super,
                UserRole.Executive,
                UserRole.Standard,
            };
        }

        [Fact]
        public void SuperGetsAllRoles()
        {
            var roles = _controller.CalculateNewRoles(UserRole.Super);

            Assert.Equal(_allRoles, roles);
        }

        [Theory]
        [InlineData(UserRole.Standard, 1)]
        [InlineData(UserRole.Executive, 2)]
        [InlineData(UserRole.Super, 3)]
        public void CorrectNumberOfRoles(UserRole target, int expectedRoleCount)
        {
            var roles = _controller.CalculateNewRoles(target);

            Assert.Equal(expectedRoleCount, roles.Count());
        }
    }
}
