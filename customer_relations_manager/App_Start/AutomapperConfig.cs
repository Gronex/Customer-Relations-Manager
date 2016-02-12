using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.User;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;

namespace customer_relations_manager.App_Start
{
    public class AutomapperConfig
    {
        public static MapperConfiguration ConfigMappings()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserViewModel>()
                    .ForMember(vm => vm.Groups, o => o.MapFrom(u => u.Groups.Select(g => g.UserGroup)))
                    .ReverseMap()
                    .ForMember(u => u.UserName, o => o.MapFrom(vm => vm.Email));
                cfg.CreateMap<User, UserOverviewViewModel>().ReverseMap()
                    .ForMember(u => u.UserName, o => o.MapFrom(vm => vm.Email));

                cfg.CreateMap<UserGroup, UserGroupViewModel>().ReverseMap();
            });

            return config;
        }
    }
}
