using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using MappingConfiguration = FluentNHibernate.Cfg.MappingConfiguration;

namespace NHibernateBootstrap
{
    public static class NHibernateBuilder
    {
        private static AutoPersistenceModel _model;
        private static Configuration _configuration;
        private static ISessionFactory _sessionFactory;

        public static void Setup<T>(IPersistenceConfigurer config)
        {
            _model = AutoMap.AssemblyOf<T>().Where(type => typeof(IHaveId).IsAssignableFrom(type));
            _model.OverrideAll(map => map.IgnoreProperties(x => x.CanWrite == false));

            _configuration = Fluently.Configure().
                Database(config).
                Mappings(m => AddAutoMapping(m)).
                ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true)).
                BuildConfiguration();
        }

        private static AutoMappingsContainer AddAutoMapping(MappingConfiguration m)
        {
            return m.AutoMappings.Add(_model);
        }

        public static ISessionFactory GetSessionFactory()
        {
            AssertSetupWasCalled();
            if (_sessionFactory != null) return _sessionFactory;
            return _sessionFactory = _configuration.BuildSessionFactory();
        }

        private static void AssertSetupWasCalled()
        {
            if (_configuration == null)
            {
                throw new SetupNotCalledException();
            }
        }
    }
}
