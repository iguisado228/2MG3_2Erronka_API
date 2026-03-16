using FluentNHibernate.Mapping;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Mapeoak
{
    public class MotaMap : ClassMap<Mota>
    {
        public MotaMap()
        {
            Table("mota");

            Id(x => x.Id).Column("id").GeneratedBy.Assigned();
            Map(x => x.Izena).Column("izena");
        }
    }
}
