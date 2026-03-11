using System;
using System.Collections.Generic;

namespace _2Erronka_API.Domain
{
    /// <summary>
    /// Langileen lanpostu motak.
    /// </summary>
    public class Lanpostua
    {
        /// <summary>Lanpostuaren identifikatzailea.</summary>
        public virtual int Id { get; set; }
        /// <summary>Lanpostuaren izena.</summary>
        public virtual string Lanpostu_izena { get; set; }

        /// <summary>Lanpostu honetan dauden langileak.</summary>
        public virtual IList<Langilea> Langileak { get; set; } = new List<Langilea>();
    }
}
