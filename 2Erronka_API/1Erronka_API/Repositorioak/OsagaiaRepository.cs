using NHibernate;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Repositorioak
{
    public class OsagaiaRepository
    {
        private readonly NHibernate.ISession _session;

        public OsagaiaRepository(NHibernate.ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }


        public Osagaia? Get(int id) =>
            _session.Query<Osagaia>().FirstOrDefault(x => x.Id == id);

        public IList<Osagaia> GetAll() => _session.Query<Osagaia>().ToList();

        public void Update(Osagaia osagaia)
        {
            _session.Update(osagaia);
        }
    }
}
