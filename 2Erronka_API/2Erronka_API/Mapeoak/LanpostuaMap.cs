using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using _2Erronka_API.Domain;

namespace _2Erronka_API.Mapeoak
{
    public class LanpostuaMap : ClassMap<Lanpostua>
    {
        public LanpostuaMap()
        {
            Table("lanpostuak");
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Lanpostu_izena).Column("lanpostua").Not.Nullable();
            
            HasMany(x => x.Langileak)
                .Inverse()
                .Cascade.All();
        }
    }
}
