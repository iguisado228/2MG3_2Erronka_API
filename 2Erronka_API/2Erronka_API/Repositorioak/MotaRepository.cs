using ISession = NHibernate.ISession;
using ISessionFactory = NHibernate.ISessionFactory;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Repositorioak
{
    public class MotaRepository
    {
        private readonly ISession _session;

        public MotaRepository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public virtual IList<Mota> GetAll()
        {
            return _session.Query<Mota>().ToList();
        }

        public virtual Mota? Get(int id)
        {
            return _session.Get<Mota>(id);
        }

        public virtual void Add(Mota m)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(m);
            tx.Commit();
        }

        public virtual void Update(Mota m)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(m);
            tx.Commit();
        }

        public virtual void Delete(Mota m)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(m);
            tx.Commit();
        }
    }
}
