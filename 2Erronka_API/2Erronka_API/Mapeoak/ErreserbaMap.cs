using FluentNHibernate.Mapping;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Mapeoak
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
            Map(x => x.PrezioTotalaHasierakoa).Column("prezio_totala_hasierakoa").Nullable();
            Map(x => x.Ordainduta).Column("ordainduta");
            Map(x => x.FakturaRuta).Column("faktura_ruta");
            Map(x => x.DeskontuKodea).Column("deskontu_kodea").Nullable();
            Map(x => x.DeskontuMota).Column("deskontu_mota").Nullable();
            Map(x => x.DeskontuBalioa).Column("deskontu_balioa").Nullable();
            Map(x => x.DeskontuZenbatekoa).Column("deskontu_zenbatekoa");
            References(x => x.Langilea).Column("langileak_id").Not.Nullable();
            References(x => x.Mahaia).Column("mahaiak_id").Not.Nullable();
        }
    }

}
