using NHibernate;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Repositorioak
{
    public class ProduktuaOsagaiaRepository
    {
        private readonly NHibernate.ISession _session;

        public ProduktuaOsagaiaRepository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public IList<ProduktuaOsagaia> GetByProduktuaId(int produktuaId)
        {
            return _session.Query<ProduktuaOsagaia>()
                .Where(po => po.Produktua.Id == produktuaId)
                .ToList();
        }

        public void UpdateOsagaia(Osagaia osagaia)
        {
            _session.Update(osagaia);
        }
    }
}
