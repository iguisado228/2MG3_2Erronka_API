using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Erronka_API.Domain
{
    public class Langilea
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; }
        public virtual string Abizena { get; set; }
        public virtual string NAN { get; set; }
        public virtual string Erabiltzaile_izena { get; set; }
        public virtual int Langile_kodea { get; set; }
        public virtual string Pasahitza { get; set; }
        public virtual string Helbidea { get; set; }
        public virtual Lanpostua Lanpostua { get; set; }
    }
}
