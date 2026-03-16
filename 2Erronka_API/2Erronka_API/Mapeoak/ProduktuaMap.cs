using FluentNHibernate.Mapping;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Mapeoak
{
    public class ProduktuaMap : ClassMap<Produktua>
    {
        public ProduktuaMap()
        {
            Table("produktuak");

            Id(x => x.Id).Column("id").GeneratedBy.Identity();
            Map(x => x.Izena).Column("izena");
            Map(x => x.Prezioa).Column("prezioa");
            Map(x => x.Stock).Column("stock");
            Map(x => x.MotaId).Column("mota_id");

            HasMany(x => x.Osagaiak)
                .Cascade.All()
                .Inverse()
                .KeyColumn("produktuak_id");
        }
    }
}
