using _1Erronka_API.Domain;
using _1Erronka_API.DTOak;
using _1Erronka_API.Modeloak;
using _1Erronka_API.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using NHibernate;
 
// iText7
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
 
namespace _1Erronka_API.Controllers
{
    /// <summary>
    /// Erreserben kudeaketarako API amaierako puntuak.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ErreserbakController : ControllerBase
    {
        private readonly ErreserbaRepository _repo;
        private readonly EskariaRepository _eskariaRepo;
        private readonly ProduktuaRepository _produktuaRepo;
        private readonly ProduktuaOsagaiaRepository _produktuaOsagaiaRepo;
        private readonly OsagaiaRepository _osagaiaRepo;
 
        public ErreserbakController(ErreserbaRepository repo, EskariaRepository eskariaRepo, ProduktuaRepository produktuaRepo, ProduktuaOsagaiaRepository produktuaOsagaiaRepo, OsagaiaRepository osagaiaRepo)
        {
            _repo = repo;
            _eskariaRepo = eskariaRepo;
            _produktuaRepo = produktuaRepo;
            _produktuaOsagaiaRepo = produktuaOsagaiaRepo;
            _osagaiaRepo = osagaiaRepo;
        }
 
        /// <summary>
        /// Erreserba guztiak eskuratzen ditu.
        /// </summary>
        /// <returns>200 OK (ErreserbaDto zerrendarekin).</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var erreserbak = _repo.GetAll();
 
            var dtoList = erreserbak.Select(e => new ErreserbaDto
            {
                Id = e.Id,
                BezeroIzena = e.BezeroIzena,
                Telefonoa = e.Telefonoa,
                PertsonaKopurua = e.PertsonaKopurua,
                EgunaOrdua = e.EgunaOrdua,
                PrezioTotala = e.PrezioTotala,
                Ordainduta = e.Ordainduta,
                FakturaRuta = e.FakturaRuta,
                LangileaId = e.Langilea.Id,
                LangileaIzena = e.Langilea.Izena,
                MahaiakId = e.Mahaia.Id
            }).ToList();
 
            return Ok(dtoList);
        }
 
        /// <summary>
        /// Erreserba berria sortzen du.
        /// </summary>
        /// <param name="dto">Erreserba sortzeko datuak.</param>
        /// <returns>200 OK (mezua eta erreserbaId barne).</returns>
        [HttpPost]
        public IActionResult Sortu([FromBody] ErreserbaSortuDto dto)
        {
            var erreserba = new Erreserba
            {
                BezeroIzena = dto.BezeroIzena,
                Telefonoa = dto.Telefonoa,
                PertsonaKopurua = dto.PertsonaKopurua,
                EgunaOrdua = dto.EgunaOrdua,
                PrezioTotala = dto.PrezioTotala,
                FakturaRuta = dto.FakturaRuta,
                Langilea = new Langilea { Id = dto.LangileaId },
                Mahaia = new Mahaia { Id = dto.MahaiakId }
            };
 
            _repo.Add(erreserba);
            return Ok(new { mezua = "Erreserba sortuta", erreserbaId = erreserba.Id });
        }
 
        /// <summary>
        /// Existitzen den erreserba bat eguneratzen du.
        /// </summary>
        /// <param name="id">Erreserbaren identifikatzailea.</param>
        /// <param name="dto">Eguneratzeko erreserbaren datuak.</param>
        /// <returns>200 OK edo 404 Not Found.</returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ErreserbaSortuDto dto)
        {
            var erreserba = _repo.Get(id);
            if (erreserba == null) return NotFound();
 
            erreserba.BezeroIzena = dto.BezeroIzena;
            erreserba.Telefonoa = dto.Telefonoa;
            erreserba.PertsonaKopurua = dto.PertsonaKopurua;
            erreserba.EgunaOrdua = dto.EgunaOrdua;
           
            // Mahaia eguneratu
            erreserba.Mahaia = new Mahaia { Id = dto.MahaiakId };
           
            _repo.Update(erreserba);
            return Ok();
        }
 
 
 
        /// <summary>
        /// Erreserba bat ordaindutzat markatzen du, tiketa sortzen du eta lotutako eskariak garbitzen ditu.
        /// </summary>
        /// <param name="dto">Ordainketaren datuak.</param>
        /// <returns>200 OK edo 404 Not Found.</returns>
        [HttpPost("ordaindu")]
        public IActionResult Ordaindu([FromBody] ErreserbaOrdainduDto dto)
        {
            using var session = _repo.OpenSession();
            using var tx = session.BeginTransaction();
 
            var erreserba = session.Get<Erreserba>(dto.ErreserbaId);
            if (erreserba == null)
                return NotFound();
 
            erreserba.Ordainduta = 1;
            erreserba.PrezioTotala = dto.Guztira;
 
            var produktuak = _repo.LortuProduktuakErreserbarako(dto.ErreserbaId);
 
            string fakturaRuta = SortuTicketPdf(
                erreserba,
                produktuak,
                dto.Jasotakoa,
                dto.Itzulia,
                dto.OrdainketaModua
            );
 
            erreserba.FakturaRuta = fakturaRuta;
 
            session.Update(erreserba);
 
            session.CreateQuery("delete from EskariaProduktua ep where ep.Eskaria.Id in (select e.Id from Eskaria e where e.Erreserba.Id = :id)")
                   .SetInt32("id", dto.ErreserbaId)
                   .ExecuteUpdate();
 
            session.CreateQuery("delete from Eskaria e where e.Erreserba.Id = :id")
                   .SetInt32("id", dto.ErreserbaId)
                   .ExecuteUpdate();
 
            tx.Commit();
 
            return Ok();
        }
 
        private string SortuTicketPdf(
            Erreserba erreserba,
            List<EskariaProduktuaDto> produktuak,
            double jasotakoa,
            double itzulia,
            string ordainketaModua)
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tiketak");
            Directory.CreateDirectory(root);
 
            string izenFitxategia = $"Tiket_Erreserba_{erreserba.Id}.pdf";
            string pdfPath = Path.Combine(root, izenFitxategia);
            string fakturaRuta = $"/tiketak/{izenFitxategia}";
 
            float ticketWidth = 162;
            float tempHeight = 10000;
            float finalHeight = 842;
 
            using (var ms = new MemoryStream())
            using (var writer = new PdfWriter(ms))
            using (var pdf = new PdfDocument(writer))
            using (var doc = new Document(pdf, new iText.Kernel.Geom.PageSize(ticketWidth, tempHeight)))
            {
                doc.SetMargins(10, 5, 5, 5);
                GehituTicketEdukia(doc, erreserba, produktuak, jasotakoa, itzulia, ordainketaModua);
               
                var renderer = doc.GetRenderer();
                if (renderer != null && renderer.GetCurrentArea() != null)
                {
                    float currentY = renderer.GetCurrentArea().GetBBox().GetTop();
                    finalHeight = (tempHeight - currentY) + 5;
                }
            }
 
            using (var writer = new PdfWriter(pdfPath))
            using (var pdf = new PdfDocument(writer))
            {
                var ticketSize = new iText.Kernel.Geom.PageSize(ticketWidth, finalHeight);
               
                using (var doc = new Document(pdf, ticketSize))
                {
                    doc.SetMargins(10, 5, 5, 5);
                    GehituTicketEdukia(doc, erreserba, produktuak, jasotakoa, itzulia, ordainketaModua);
                }
            }
 
            return fakturaRuta;
        }
 
        private void GehituTicketEdukia(
            Document doc,
            Erreserba erreserba,
            List<EskariaProduktuaDto> produktuak,
            double jasotakoa,
            double itzulia,
            string ordainketaModua)
        {
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
 
            doc.Add(new Paragraph("TXAPELA JATETXEA")
                .SetFont(boldFont)
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(0));
 
            doc.Add(new Paragraph("CIF: B12345678\nLazkao\nTel: 943 623 081")
                .SetFont(regularFont)
                .SetFontSize(8)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(5));
 
            doc.Add(new Paragraph($"Tiket: {erreserba.Id}\nData: {DateTime.Now:dd/MM/yyyy HH:mm}\nLangilea: {erreserba.Langilea.Izena}")
                .SetFont(regularFont)
                .SetFontSize(8)
                .SetMarginBottom(5));
               
            doc.Add(new LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.DashedLine()).SetMarginTop(5).SetMarginBottom(5));
 
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 4, 2, 2 })).UseAllAvailableWidth();
            table.SetFontSize(8);
           
            table.AddHeaderCell(new Cell().Add(new Paragraph("Kop").SetFont(boldFont)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Prod").SetFont(boldFont)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Ud.").SetFont(boldFont)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
            table.AddHeaderCell(new Cell().Add(new Paragraph("Tot").SetFont(boldFont)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
 
            foreach (var p in produktuak)
            {
                double lineTotal = p.Kantitatea * p.Prezioa;
                table.AddCell(new Cell().Add(new Paragraph(p.Kantitatea.ToString())).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(p.ProduktuaIzena)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph($"{p.Prezioa:0.00}")).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
                table.AddCell(new Cell().Add(new Paragraph($"{lineTotal:0.00}")).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));
            }
 
            doc.Add(table);
           
            doc.Add(new LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.DashedLine()).SetMarginTop(5).SetMarginBottom(5));
 
            double subtotala = produktuak.Sum(p => p.Kantitatea * p.Prezioa);
            double iva = subtotala * 0.10;
            double guztira = subtotala + iva;
 
            doc.Add(new Paragraph($"Subtotala: {subtotala:0.00}\nIVA (10%): {iva:0.00}")
                .SetFont(regularFont)
                .SetFontSize(8)
                .SetTextAlignment(TextAlignment.RIGHT));
               
            doc.Add(new Paragraph($"GUZTIRA: {guztira:0.00} €")
                .SetFont(boldFont)
                .SetFontSize(12)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetMarginTop(2));
 
            doc.Add(new LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.DashedLine()).SetMarginTop(5).SetMarginBottom(5));
 
            doc.Add(new Paragraph($"Modua: {ordainketaModua}")
                .SetFont(regularFont)
                .SetFontSize(8));
 
            if (ordainketaModua == "Eskudirua")
            {
                doc.Add(new Paragraph($"Jasotakoa: {jasotakoa:0.00}\nItzulia: {itzulia:0.00}")
                    .SetFont(regularFont)
                    .SetFontSize(8));
            }
 
            doc.Add(new Paragraph("\nEskerrik asko!")
                .SetFont(boldFont)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER));
        }
 
        /// <summary>
        /// Erreserba bat ezabatzen du eta stockak leheneratzen ditu, lotutako eskariak ezabatuz.
        /// </summary>
        /// <param name="id">Erreserbaren identifikatzailea.</param>
        /// <returns>200 OK (true) edo 400 Bad Request.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _repo.ExecuteSerializableTransaction(() =>
                {
                    var erreserba = _repo.Get(id);
                    if (erreserba == null) throw new Exception("Erreserba ez da aurkitu");
 
                    var eskariak = _eskariaRepo.GetAll().Where(e => e.Erreserba.Id == id).ToList();
 
                    foreach (var eskaria in eskariak)
                    {
                        foreach (var p in eskaria.Produktuak)
                        {
                            var produktua = _produktuaRepo.Get(p.Produktua.Id);
                            if (produktua != null)
                            {
                                produktua.Stock += p.Kantitatea;
                                _produktuaRepo.Update(produktua);
 
                                var osagaiak = _produktuaOsagaiaRepo.GetByProduktuaId(produktua.Id);
                                foreach (var po in osagaiak)
                                {
                                    po.Osagaia.Stock += po.Kantitatea * p.Kantitatea;
                                    _osagaiaRepo.Update(po.Osagaia);
                                }
                            }
                        }
 
                        _eskariaRepo.Delete(eskaria);
                    }
 
                    _repo.Delete(erreserba);
                });
 
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
 
        /// <summary>
        /// Erreserba baten tiketa (PDF) deskargatu edo bistaratzen du.
        /// </summary>
        /// <param name="id">Erreserbaren identifikatzailea.</param>
        /// <returns>PDF fitxategia duen 200 OK edo 404 Not Found.</returns>
        [HttpGet("tiket/{id}")]
        public IActionResult DeskargatuTicket(int id)
        {
            var erreserba = _repo.GetAll().FirstOrDefault(e => e.Id == id);
            if (erreserba == null || string.IsNullOrEmpty(erreserba.FakturaRuta))
                return NotFound("Ez da tiketa aurkitu.");
 
            string rutaAbsoluta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", erreserba.FakturaRuta.TrimStart('/'));
 
            if (!System.IO.File.Exists(rutaAbsoluta))
                return NotFound("PDF fitxategia ez da existitzen.");
 
            var fileBytes = System.IO.File.ReadAllBytes(rutaAbsoluta);
           
            string izenFitxategia = $"Tiket_Erreserba_{id}.pdf";
            Response.Headers["Content-Disposition"] = $"inline; filename={izenFitxategia}";
           
            return File(fileBytes, "application/pdf");
        }
    }
}