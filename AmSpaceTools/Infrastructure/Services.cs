using AmSpaceClient;
using AmSpaceModels;
using AmSpaceTools.Decorators;
using AmSpaceTools.ModelConverters;
using AmSpaceTools.ViewModels;
using AutoMapper;
using ExcelWorker;
using log4net;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure
{
    public static class Services
    {
        private static Container _container;
        private static IConfigurationProvider _mapperConfiguration
        {
            get
            {
                return new MapperConfiguration(cfg => {
                    cfg.CreateMap<IEnumerable<CompetencyActionViewModel>, IEnumerable<CompetencyActionDto>>().ConvertUsing(new CompetencyActionConverter());
                    cfg.CreateMap<IEnumerable<CompetencyAction>, IEnumerable<UpdateAction>>().ConvertUsing(new CompetencyActionsToUpdateConverter());
                    cfg.CreateMap<IEnumerable<IdpExcelRow>, IEnumerable<UpdateAction>>().ConvertUsing(new IdpExcelRowToUpdateConverter());
                    cfg.CreateMap<CompetencyAction, UpdateAction>().ConvertUsing(new ActionToUpdateConverter());
                });
            }
        }
        public static Container Container
        {
            get
            {
                return _container;
            }
        }
        static Services()
        {
            _container = new Container(_ => {
                _.For<IAmSpaceClient>().Use<AmSpaceClient.FakeClient>().Singleton();
                _.For<MainWindowViewModel>().Use<MainWindowViewModel>().Singleton();
                _.For<IExcelWorker<CompetencyActionDto>>().Use<AmSpaceExcelWorker<CompetencyActionDto>>().Transient();
                _.For<IExcelWorker<CompetencyActionDto>>().DecorateAllWith<ExcelWorkerDecorator<CompetencyActionDto>>();
                _.For<ILog>().Use(a => LogManager.GetLogger(typeof(App)));
                _.For<IMapper>().Use(a => new Mapper(_mapperConfiguration)).Singleton();
                _.Scan(scanner => {
                    scanner.TheCallingAssembly();
                    scanner.WithDefaultConventions();
                });
            });
        }
    }
}
