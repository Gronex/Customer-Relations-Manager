using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.Company;
using customer_relations_manager.ViewModels.Opportunity;
using customer_relations_manager.ViewModels.User;
using Core.DomainModels.Customers;
using Core.DomainModels.Opportunity;
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

                
                cfg.CreateMap<UserGroup, GroupViewModel>().ReverseMap();
                cfg.CreateMap<ProductionGoal, GoalViewModel>().ReverseMap();
                cfg.CreateMap<Company, CompanyViewModel>().ReverseMap();
                cfg.CreateMap<Company, CompanyOverviewViewModel>().ReverseMap();
                cfg.CreateMap<Opportunity, OpportunityViewModel>()
                    .ForMember(vm => vm.Groups, c => c.MapFrom(o => o.UserGroups.Select(ug => ug.UserGroup)))
                    .ReverseMap()
                    .ForMember(vm => vm.StartDate, c => c.MapFrom(o => o.StartDate.Value.Date))
                    .ForMember(vm => vm.EndDate, c => c.MapFrom(o => o.EndDate.Value.Date))
                    .ForMember(vm => vm.ExpectedClose, c => c.MapFrom(o => o.ExpectedClose.Value.Date));

                cfg.CreateMap<Opportunity, OpportunityOverviewViewMode>().ReverseMap();
                cfg.CreateMap<Stage, StageViewModel>().ReverseMap();
                cfg.CreateMap<OpportunityCategory, CategoryViewModel>().ReverseMap();
                cfg.CreateMap<Department, GroupViewModel>().ReverseMap();

                cfg.CreateMap<Person, PersonViewModel>().ReverseMap();
            });

            return config;
        }
    }
}
