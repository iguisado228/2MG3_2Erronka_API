using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1Erronka_API.Domain
{
    public class Langilea
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; }
        public virtual string Erabiltzaile_izena { get; set; }
        public virtual int Langile_kodea { get; set; }
        public virtual string Pasahitza { get; set; }
        public virtual Boolean Gerentea { get; set; }
        public virtual Boolean TpvSarrera { get; set; }
    }
}
