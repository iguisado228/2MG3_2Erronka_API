using FluentNHibernate.Mapping;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Mapeoak
{
    public class ErreserbaMap : ClassMap<Erreserba>
    {
        public ErreserbaMap()
        {
            Table("erreserbak");

            Id(x => x.Id).Column("id").GeneratedBy.Identity();
            Map(x => x.BezeroIzena).Column("bezero_izena");
            Map(x => x.Telefonoa).Column("telefonoa");
            Map(x => x.PertsonaKopurua).Column("pertsona_kopurua");
            Map(x => x.EgunaOrdua).Column("eguna_ordua");
            Map(x => x.PrezioTotala).Column("prezio_totala");
            Map(x => x.Ordainduta).Column("ordainduta");
            Map(x => x.FakturaRuta).Column("faktura_ruta");
            References(x => x.Langilea).Column("langileak_id").Not.Nullable();
            References(x => x.Mahaia).Column("mahaiak_id").Not.Nullable();
        }
    }

}
