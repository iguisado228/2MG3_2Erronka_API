using FluentNHibernate.Mapping;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Mapeoak
{
    public class ProduktuaOsagaiaMap : ClassMap<ProduktuaOsagaia>
    {
        public ProduktuaOsagaiaMap()
        {
            Table("produktuak_has_osagaiak");

            CompositeId()
                .KeyReference(x => x.Produktua, "produktuak_id")
                .KeyReference(x => x.Osagaia, "osagaiak_id");

            Map(x => x.Kantitatea).Column("kantitatea").Not.Nullable();
        }
    }
}
