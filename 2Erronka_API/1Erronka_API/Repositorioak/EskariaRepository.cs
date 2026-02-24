using NHibernate;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Repositorioak
{
    public class EskariaRepository
    {
        private readonly NHibernate.ISession _session;

        public EskariaRepository(NHibernate.ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }


        public IList<Eskaria> GetAll() => _session.Query<Eskaria>().ToList();

        public Eskaria? Get(int id) =>
            _session.Query<Eskaria>().FirstOrDefault(x => x.Id == id);

        public void Add(Eskaria eskaria)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Save(eskaria);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Save(eskaria);
                tx.Commit();
            }
        }

        public void Update(Eskaria eskaria)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Update(eskaria);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Update(eskaria);
                tx.Commit();
            }
        }

        public void Delete(Eskaria eskaria)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Delete(eskaria);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Delete(eskaria);
                tx.Commit();
            }
        }

        public void ExecuteSerializableTransaction(Action action)
        {
            using var tx = _session.BeginTransaction(System.Data.IsolationLevel.Serializable);
            try
            {
                action();
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}
