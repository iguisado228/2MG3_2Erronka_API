using NHibernate;
using _2Erronka_API.Modeloak;
using FluentNHibernate.Mapping;
using _2Erronka_API.Domain;

namespace _2Erronka_API.Repositorioak
{
    public class LangileaRepository
    {
        private readonly NHibernate.ISession _session;

        public LangileaRepository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public virtual void Add(Langilea langilea)
        {
            using var tx = _session.BeginTransaction();

            _session.Save(langilea);

            tx.Commit();
        }

        public virtual Langilea? Get(int id, bool eager = false)
        {
            var query = _session.Query<Langilea>()
                .Where(x => x.Id == id);

            var langilea = query.SingleOrDefault();
            return langilea;
        }

        public virtual Langilea? GetByKodea(int kodea)
        {
            return _session.Query<Langilea>()
                .FirstOrDefault(u => u.Langile_kodea == kodea);
        }

        public virtual IList<Langilea> GetAll()
        {
            return _session.Query<Langilea>().ToList();
        }

        public virtual IList<Langilea> GetByLanpostuaId(int lanpostuaId)
        {
            return _session.Query<Langilea>()
                .Where(x => x.Lanpostua != null && x.Lanpostua.Id == lanpostuaId)
                .ToList();
        }

        public virtual void Update(Langilea langilea)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Update(langilea);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Update(langilea);
                tx.Commit();
            }
        }

        public virtual void Delete(Langilea langilea)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Delete(langilea);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Delete(langilea);
                tx.Commit();
            }
        }

    }
}
