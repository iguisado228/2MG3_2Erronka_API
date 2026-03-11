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


        public Produktua? Get(int id) =>
            _session.Query<Produktua>().FirstOrDefault(x => x.Id == id);

        public IList<Produktua> GetAll() => _session.Query<Produktua>().ToList();

        public void Update(Produktua produktua)
        {
            _session.Update(produktua); _session.Flush();
        }

    }
}
