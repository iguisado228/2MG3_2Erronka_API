using _1Erronka_API.DTOak;
using _1Erronka_API.Modeloak;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;


namespace _1Erronka_API.Repositorioak
{
    public class ErreserbaRepository
    {
        private readonly ISession _session;

        public ErreserbaRepository(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.GetCurrentSession();
        }

        public IList<Erreserba> GetAll()
        {
            return _session.Query<Erreserba>()
                .Fetch(x => x.Langilea)
                .Fetch(x => x.Mahaia)
                .ToList();
        }

        public Erreserba? Get(int id)
        {
            return _session.Query<Erreserba>().FirstOrDefault(x => x.Id == id);
        }

        public void Add(Erreserba erreserba)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Save(erreserba);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Save(erreserba);
                tx.Commit();
            }
        }

        public void Update(Erreserba erreserba)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Update(erreserba);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Update(erreserba);
                tx.Commit();
            }
        }

        public void Delete(Erreserba erreserba)
        {
            if (_session.Transaction != null && _session.Transaction.IsActive)
            {
                _session.Delete(erreserba);
            }
            else
            {
                using var tx = _session.BeginTransaction();
                _session.Delete(erreserba);
                tx.Commit();
            }
        }

        public List<EskariaProduktuaDto> LortuProduktuakErreserbarako(int erreserbaId)
        {
            var eskariak = _session.Query<Eskaria>()
                .Where(e => e.Erreserba.Id == erreserbaId)
                .ToList();

            var produktuak = eskariak
                .SelectMany(e => e.Produktuak)
                .Select(p => new EskariaProduktuaDto
                {
                    ProduktuaIzena = p.Produktua.Izena,
                    Prezioa = p.Prezioa,
                    Kantitatea = p.Kantitatea
                })
                .ToList();

            return produktuak;
        }

        public ISession OpenSession()
        {
            return _session.SessionFactory.OpenSession();
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
