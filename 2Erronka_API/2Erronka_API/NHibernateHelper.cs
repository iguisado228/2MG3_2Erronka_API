using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using _2Erronka_API.Mapeoak;
using _2Erronka_API.Modeloak;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace _2Erronka_API
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        public static NHibernate.ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public static ISessionFactory SessionFactory =>
            _sessionFactory ??= CreateSessionFactory();

        private static ISessionFactory CreateSessionFactory()
        {
            var config = Fluently.Configure()
                .Database(MySQLConfiguration.Standard
                //.ConnectionString("Server=192.168.10.5;Port=3306;Database=2mg3_2erronka;Uid=root;Pwd=2Taldea2;"))
                .ConnectionString("Server=localhost;Port=3306;Database=2mg3_2erronka;Uid=admin;Pwd=2Taldea2;"))
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssembly(typeof(NHibernateHelper).Assembly);
                })
                .ExposeConfiguration(cfg =>
                {
                    cfg.SetProperty("current_session_context_class", "async_local");
                })
                .BuildConfiguration();

            dbEguneratu(config);

                return config.BuildSessionFactory();
        }

        public static void dbEguneratu(NHibernate.Cfg.Configuration config)
        {
            //Eguneratu

            SchemaUpdate schemaUpdate = new SchemaUpdate(config);
            schemaUpdate.Execute(false, true);

        }       
    }
}
