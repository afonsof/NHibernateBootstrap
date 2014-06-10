using System;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Engine;
using MappingConfiguration = FluentNHibernate.Cfg.MappingConfiguration;

namespace NHibernateBootstrap
{
    public static class NHibernateBuilder
    {
        private static AutoPersistenceModel _model;

        private static FluentConfiguration _fluentConfiguration;
        private static Configuration _configuration;

        public static Configuration Configuration
        {
            get { return _configuration; }
        }

        public static void Setup<T>(IPersistenceConfigurer config, Action<AutoPersistenceModel> func = null )
        {
            _model = AutoMap.AssemblyOf<T>().Where(type => typeof(IHaveId).IsAssignableFrom(type));
            _model.OverrideAll(map => map.IgnoreProperties(x => x.CanWrite == false));

            if (func != null)
            {
                func(_model);
            }

            _fluentConfiguration = Fluently.Configure().Database(config).Mappings(m => AddAutoMapping(m));

            _configuration = Fluently.Configure().
                Database(config).
                Mappings(m => AddAutoMapping(m)).
                //ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true)).
                BuildConfiguration();
        }

        private static AutoMappingsContainer AddAutoMapping(MappingConfiguration m)
        {
            return m.AutoMappings.Add(_model);
        }

        public static void SetupMySql<T>(string connectionString)
        {
            Setup<T>(MySQLConfiguration.Standard.ConnectionString(connectionString));
        }

        public static void SetupMsSql<T>(string connectionString)
        {
            Setup<T>(MsSqlConfiguration.MsSql2008.ConnectionString(connectionString));
        }

        public static void SetupSqLite<T>(string connectionString)
        {
            Setup<T>(SQLiteConfiguration.Standard.ConnectionString(connectionString));
        }

        public static ISessionFactory GetSessionFactory()
        {
            AssertSetupWasCalled();
            return Configuration.BuildSessionFactory();
        }

        public static void Reset()
        {
            AssertSetupWasCalled();
            _fluentConfiguration
                .ExposeConfiguration(BuildSchema)
                .BuildConfiguration();
        }

        private static void AssertSetupWasCalled()
        {
            if (_configuration == null || _fluentConfiguration == null)
            {
                throw new SetupNotCalledException();
            }
        }

        private static void BuildSchema(Configuration config)
        {
            var nhvc = new NHibernate.Validator.Cfg.Loquacious.FluentConfiguration();
            var validator = new ValidatorEngine();
            validator.Configure(nhvc);
            config.Initialize(validator);

            var schemaExport = new SchemaExport(config);

            schemaExport.Drop(true, true);
            schemaExport.Create(true, true);
        }
    }
}
