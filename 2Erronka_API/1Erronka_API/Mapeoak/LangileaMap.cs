using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using _1Erronka_API.Domain;

namespace _1Erronka_API.Mapeoak
{
    public class LangileaMap : ClassMap<Langilea>
    {
        public LangileaMap()
        {
            Table("langileak");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Izena);
            Map(x => x.Erabiltzaile_izena);
            Map(x => x.Langile_kodea);
            Map(x => x.Pasahitza);
            Map(x => x.Gerentea);
            Map(x => x.TpvSarrera).Column("tpv_sarrera");
        }
    }
}
