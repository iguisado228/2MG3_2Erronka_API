using NHibernate;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Repositorioak
{
    public class OsagaiaRepository
    {
        private readonly NHibernate.ISession _session;

        public OsagaiaRepository(NHibernate.ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }


        public virtual Osagaia? Get(int id) =>
            _session.Query<Osagaia>().FirstOrDefault(x => x.Id == id);

        public virtual IList<Osagaia> GetAll() => _session.Query<Osagaia>().ToList();

        public virtual void Update(Osagaia osagaia)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Update(osagaia);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Update(osagaia);
                tx.Commit();
            }
        }

        public virtual void Add(Osagaia osagaia)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(osagaia);
            tx.Commit();
        }

        public virtual void Delete(Osagaia osagaia)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(osagaia);
            tx.Commit();
        }
    }
}
