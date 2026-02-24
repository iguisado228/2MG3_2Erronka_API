using NHibernate;
using _1Erronka_API.Modeloak;
using FluentNHibernate.Mapping;
using _1Erronka_API.Domain;

namespace _1Erronka_API.Repositorioak
{
    public class LangileaRepository
    {
        private readonly NHibernate.ISession _session;

        public LangileaRepository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public void Add(Langilea langilea)
        {
            using var tx = _session.BeginTransaction();

            _session.Save(langilea);

            tx.Commit();
        }

        public Langilea? Get(int id, bool eager = false)
        {
            var query = _session.Query<Langilea>()
                .Where(x => x.Id == id);

            var langilea = query.SingleOrDefault();
            return langilea;

        }

        public IList<Langilea> GetAll()
        {
            return _session.Query<Langilea>().ToList();
        }

    }
}
