using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Engine;

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

        public static void Setup<T>(IPersistenceConfigurer config)
        {
            _model = AutoMap.AssemblyOf<T>().Where(type => typeof(IHaveId).IsAssignableFrom(type));
            _model.Conventions.Add(DefaultCascade.All());
            _model.OverrideAll(map => map.IgnoreProperties(x => x.CanWrite == false));

            _fluentConfiguration = Fluently.Configure().Database(config).Mappings(m => m.AutoMappings.Add(_model));

            _configuration = Fluently.Configure().
                Database(config).
                Mappings(m => m.AutoMappings.Add(_model)).
                ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true)).
                BuildConfiguration();
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
            return Configuration.BuildSessionFactory();
        }

        public static void Reset()
        {
            _fluentConfiguration
                .ExposeConfiguration(BuildSchema)
                .BuildConfiguration();






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
