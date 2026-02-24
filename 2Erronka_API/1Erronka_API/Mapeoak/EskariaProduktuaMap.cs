using FluentNHibernate.Mapping;
using _1Erronka_API.Modeloak;

public class EskariaProduktuaMap : ClassMap<EskariaProduktua>
{
    public EskariaProduktuaMap()
    {
        Table("eskariak_has_produktuak");

        CompositeId()
            .KeyReference(x => x.Eskaria, "eskariak_id")
            .KeyReference(x => x.Produktua, "produktuak_id");

        Map(x => x.Kantitatea).Column("kantitatea").Not.Nullable();
        Map(x => x.Prezioa).Column("prezioa").Not.Nullable();
    }
}
