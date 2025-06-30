using AutoMapper;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using XWY.WebAPI.Business.DTOs;
using XWY.WebAPI.DataAccess.UnitOfWork;

namespace XWY.WebAPI.Business.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReporteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseDto<byte[]>> GenerarReporteArticulosPdfAsync(ReporteParametrosDto parametros)
        {
            try
            {
                var datosReporte = await ObtenerDatosReporteArticulosAsync(parametros);
                if (!datosReporte.Success)
                {
                    return new ResponseDto<byte[]>(datosReporte.Message);
                }

                using var memoryStream = new MemoryStream();
                var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                var writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

                var title = new Paragraph("REPORTE DE ARTÍCULOS - INVENTARIO XWY", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                var dateInfo = new Paragraph($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}", cellFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 15
                };
                document.Add(dateInfo);

                var table = new PdfPTable(8) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 15f, 25f, 20f, 15f, 15f, 15f, 10f, 15f });

                var headers = new[] { "Código", "Nombre", "Equipo", "Categoría", "Estado", "Ubicación", "Stock", "Precio" };
                foreach (var header in headers)
                {
                    var cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = BaseColor.DARK_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 8
                    };
                    table.AddCell(cell);
                }

                foreach (var articulo in datosReporte.Data)
                {
                    table.AddCell(new PdfPCell(new Phrase(articulo.Codigo, cellFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(articulo.Nombre, cellFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(articulo.Equipo, cellFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(articulo.Categoria, cellFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(articulo.Estado, cellFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(articulo.Ubicacion ?? "", cellFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(articulo.Stock.ToString(), cellFont))
                    {
                        Padding = 5,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });
                    table.AddCell(new PdfPCell(new Phrase(articulo.Precio?.ToString("C") ?? "N/A", cellFont))
                    {
                        Padding = 5,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    });
                }

                document.Add(table);

                var summary = new Paragraph($"\nTotal de artículos: {datosReporte.Data.Count}", cellFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingBefore = 15
                };
                document.Add(summary);

                document.Close();

                return new ResponseDto<byte[]>(memoryStream.ToArray(), "Reporte PDF generado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<byte[]>($"Error al generar reporte PDF: {ex.Message}");
            }
        }

        public async Task<ResponseDto<byte[]>> GenerarReporteArticulosExcelAsync(ReporteParametrosDto parametros)
        {
            try
            {
                var datosReporte = await ObtenerDatosReporteArticulosAsync(parametros);
                if (!datosReporte.Success)
                {
                    return new ResponseDto<byte[]>(datosReporte.Message);
                }

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Artículos");

                var titleRange = worksheet.Range("A1:H1");
                titleRange.Merge();
                titleRange.Value = "REPORTE DE ARTÍCULOS - INVENTARIO XWY";
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Font.FontSize = 16;
                titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var dateRange = worksheet.Range("A2:H2");
                dateRange.Merge();
                dateRange.Value = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}";
                dateRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                var headers = new[] { "Código", "Nombre", "Equipo", "Categoría", "Estado", "Ubicación", "Stock", "Precio" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(4, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.DarkGray;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                int row = 5;
                foreach (var articulo in datosReporte.Data)
                {
                    worksheet.Cell(row, 1).Value = articulo.Codigo;
                    worksheet.Cell(row, 2).Value = articulo.Nombre;
                    worksheet.Cell(row, 3).Value = articulo.Equipo;
                    worksheet.Cell(row, 4).Value = articulo.Categoria;
                    worksheet.Cell(row, 5).Value = articulo.Estado;
                    worksheet.Cell(row, 6).Value = articulo.Ubicacion ?? "";
                    worksheet.Cell(row, 7).Value = articulo.Stock;
                    worksheet.Cell(row, 8).Value = articulo.Precio ?? 0;
                    row++;
                }

                worksheet.Column(7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Column(8).Style.NumberFormat.Format = "$#,##0.00";
                worksheet.Column(8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                worksheet.Columns().AdjustToContents();

                var summaryCell = worksheet.Cell(row + 1, 1);
                summaryCell.Value = $"Total de artículos: {datosReporte.Data.Count}";
                summaryCell.Style.Font.Bold = true;

                using var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);

                return new ResponseDto<byte[]>(memoryStream.ToArray(), "Reporte Excel generado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<byte[]>($"Error al generar reporte Excel: {ex.Message}");
            }
        }

        public async Task<ResponseDto<byte[]>> GenerarReportePrestamosPdfAsync(ReporteParametrosDto parametros)
        {
            try
            {
                var datosReporte = await ObtenerDatosReportePrestamosAsync(parametros);
                if (!datosReporte.Success)
                {
                    return new ResponseDto<byte[]>(datosReporte.Message);
                }

                using var memoryStream = new MemoryStream();
                var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                var writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);

                var title = new Paragraph("REPORTE DE PRÉSTAMOS - INVENTARIO XWY", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                var dateInfo = new Paragraph($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}", cellFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 15
                };
                document.Add(dateInfo);

                var table = new PdfPTable(8) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 15f, 20f, 20f, 15f, 15f, 15f, 15f, 20f });

                var headers = new[] { "Código Art.", "Artículo", "Usuario", "F. Solicitud", "F. Entrega", "F. Devolución", "Estado", "Aprobado Por" };
                foreach (var header in headers)
                {
                    var cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = BaseColor.DARK_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 6
                    };
                    table.AddCell(cell);
                }

                foreach (var prestamo in datosReporte.Data)
                {
                    table.AddCell(new PdfPCell(new Phrase(prestamo.CodigoArticulo, cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(prestamo.NombreArticulo, cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(prestamo.UsuarioNombre, cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(prestamo.FechaSolicitud.ToString("dd/MM/yyyy"), cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(prestamo.FechaEntregaReal?.ToString("dd/MM/yyyy") ?? "N/A", cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(prestamo.FechaDevolucionReal?.ToString("dd/MM/yyyy") ?? "N/A", cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(prestamo.EstadoPrestamo, cellFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(prestamo.AprobadoPor ?? "N/A", cellFont)) { Padding = 4 });
                }

                document.Add(table);

                var summary = new Paragraph($"\nTotal de préstamos: {datosReporte.Data.Count}", cellFont)
                {
                    Alignment = Element.ALIGN_LEFT,
                    SpacingBefore = 15
                };
                document.Add(summary);

                document.Close();

                return new ResponseDto<byte[]>(memoryStream.ToArray(), "Reporte PDF generado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<byte[]>($"Error al generar reporte PDF: {ex.Message}");
            }
        }

        public async Task<ResponseDto<byte[]>> GenerarReportePrestamosExcelAsync(ReporteParametrosDto parametros)
        {
            try
            {
                var datosReporte = await ObtenerDatosReportePrestamosAsync(parametros);
                if (!datosReporte.Success)
                {
                    return new ResponseDto<byte[]>(datosReporte.Message);
                }

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Préstamos");

                var titleRange = worksheet.Range("A1:H1");
                titleRange.Merge();
                titleRange.Value = "REPORTE DE PRÉSTAMOS - INVENTARIO XWY";
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Font.FontSize = 16;
                titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var dateRange = worksheet.Range("A2:H2");
                dateRange.Merge();
                dateRange.Value = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}";
                dateRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                var headers = new[] { "Código Artículo", "Artículo", "Usuario", "Email", "F. Solicitud", "F. Entrega", "F. Devolución", "Estado" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(4, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.DarkGray;
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                int row = 5;
                foreach (var prestamo in datosReporte.Data)
                {
                    worksheet.Cell(row, 1).Value = prestamo.CodigoArticulo;
                    worksheet.Cell(row, 2).Value = prestamo.NombreArticulo;
                    worksheet.Cell(row, 3).Value = prestamo.UsuarioNombre;
                    worksheet.Cell(row, 4).Value = prestamo.UsuarioEmail;
                    worksheet.Cell(row, 5).Value = prestamo.FechaSolicitud.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 6).Value = prestamo.FechaEntregaReal?.ToString("dd/MM/yyyy") ?? "N/A";
                    worksheet.Cell(row, 7).Value = prestamo.FechaDevolucionReal?.ToString("dd/MM/yyyy") ?? "N/A";
                    worksheet.Cell(row, 8).Value = prestamo.EstadoPrestamo;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                var summaryCell = worksheet.Cell(row + 1, 1);
                summaryCell.Value = $"Total de préstamos: {datosReporte.Data.Count}";
                summaryCell.Style.Font.Bold = true;

                using var memoryStream = new MemoryStream();
                workbook.SaveAs(memoryStream);

                return new ResponseDto<byte[]>(memoryStream.ToArray(), "Reporte Excel generado exitosamente");
            }
            catch (Exception ex)
            {
                return new ResponseDto<byte[]>($"Error al generar reporte Excel: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<ReporteArticulosDto>>> ObtenerDatosReporteArticulosAsync(ReporteParametrosDto parametros)
        {
            try
            {
                var articulos = await _unitOfWork.Articulos.GetFilteredAsync(
                    parametros.CategoriaId,
                    parametros.EstadoArticuloId,
                    null);

                var reporteData = articulos.Select(a => new ReporteArticulosDto
                {
                    Codigo = a.Codigo,
                    Nombre = a.Nombre,
                    Equipo = a.Equipo,
                    Categoria = a.Categoria.Nombre,
                    Estado = a.EstadoArticulo.Nombre,
                    Ubicacion = a.Ubicacion,
                    Stock = a.Stock,
                    Precio = a.Precio
                }).ToList();

                return new ResponseDto<List<ReporteArticulosDto>>(reporteData);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ReporteArticulosDto>>($"Error al obtener datos de artículos: {ex.Message}");
            }
        }

        public async Task<ResponseDto<List<ReportePrestamosDto>>> ObtenerDatosReportePrestamosAsync(ReporteParametrosDto parametros)
        {
            try
            {
                var prestamos = await _unitOfWork.Prestamos.GetFilteredAsync(
                    null,
                    null,
                    parametros.EstadoPrestamoId,
                    parametros.FechaDesde,
                    parametros.FechaHasta);

                var reporteData = prestamos.Select(p => new ReportePrestamosDto
                {
                    CodigoArticulo = p.Articulo.Codigo,
                    NombreArticulo = p.Articulo.Nombre,
                    UsuarioNombre = $"{p.Usuario.Nombres} {p.Usuario.Apellidos}",
                    UsuarioEmail = p.Usuario.Email,
                    FechaSolicitud = p.FechaSolicitud,
                    FechaEntregaEstimada = p.FechaEntregaEstimada,
                    FechaEntregaReal = p.FechaEntregaReal,
                    FechaDevolucionReal = p.FechaDevolucionReal,
                    EstadoPrestamo = p.EstadoPrestamo.Nombre,
                    AprobadoPor = p.UsuarioAprobador != null ? $"{p.UsuarioAprobador.Nombres} {p.UsuarioAprobador.Apellidos}" : null
                }).ToList();

                return new ResponseDto<List<ReportePrestamosDto>>(reporteData);
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<ReportePrestamosDto>>($"Error al obtener datos de préstamos: {ex.Message}");
            }
        }

        public async Task<ResponseDto<byte[]>> GenerarReportePersonalizadoAsync(ReporteParametrosDto parametros)
        {
            try
            {
                if (parametros.TipoReporte?.ToLower() == "articulos")
                {
                    return parametros.Formato?.ToLower() == "excel"
                        ? await GenerarReporteArticulosExcelAsync(parametros)
                        : await GenerarReporteArticulosPdfAsync(parametros);
                }
                else if (parametros.TipoReporte?.ToLower() == "prestamos")
                {
                    return parametros.Formato?.ToLower() == "excel"
                        ? await GenerarReportePrestamosExcelAsync(parametros)
                        : await GenerarReportePrestamosPdfAsync(parametros);
                }
                else
                {
                    return new ResponseDto<byte[]>("Tipo de reporte no válido");
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<byte[]>($"Error al generar reporte personalizado: {ex.Message}");
            }
        }
    }
}
