using ISession = NHibernate.ISession;
using ISessionFactory = NHibernate.ISessionFactory;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Repositorioak
{
    public class HornitzaileaRepository
    {
        private readonly ISession _session;

        public HornitzaileaRepository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public virtual IList<Hornitzailea> GetAll()
        {
            return _session.Query<Hornitzailea>().ToList();
        }

        public virtual Hornitzailea? Get(int id)
        {
            return _session.Get<Hornitzailea>(id);
        }

        public virtual void Add(Hornitzailea h)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(h);
            tx.Commit();
        }

        public virtual void Update(Hornitzailea h)
        {
            using var tx = _session.BeginTransaction();
            _session.Update(h);
            tx.Commit();
        }

        public virtual void Delete(Hornitzailea h)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(h);
            tx.Commit();
        }
    }
}
