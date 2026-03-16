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

        public virtual IList<Lanpostua> GetAll()
        {
            return _session.Query<Lanpostua>().ToList();
        }

        public virtual Lanpostua? Get(int id)
        {
            return _session.Get<Lanpostua>(id);
        }
    }
}
