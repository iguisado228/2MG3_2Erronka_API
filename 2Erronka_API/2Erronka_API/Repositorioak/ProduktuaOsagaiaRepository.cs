using NHibernate;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Repositorioak
{
    public class ProduktuaOsagaiaRepository
    {
        private readonly NHibernate.ISession _session;

        public ProduktuaOsagaiaRepository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public virtual IList<ProduktuaOsagaia> GetByProduktuaId(int produktuaId)
        {
            return _session.Query<ProduktuaOsagaia>()
                .Where(po => po.Produktua.Id == produktuaId)
                .ToList();
        }

        public virtual ProduktuaOsagaia? GetOne(int produktuaId, int osagaiaId)
        {
            return _session.Query<ProduktuaOsagaia>()
                .FirstOrDefault(po =>
                    po.Produktua.Id == produktuaId &&
                    po.Osagaia.Id == osagaiaId);
        }

        public virtual void SaveOrUpdate(ProduktuaOsagaia entitatea)
        {
            _session.SaveOrUpdate(entitatea);
        }

        public virtual void Delete(ProduktuaOsagaia entitatea)
        {
            _session.Delete(entitatea);
        }

        public virtual void UpdateOsagaia(Osagaia osagaia)
        {
            _session.Update(osagaia);
        }
    }
}
