using FluentNHibernate.Mapping;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Mapeoak
{
    public class EskariaMap : ClassMap<Eskaria>
    {
        public EskariaMap()
        {
            Table("eskariak");

            Id(x => x.Id).Column("id").GeneratedBy.Identity();
            Map(x => x.Prezioa).Column("prezioa");
            Map(x => x.Egoera).Column("egoera");
            References(x => x.Erreserba).Column("erreserbak_id");
            HasMany(x => x.Produktuak).Cascade.All().Inverse().KeyColumn("eskariak_id");
            References(x => x.Langilea).Column("erreserbak_langileak_id").Not.Nullable();
            References(x => x.Mahaia).Column("erreserbak_mahaiak_id").Not.Nullable();

        }
    }
}
