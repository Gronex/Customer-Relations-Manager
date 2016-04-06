using AutoMapper;
using Core.ApplicationServices.Graph;
using Core.ApplicationServices.ServiceInterfaces;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess;
using Infrastructure.DataAccess.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(customer_relations_manager.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(customer_relations_manager.App_Start.NinjectWebCommon), "Stop")]

namespace customer_relations_manager.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var mapperConfig = AutomapperConfig.ConfigMappings();


            kernel.Bind<IApplicationContext>().To<ApplicationContext>().InRequestScope();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<IMapper>().ToConstant(mapperConfig.CreateMapper());

            // Identity
            kernel.Bind(typeof(IUserStore<>)).To(typeof(UserStore<>)).InRequestScope().WithConstructorArgument("context", kernel.Get<ApplicationContext>());
            kernel.Bind<IAuthenticationManager>().ToMethod(c => HttpContext.Current.GetOwinContext().Authentication).InRequestScope();
            kernel.Bind<ApplicationUserManager>().ToMethod(c => HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>());


            kernel.Bind(typeof (IGenericRepository<>)).To(typeof (GenericRepository<>));
            kernel.Bind<IActivityRepository>().To<ActivityRepository>();
            kernel.Bind<IGoalRepository>().To<GoalRepository>();
            kernel.Bind<IOpportunityRepository>().To<OpportunityRepository>();
            kernel.Bind<IGraphService>().To<GraphService>();
            kernel.Bind<IPersonRepository>().To<PersonRepository>();
            kernel.Bind<IActivityCommentRepository>().To<ActivityCommentRepository>();
            kernel.Bind<IProductionViewSettingsRepository>().To<ProductionViewSettingsRepository>();
            kernel.Bind<IActivityViewSettingsRepository>().To<ActivityViewSettingsRepository>();
        }        
    }
}
