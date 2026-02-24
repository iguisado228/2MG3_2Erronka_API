using NHibernate;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Repositorioak
{
    public class MahaiaRepository
    {
        private readonly NHibernate.ISession _session;

        public MahaiaRepository(NHibernate.ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public IList<Mahaia> GetAll() => _session.Query<Mahaia>().ToList();

        public Mahaia? Get(int id) => _session.Get<Mahaia>(id);

        public void Add(Mahaia mahaia)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(mahaia);
            tx.Commit();
        }

        public void Update(Mahaia mahaia)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(mahaia);
            tx.Commit();
        }
        public void Delete(Mahaia mahaia)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(mahaia);
            tx.Commit();
        }

    }
}