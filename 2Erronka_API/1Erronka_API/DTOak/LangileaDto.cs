namespace _1Erronka_API.DTOak
{
    /// <summary>
    /// Langile baten datu-laburpena, login eta bestelako erantzunetan erabiltzeko.
    /// </summary>
    public class LangileaDto
    {
        /// <summary>Langilearen identifikatzailea.</summary>
        public virtual int Id { get; set; }
        /// <summary>Langilearen izena.</summary>
        public virtual string Izena { get; set; }
        /// <summary>Langilearen abizena.</summary>
        public virtual string Abizena { get; set; }
        /// <summary>Langilearen NANa.</summary>
        public virtual string NAN { get; set; }
        /// <summary>Langilearen erabiltzaile-izena.</summary>
        public virtual string Erabiltzaile_izena { get; set; }
        /// <summary>Langilearen kodea.</summary>
        public int Langile_kodea { get; set; }
        /// <summary>Langilearen pasahitza (APIan ez da beti bidaltzen).</summary>
        public virtual string Pasahitza { get; set; }
        /// <summary>Langilearen helbidea.</summary>
        public virtual string Helbidea { get; set; }
        /// <summary>Langilearen lanpostua.</summary>
        public virtual LanpostuaDto Lanpostua { get; set; }
    }
}