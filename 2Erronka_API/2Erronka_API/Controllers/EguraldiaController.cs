using _2Erronka_API.DTOak;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace _2Erronka_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EguraldiaController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EguraldiaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("azkena")]
        public IActionResult GetAzkena()
        {
            string xmlKarpeta = _configuration["Eguraldia:XmlKarpeta"] ?? string.Empty;
            string xmlPatroia = _configuration["Eguraldia:XmlPatroia"] ?? "eguraldia_*.xml";

            if (string.IsNullOrWhiteSpace(xmlKarpeta))
            {
                return StatusCode(500, new { message = "Eguraldiaren XML karpeta ez dago konfiguratuta." });
            }

            if (!System.IO.Directory.Exists(xmlKarpeta))
            {
                return NotFound(new { message = "Eguraldiaren XML karpeta ez da existitzen." });
            }

            string? azkenXmla = System.IO.Directory
                .GetFiles(xmlKarpeta, xmlPatroia)
                .OrderByDescending(System.IO.File.GetLastWriteTimeUtc)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(azkenXmla))
            {
                return NotFound(new { message = "Ez da eguraldi XML fitxategirik aurkitu." });
            }

            XDocument dokumentua = XDocument.Load(azkenXmla);
            XElement? erroa = dokumentua.Element("eguraldia");
            if (erroa == null)
            {
                return BadRequest(new { message = "Eguraldi XMLaren erroa ez da zuzena." });
            }

            EguraldiaDto eguraldia = new()
            {
                Udalerria = BalioaLortu(erroa.Element("udalerria")),
                Probintzia = BalioaLortu(erroa.Element("probintzia"))
            };

            XElement? iragarpena = erroa.Element("iragarpena");
            if (iragarpena != null)
            {
                eguraldia.Egunak = iragarpena
                    .Elements("eguna")
                    .Select(eguna => new EguraldiEgunaDto
                    {
                        Data = BalioaLortu(eguna.Attribute("data")),
                        AstekoEguna = BalioaLortu(eguna.Element("astekoEguna")),
                        ZeruEgoera = AtributuaLortu(eguna.Element("zeruEgoera"), "textua"),
                        ZeruEgoeraKodea = BalioaLortu(eguna.Element("zeruEgoera")),
                        TenperaturaMinimoa = BalioaLortu(eguna.Element("tenperaturaMinimoa")),
                        TenperaturaMaximoa = BalioaLortu(eguna.Element("tenperaturaMaximoa")),
                        PrezipitazioProbabilitatea = BalioaLortu(eguna.Element("prezipitazioProbabilitatea")),
                        Xehetasunak = eguna
                            .Element("xehetasunak")?
                            .Elements("tartea")
                            .Select(tartea => new EguraldiTarteaDto
                            {
                                Aldia = BalioaLortu(tartea.Attribute("aldia")),
                                Ordua = BalioaLortu(tartea.Element("ordua")),
                                Tenperatura = BalioaLortu(tartea.Element("tenperatura")),
                                ZeruEgoera = AtributuaLortu(tartea.Element("zeruEgoera"), "textua"),
                                ZeruEgoeraKodea = BalioaLortu(tartea.Element("zeruEgoera"))
                            })
                            .ToList() ?? new List<EguraldiTarteaDto>()
                    })
                    .ToList();
            }

            return Ok(eguraldia);
        }

        private static string BalioaLortu(XElement? elementua)
        {
            return elementua?.Value?.Trim() ?? string.Empty;
        }

        private static string BalioaLortu(XAttribute? atributua)
        {
            return atributua?.Value?.Trim() ?? string.Empty;
        }

        private static string AtributuaLortu(XElement? elementua, string atributua)
        {
            string balioa = elementua?.Attribute(atributua)?.Value?.Trim() ?? string.Empty;
            return string.IsNullOrWhiteSpace(balioa)
                ? BalioaLortu(elementua)
                : balioa;
        }
    }
}
