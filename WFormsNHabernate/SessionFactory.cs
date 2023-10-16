using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using System;

namespace WFormsNHibernate
{
    public class SessionFactory
    {
        private static volatile ISessionFactory iSessionFactory;
        private static object syncRoot = new Object();

        public static ISession OpenSession
        {
            get
            {
                if (iSessionFactory == null)
                {
                    lock (syncRoot)
                    {
                        if (iSessionFactory == null)
                        {
                            iSessionFactory = BuildSessionFactory();
                        }
                    }
                }
                return iSessionFactory.OpenSession();
            }
        }

        private static ISessionFactory BuildSessionFactory()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.AppSettings["connection_string"];

                return Fluently.Configure()
                     .Database(MsSqlConfiguration.MsSql2012
                     .ConnectionString(connectionString))
                     .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Program>())
                     .ExposeConfiguration(BuildSchema)
                     .BuildSessionFactory();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                throw ex;
            }
        }

        /* Create session */
        private static AutoPersistenceModel CreateMappings()
        {
            return AutoMap
                .Assembly(System.Reflection.Assembly.GetCallingAssembly())
                .Where(t => t.Namespace == "WFormsNHibernate.Model");
        }

        private static void BuildSchema(Configuration config)
        {
        }

    }
}
