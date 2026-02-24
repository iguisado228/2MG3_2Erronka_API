using FluentNHibernate.Mapping;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Mapeoak
{
    public class ProduktuaMap : ClassMap<Produktua>
    {
        public ProduktuaMap()
        {
            Table("produktuak");

            Id(x => x.Id).Column("id").GeneratedBy.Identity();
            Map(x => x.Izena).Column("izena");
            Map(x => x.Prezioa).Column("prezioa");
            Map(x => x.Mota).Column("mota");
            Map(x => x.Stock).Column("stock");

            HasMany(x => x.Osagaiak)
                .Cascade.All()
                .Inverse()
                .KeyColumn("produktuak_id");
        }
    }
}
