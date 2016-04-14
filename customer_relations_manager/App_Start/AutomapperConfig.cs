using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.Activity;
using customer_relations_manager.ViewModels.Company;
using customer_relations_manager.ViewModels.GraphFilter;
using customer_relations_manager.ViewModels.GraphFilter.ActivityGraph;
using customer_relations_manager.ViewModels.GraphFilter.ProductionGraph;
using customer_relations_manager.ViewModels.Opportunity;
using customer_relations_manager.ViewModels.User;
using Core.DomainModels.Activities;
using Core.DomainModels.Comments;
using Core.DomainModels.Customers;
using Core.DomainModels.Opportunity;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainModels.ViewSettings;

namespace customer_relations_manager.App_Start
{
    public class AutomapperConfig
    {
        public static MapperConfiguration ConfigMappings()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DateTime, TimeSpan>().ReverseMap();

                cfg.CreateMap<User, UserViewModel>()
                    .ForMember(vm => vm.Groups, o => o.MapFrom(u => u.Groups.Select(g => g.UserGroup)))
                    .ReverseMap()
                    .ForMember(u => u.UserName, o => o.MapFrom(vm => vm.Email));
                cfg.CreateMap<User, UserOverviewViewModel>().ReverseMap()
                    .ForMember(u => u.UserName, o => o.MapFrom(vm => vm.Email));

                
                cfg.CreateMap<UserGroup, GroupViewModel>().ReverseMap();
                cfg.CreateMap<ProductionGoal, GoalViewModel>()
                    .ForMember(vm => vm.Year, o => o.MapFrom(pg => pg.StartDate.Year))
                    .ForMember(vm => vm.Month, o => o.MapFrom(pg => pg.StartDate.Month))
                    .ReverseMap()
                    .ForMember(pg => pg.StartDate, o => o.MapFrom(vm => new DateTime(vm.Year, vm.Month, 1)));
                cfg.CreateMap<Company, CompanyViewModel>().ReverseMap();
                cfg.CreateMap<Company, CompanyOverviewViewModel>().ReverseMap();
                cfg.CreateMap<Opportunity, OpportunityViewModel>()
                    .ForMember(vm => vm.Groups, c => c.MapFrom(o => o.UserGroups.Select(ug => ug.UserGroup)))
                    .ReverseMap()
                    .ForMember(vm => vm.StartDate, c => c.MapFrom(o => o.StartDate != null ? o.StartDate.Value.Date : (DateTime?) null))
                    .ForMember(vm => vm.EndDate, c => c.MapFrom(o => o.EndDate != null ? o.EndDate.Value.Date : (DateTime?) null))
                    .ForMember(vm => vm.ExpectedClose, c => c.MapFrom(o => o.ExpectedClose != null ? o.ExpectedClose.Value.Date : (DateTime?) null));

                cfg.CreateMap<Opportunity, OpportunityOverviewViewMode>().ReverseMap();
                cfg.CreateMap<Stage, StageViewModel>().ReverseMap();
                cfg.CreateMap<OpportunityCategory, CategoryViewModel>().ReverseMap();
                cfg.CreateMap<Department, GroupViewModel>().ReverseMap();

                cfg.CreateMap<Person, PersonViewModel>()
                    .ForMember(vm => vm.CompanyName, c => c.MapFrom(p => p.Company.Name))    
                .ReverseMap();

                cfg.CreateMap<ActivityCategory, ActivityCategoryViewModel>().ReverseMap();
                cfg.CreateMap<Activity, ActivityViewModel>()
                    .AfterMap((a, vm) =>
                    {
                        vm.CategoryName = a.Category?.Name;
                        vm.ResponsibleEmail = a.PrimaryResponsible?.Email;
                        vm.ResponsibleName = a.PrimaryResponsible?.Name;
                    })
                    .ReverseMap()
                    .AfterMap((vm, a) =>
                    {
                        a.Category = new ActivityCategory { Name = vm.CategoryName };
                        a.PrimaryResponsible = new User { Email = vm.ResponsibleEmail };
                    });
                cfg.CreateMap<Activity, ActivityOverviewViewModel>().ReverseMap();


                cfg.CreateMap<ActivityComment, CommentViewModel>().ReverseMap();
                cfg.CreateMap<OpportunityComment, CommentViewModel>().ReverseMap();
                cfg.CreateMap<Person, PersonOverviewViewModel>().ReverseMap();

                cfg.CreateMap<GraphFilterOverviewViewModel, ProductionViewSettings>().ReverseMap();
                cfg.CreateMap<GraphFilterOverviewViewModel, ActivityViewSettings>().ReverseMap();
                cfg.CreateMap<ProductionGraphFilterViewModel, ProductionViewSettings>().ReverseMap();
                cfg.CreateMap<ActivityGraphFilterViewModel, ActivityViewSettings>().ReverseMap();
            });

            return config;
        }
    }
}
