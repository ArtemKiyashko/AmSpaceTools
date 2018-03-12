using AmSpaceClient;
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
            });
        }
    }
}
