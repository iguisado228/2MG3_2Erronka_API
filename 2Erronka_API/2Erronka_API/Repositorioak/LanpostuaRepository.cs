using ISession = NHibernate.ISession;
using ISessionFactory = NHibernate.ISessionFactory;
using _2Erronka_API.Domain;

namespace _2Erronka_API.Repositorioak
{
    public class LanpostuaRepository
    {
        private readonly ISession _session;

        public LanpostuaRepository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public virtual void Add(Lanpostua lanpostua)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(lanpostua);
            tx.Commit();
        }

        public virtual IList<Lanpostua> GetAll()
        {
            return _session.Query<Lanpostua>().ToList();
        }

        public virtual Lanpostua? Get(int id)
        {
            return _session.Get<Lanpostua>(id);
        }

        public virtual void Update(Lanpostua lanpostua)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Update(lanpostua);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Update(lanpostua);
                tx.Commit();
            }
        }

        public virtual void Delete(Lanpostua lanpostua)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Delete(lanpostua);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Delete(lanpostua);
                tx.Commit();
            }
        }
    }
}
