using NHibernate;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Repositorioak
{
    public class MahaiaRepository
    {
        private readonly NHibernate.ISession _session;

        public MahaiaRepository(NHibernate.ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public virtual IList<Mahaia> GetAll() => _session.Query<Mahaia>().ToList();

        public virtual Mahaia? Get(int id) => _session.Get<Mahaia>(id);

        public virtual void Add(Mahaia mahaia)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(mahaia);
            tx.Commit();
        }

        public virtual void Update(Mahaia mahaia)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(mahaia);
            tx.Commit();
        }
        public virtual void Delete(Mahaia mahaia)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(mahaia);
            tx.Commit();
        }

    }
}