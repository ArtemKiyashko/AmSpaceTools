using AmSpaceClient;
using AmSpaceModels;
using AmSpaceModels.Idp;
using AmSpaceModels.Organization;
using AmSpaceModels.Sap;
using AmSpaceTools.Decorators;
using AmSpaceTools.Infrastructure.Providers;
using AmSpaceTools.ModelConverters;
using AmSpaceTools.ViewModels;
using AutoMapper;
using ExcelWorker;
using ExcelWorker.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;

namespace AmSpaceTools.Infrastructure
{
    public static class Services
    {
        private static IContainer _container;
        private static IConfigurationProvider _mapperConfiguration
        {
            get
            {
                return new MapperConfiguration(cfg => {
                    cfg.CreateMap<CompetencyAction, UpdateAction>().ConvertUsing(new ActionToUpdateConverter());
                    cfg.CreateMap<IDictionary<Competency, List<IdpAction>>, IEnumerable<IdpExcelRow>>().ConvertUsing(new CompetencyActionsToExcelRowConverter());
                    cfg.CreateMap<SapPersonExcelRow, ExternalAccount>().ConvertUsing(_container.Resolve<SapPersonExcelToAmspaceConverter>());
                });
            }
        }
        public static IContainer Container
        {
            get
            {
                return _container;
            }
        }
        static Services()
        {
            var builder = new ContainerBuilder();
            //assembly
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).InstancePerDependency();
            //singleton
            builder.RegisterType<AmSpaceHttpClient>().As<IAmSpaceClient>().SingleInstance();
            builder.RegisterType<MainWindowViewModel>().SingleInstance();
            builder.RegisterType<AmSpaceEnvironmentsProvider>().As<IAmSpaceEnvironmentsProvider>().SingleInstance();
            builder.RegisterType<ActiveDirectoryProvider>().As<IActiveDirectoryProvider>().SingleInstance();
            //transient
            builder.RegisterType<AmSpaceExcelWorker>().As<IExcelWorker>().InstancePerDependency();
            builder.RegisterDecorator<ExcelWorkerDecorator, IExcelWorker>();
            builder.RegisterType<ProgressIndicatorViewModel>().InstancePerDependency();
            builder.Register(context => LogManager.GetLogger(typeof(App))).As<ILog>().InstancePerDependency();
            builder.Register(context => new Mapper(_mapperConfiguration)).As<IMapper>().InstancePerDependency();

            _container = builder.Build();
        }
    }
}
