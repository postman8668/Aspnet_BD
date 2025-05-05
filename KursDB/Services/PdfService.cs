using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using KursDB.Models.ViewModels;

namespace KursDB.Services
{
    public class PdfService
    {
        private readonly BaseFont _russianFont;

        public PdfService(IWebHostEnvironment env)
        {
            // Регистрация провайдера кодировок (необходимо для .NET Core)
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Путь к шрифту Arial в системе
            var fontPath = Path.Combine(env.WebRootPath, "fonts", "ARIAL.TTF");

            // Альтернативный вариант - использование шрифта из ресурсов
            // var fontPath = GetFontFromEmbeddedResources("arial.ttf");

            _russianFont = BaseFont.CreateFont(
                fontPath,
                BaseFont.IDENTITY_H,
                BaseFont.EMBEDDED
            );
        }

        // Метод для получения шрифта из встроенных ресурсов (альтернативный вариант)
        private byte[] GetFontFromEmbeddedResources(string fontName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"YourNamespace.Resources.Fonts.{fontName}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Шрифт {fontName} не найден в ресурсах");

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        private Font GetRussianFont(float size, int style = Font.NORMAL)
        {
            return new Font(_russianFont, size, style);
        }

        public byte[] GenerateOverdueLoansPdf(List<OverdueLoanViewModel> loans)
        {
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Заголовок
                var title = new Paragraph("Отчет о просроченных выдачах",
                    GetRussianFont(16, Font.BOLD));
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                document.Add(new Paragraph($"Дата формирования: {DateTime.Now:dd.MM.yyyy}",
                    GetRussianFont(10)));
                document.Add(new Paragraph("\n"));

                // Таблица
                var table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 3, 3, 2, 2, 2 });

                // Заголовки таблицы
                table.AddCell(CreateHeaderCell("Читатель"));
                table.AddCell(CreateHeaderCell("Издание"));
                table.AddCell(CreateHeaderCell("Дата выдачи"));
                table.AddCell(CreateHeaderCell("Срок возврата"));
                table.AddCell(CreateHeaderCell("Дней просрочки"));

                // Данные
                foreach (var loan in loans)
                {
                    table.AddCell(new Phrase(loan.ReaderName, GetRussianFont(10)));
                    table.AddCell(new Phrase(loan.PublicationTitle, GetRussianFont(10)));
                    table.AddCell(new Phrase(loan.LoanDate.ToString("dd.MM.yyyy"), GetRussianFont(10)));
                    table.AddCell(new Phrase(loan.DueDate.ToString("dd.MM.yyyy"), GetRussianFont(10)));
                    table.AddCell(new Phrase(loan.DaysOverdue.ToString(), GetRussianFont(10)));
                }

                document.Add(table);
                document.Close();

                return ms.ToArray();
            }
        }

        public byte[] GenerateAuthorBooksPdf(List<AuthorBooksInReadingRoomViewModel> books)
        {
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Заголовок
                var title = new Paragraph("Справка о книгах автора в читальном зале",
                    GetRussianFont(16, Font.BOLD));
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                document.Add(new Paragraph($"Дата формирования: {DateTime.Now:dd.MM.yyyy}",
                    GetRussianFont(10)));
                document.Add(new Paragraph("\n"));

                // Таблица
                var table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 3, 2, 1, 2 });

                // Заголовки таблицы
                table.AddCell(CreateHeaderCell("Автор"));
                table.AddCell(CreateHeaderCell("Название издания"));
                table.AddCell(CreateHeaderCell("Библиотека"));
                table.AddCell(CreateHeaderCell("Зал"));
                table.AddCell(CreateHeaderCell("Доступно экз."));

                // Данные
                foreach (var book in books)
                {
                    table.AddCell(new Phrase(book.AuthorName, GetRussianFont(10)));
                    table.AddCell(new Phrase(book.PublicationTitle, GetRussianFont(10)));
                    table.AddCell(new Phrase(book.LibraryName, GetRussianFont(10)));
                    table.AddCell(new Phrase(book.HallNumber.ToString(), GetRussianFont(10)));
                    table.AddCell(new Phrase(book.AvailableCopies.ToString(), GetRussianFont(10)));
                }

                document.Add(table);
                document.Close();

                return ms.ToArray();
            }
        }

        public byte[] GenerateLibraryStatisticsPdf(LibraryStatisticsViewModel stats, int month, int year)
        {
            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                // Заголовок
                var title = new Paragraph($"Отчет о работе библиотеки за {month}/{year} год",
                    GetRussianFont(16, Font.BOLD));
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                document.Add(new Paragraph($"Библиотека: {stats.LibraryName}",
                    GetRussianFont(10)));
                document.Add(new Paragraph($"Дата формирования: {DateTime.Now:dd.MM.yyyy}",
                    GetRussianFont(10)));
                document.Add(new Paragraph("\n"));

                // Основная статистика
                var statsTable = new PdfPTable(2);
                statsTable.WidthPercentage = 100;
                statsTable.SetWidths(new float[] { 3, 2 });

                AddStatRow(statsTable, "Общее количество читателей", stats.TotalReaders.ToString());
                AddStatRow(statsTable, "Новых читателей за месяц", stats.NewReadersThisMonth.ToString());
                AddStatRow(statsTable, "Всего выдач за месяц", stats.TotalLoansThisMonth.ToString());
                AddStatRow(statsTable, "Просроченных выдач", stats.OverdueLoans.ToString());

                document.Add(statsTable);
                document.Close();

                return ms.ToArray();
            }
        }

        private PdfPCell CreateHeaderCell(string text)
        {
            return new PdfPCell(new Phrase(text, GetRussianFont(10, Font.BOLD)))
            {
                BackgroundColor = new BaseColor(220, 220, 220),
                Padding = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
        }

        private void AddStatRow(PdfPTable table, string label, string value)
        {
            table.AddCell(new Phrase(label, GetRussianFont(10, Font.BOLD)));
            table.AddCell(new Phrase(value, GetRussianFont(10)));
        }
    }
}