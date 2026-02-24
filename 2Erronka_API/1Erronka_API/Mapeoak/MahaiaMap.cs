using FluentNHibernate.Mapping;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Mapeoak
{
    public class MahaiaMap : ClassMap<Mahaia>
    {
        public MahaiaMap()
        {
            Table("mahaiak");

            Id(x => x.Id).Column("id").GeneratedBy.Identity();
            Map(x => x.Zenbakia).Column("zenbakia");
            Map(x => x.PertsonaKopurua).Column("pertsona_kopurua");
            Map(x => x.Kokapena).Column("kokapena");
        }
    }
}
