using NHibernate;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Repositorioak
{
    public class ProduktuaRepository
    {
        private readonly NHibernate.ISession _session;

        public ProduktuaRepository(NHibernate.ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }


        public virtual Produktua? Get(int id) =>
            _session.Query<Produktua>().FirstOrDefault(x => x.Id == id);

        public virtual IList<Produktua> GetAll() => _session.Query<Produktua>().ToList();

        public virtual void Update(Produktua produktua)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Update(produktua);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Update(produktua);
                tx.Commit();
            }
        }

        public virtual void Add(Produktua produktua)
        {
            using var tx = _session.BeginTransaction();
            _session.Save(produktua);
            tx.Commit();
        }

        public virtual void Delete(Produktua produktua)
        {
            using var tx = _session.BeginTransaction();
            _session.Delete(produktua);
            tx.Commit();
        }
    }
}
