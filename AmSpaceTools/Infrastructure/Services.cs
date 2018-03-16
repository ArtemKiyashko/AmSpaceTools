using AmSpaceClient;
using AmSpaceTools.ViewModels;
using ExcelWorker;
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
                _.For<IAmSpaceClient>().Use<AmSpaceClient.AmSpaceClient>().Singleton();
                _.For<MainWindowViewModel>().Use<MainWindowViewModel>().Singleton();
                _.For<IExcelWorker>().Use<AmSpaceExcelWorker>().Transient();
                _.Scan(scanner => {
                    scanner.TheCallingAssembly();
                    scanner.WithDefaultConventions();
                });
            });
        }
    }
}
