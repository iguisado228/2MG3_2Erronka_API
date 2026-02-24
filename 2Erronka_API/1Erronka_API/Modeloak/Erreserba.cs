using _1Erronka_API.Domain;

namespace _1Erronka_API.Modeloak
{
    public class Erreserba
    {
        public virtual int Id { get; set; }
        public virtual string BezeroIzena { get; set; }
        public virtual string Telefonoa { get; set; }
        public virtual int PertsonaKopurua { get; set; }
        public virtual DateTime EgunaOrdua { get; set; }
        public virtual double PrezioTotala { get; set; }
        public virtual int Ordainduta { get; set; } = 0;
        public virtual string FakturaRuta { get; set; }
        public virtual Langilea Langilea { get; set; }
        public virtual Mahaia Mahaia { get; set; }
    }

}
