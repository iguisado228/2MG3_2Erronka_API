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
        /// <summary>Langilearen erabiltzaile-izena.</summary>
        public virtual string Erabiltzaile_izena { get; set; }
        /// <summary>Langilearen kodea.</summary>
        public int Langile_kodea { get; set; }
        /// <summary>Langilearen pasahitza (APIan ez da beti bidaltzen).</summary>
        public virtual string Pasahitza { get; set; }
        /// <summary>Gerentea den adierazlea.</summary>
        public virtual Boolean Gerentea { get; set; }
        /// <summary>TPV-ra sartzeko baimena duen adierazlea.</summary>
        public virtual Boolean TpvSarrera { get; set; }
    }
}