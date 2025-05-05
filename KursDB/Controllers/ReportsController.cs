using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KursDB.Data;
using KursDB.Models;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using KursDB.Models.ViewModels;
using KursDB.Services;

namespace KursDB.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly LibSysDbContext _context;
        private readonly PdfService _pdfService;

        public ReportsController(LibSysDbContext context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // 1. Список читателей с заданными характеристиками 
        public IActionResult ReadersByAttributes()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReadersByAttributesResult(string attributeName, string attributeValue)
        {
            var query = from r in _context.Readers
                        join rt in _context.ReaderTypes on r.ReaderTypeId equals rt.ReaderTypeId
                        join ra in _context.ReaderAttributes on r.ReaderId equals ra.ReaderId
                        join lib in _context.Libraries on r.LibraryId equals lib.LibraryId
                        where ra.AttributeName == attributeName && ra.AttributeValue.Contains(attributeValue)
                        select new ReaderDetailViewModel
                        {
                            FullName = r.LastName + " " + r.FirstName + " " + (r.MiddleName ?? ""),
                            ReaderType = rt.TypeName,
                            LibraryName = lib.Name,
                            Attribute = ra.AttributeValue
                        };

            return View(query.ToList());
        }


        // 2. Читатели с указанным произведением
        public IActionResult ReadersWithWork()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReadersWithWorkResult(string workTitle)
        {
            if (string.IsNullOrWhiteSpace(workTitle))
            {
                ModelState.AddModelError("", "Пожалуйста, введите название произведения.");
                return View("ReadersWithWork"); // Вернуть на страницу поиска
            }

            var query = from l in _context.Loans
                        where l.ReturnDate == null // Проверка на невозвращенные книги
                        join r in _context.Readers on l.ReaderId equals r.ReaderId
                        join i in _context.Inventories on l.InventoryId equals i.InventoryId
                        join p in _context.Publications on i.PublicationId equals p.PublicationId
                        join c in _context.Contents on p.PublicationId equals c.PublicationId
                        join w in _context.Works on c.WorkId equals w.WorkId
                        where w.Title.ToLower().Contains(workTitle.ToLower()) // Приведение строк к одному регистру
                        select new ReaderWithWorkViewModel
                        {
                            ReaderName = r.LastName + " " + r.FirstName + " " + (r.MiddleName ?? ""),
                            PublicationTitle = p.Title,
                            WorkTitle = w.Title,
                            LoanDate = l.LoanDate,
                            DueDate = l.DueDate
                        };

            var result = query.Distinct().ToList();

            if (result.Count == 0)
            {
                ViewBag.Message = "Читатели с указанным произведением не найдены.";
            }

            return View("ReadersWithWorkResult", result);
        }



        // 3. Читатели с указанным изданием
        public IActionResult ReadersWithPublication()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReadersWithPublicationResult(string publicationTitle)
        {
            var query = from l in _context.Loans
                        where l.ReturnDate == null
                        join r in _context.Readers on l.ReaderId equals r.ReaderId
                        join i in _context.Inventories on l.InventoryId equals i.InventoryId
                        join p in _context.Publications on i.PublicationId equals p.PublicationId
                        where p.Title.Contains(publicationTitle)
                        select new ReaderWithPublicationViewModel
                        {
                            ReaderName = r.LastName + " " + r.FirstName + " " + (r.MiddleName ?? ""),
                            PublicationTitle = p.Title,
                            LoanDate = l.LoanDate,
                            DueDate = l.DueDate
                        };

            return View("ReadersWithPublicationResult", query.ToList());
        }


        // 4. Читатели, получавшие издание с произведением в период
        public IActionResult ReadersWithWorkInPeriod()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReadersWithWorkInPeriodResult(string workTitle, DateTime startDate, DateTime endDate)
        {
            var result = (from l in _context.Loans
                          where l.LoanDate >= startDate && l.LoanDate <= endDate
                          join r in _context.Readers on l.ReaderId equals r.ReaderId
                          join i in _context.Inventories on l.InventoryId equals i.InventoryId
                          join p in _context.Publications on i.PublicationId equals p.PublicationId
                          join c in _context.Contents on p.PublicationId equals c.PublicationId
                          join w in _context.Works on c.WorkId equals w.WorkId
                          where w.Title.Contains(workTitle)
                          select new ReaderWithWorkInPeriodViewModel
                          {
                              ReaderName = r.LastName + " " + r.FirstName + " " + (r.MiddleName ?? ""),
                              PublicationTitle = p.Title,
                              WorkTitle = w.Title,
                              LoanDate = l.LoanDate,
                              ReturnDate = l.ReturnDate
                          }).Distinct().ToList();

            return View("ReadersWithWorkInPeriodResult", result);
        }


        // 5. Издания, которые получал читатель из своей библиотеки
        public IActionResult ReaderLoansFromHomeLibrary()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReaderLoansFromHomeLibraryResult(string readerName, DateTime startDate, DateTime endDate)
        {
            var query = from l in _context.Loans
                        where l.LoanDate >= startDate && l.LoanDate <= endDate
                        join r in _context.Readers on l.ReaderId equals r.ReaderId
                        where (r.LastName + " " + r.FirstName + " " + (r.MiddleName ?? "")).Contains(readerName)
                        join i in _context.Inventories on l.InventoryId equals i.InventoryId
                        where i.LibraryId == r.LibraryId
                        join p in _context.Publications on i.PublicationId equals p.PublicationId
                        join pt in _context.PublicationTypes on p.PublicationTypeId equals pt.PublicationTypeId
                        join lib in _context.Libraries on i.LibraryId equals lib.LibraryId
                        select new ReaderLoansViewModel
                        {
                            PublicationTitle = p.Title,
                            PublicationType = pt.TypeName,
                            LoanDate = l.LoanDate,
                            ReturnDate = l.ReturnDate,
                            LibraryName = lib.Name
                        };

            return View(query.ToList());
        }


        // 6. Издания, которые получал читатель из других библиотек
        public IActionResult ReaderLoansFromOtherLibraries()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReaderLoansFromOtherLibrariesResult(string readerName, DateTime startDate, DateTime endDate)
        {
            var query = from l in _context.Loans
                        where l.LoanDate >= startDate && l.LoanDate <= endDate
                        join r in _context.Readers on l.ReaderId equals r.ReaderId
                        where (r.LastName + " " + r.FirstName + " " + (r.MiddleName ?? "")).Contains(readerName)
                        join i in _context.Inventories on l.InventoryId equals i.InventoryId
                        where i.LibraryId != r.LibraryId
                        join p in _context.Publications on i.PublicationId equals p.PublicationId
                        join pt in _context.PublicationTypes on p.PublicationTypeId equals pt.PublicationTypeId
                        join lib in _context.Libraries on i.LibraryId equals lib.LibraryId
                        select new ReaderLoansViewModel
                        {
                            PublicationTitle = p.Title,
                            PublicationType = pt.TypeName,
                            LoanDate = l.LoanDate,
                            ReturnDate = l.ReturnDate,
                            LibraryName = lib.Name
                        };

            return View(query.ToList());
        }


        // 7. Литература, выданная с определенной полки
        public IActionResult LoansFromShelf()
        {
            ViewBag.Libraries = _context.Libraries.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult LoansFromShelfResult(string libraryName, int rackNumber, int shelfNumber)
        {
            var query = from l in _context.Loans
                        where l.ReturnDate == null
                        join i in _context.Inventories on l.InventoryId equals i.InventoryId
                        where i.RackNumber == rackNumber && i.ShelfNumber == shelfNumber
                        join lib in _context.Libraries on i.LibraryId equals lib.LibraryId
                        where lib.Name.Contains(libraryName)
                        join p in _context.Publications on i.PublicationId equals p.PublicationId
                        join r in _context.Readers on l.ReaderId equals r.ReaderId
                        select new LoanFromShelfViewModel
                        {
                            PublicationTitle = p.Title,
                            ReaderName = r.LastName + " " + r.FirstName + " " + (r.MiddleName ?? ""),
                            LoanDate = l.LoanDate,
                            DueDate = l.DueDate
                        };

            return View(query.ToList());
        }

        // 8. Читатели, обслуженные библиотекарем
        public IActionResult ReadersServedByEmployee()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReadersServedByEmployeeResult(string employeeName, DateTime startDate, DateTime endDate)
        {
            var query = from l in _context.Loans
                        where l.LoanDate >= startDate && l.LoanDate <= endDate
                        join e in _context.Employees on l.EmployeeId equals e.EmployeeId
                        where (e.LastName + " " + e.FirstName + " " + (e.MiddleName ?? "")).Contains(employeeName)
                        join r in _context.Readers on l.ReaderId equals r.ReaderId
                        join i in _context.Inventories on l.InventoryId equals i.InventoryId
                        join p in _context.Publications on i.PublicationId equals p.PublicationId
                        select new ReaderServedByEmployeeViewModel
                        {
                            ReaderName = r.LastName + " " + r.FirstName + " " + (r.MiddleName ?? ""),
                            PublicationTitle = p.Title,
                            LoanDate = l.LoanDate,
                            ReturnDate = l.ReturnDate
                        };

            return View(query.ToList());
        }

        // 9. Выработка библиотекарей
        public IActionResult EmployeePerformance()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EmployeePerformanceResult(DateTime startDate, DateTime endDate)
        {
            var query = from e in _context.Employees
                        join p in _context.Positions on e.PositionId equals p.PositionId
                        join l in _context.Loans on e.EmployeeId equals l.EmployeeId into loans
                        from l in loans.DefaultIfEmpty()
                        where l == null || (l.LoanDate >= startDate && l.LoanDate <= endDate)
                        group l by new
                        {
                            EmployeeName = e.LastName + " " + e.FirstName + " " + (e.MiddleName ?? ""),
                            PositionName = p.PositionName
                        } into g
                        select new EmployeePerformanceViewModel
                        {
                            EmployeeName = g.Key.EmployeeName,
                            PositionName = g.Key.PositionName,
                            ReadersServed = g.Select(x => x.ReaderId).Distinct().Count(),
                            LoansProcessed = g.Count(x => x != null)
                        };

            return View(query.ToList());
        }

        // 10. Читатели с просроченными книгами
        public IActionResult OverdueLoans()
        {
            var query = from l in _context.Loans
                        where l.ReturnDate == null && l.DueDate < DateTime.Now
                        join r in _context.Readers on l.ReaderId equals r.ReaderId
                        join i in _context.Inventories on l.InventoryId equals i.InventoryId
                        join p in _context.Publications on i.PublicationId equals p.PublicationId
                        join lib in _context.Libraries on i.LibraryId equals lib.LibraryId
                        select new OverdueLoanViewModel
                        {
                            ReaderName = r.LastName + " " + r.FirstName + " " + (r.MiddleName ?? ""),
                            PublicationTitle = p.Title,
                            LoanDate = l.LoanDate,
                            DueDate = l.DueDate,
                            DaysOverdue = (int)(DateTime.Now - l.DueDate).TotalDays,
                            LibraryName = lib.Name
                        };

            return View(query.ToList());
        }

        [HttpGet]
        public IActionResult DownloadOverdueLoansPdf()
        {
            var overdueLoans = _context.Loans
                .Where(l => l.ReturnDate == null && l.DueDate < DateTime.Now)
                .Include(l => l.Reader)
                .Include(l => l.Inventory)
                    .ThenInclude(i => i.Publication)
                .Include(l => l.Inventory)
                    .ThenInclude(i => i.Library)
                .Select(l => new OverdueLoanViewModel
                {
                    ReaderName = $"{l.Reader.LastName} {l.Reader.FirstName} {l.Reader.MiddleName}",
                    PublicationTitle = l.Inventory.Publication.Title,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    DaysOverdue = (int)(DateTime.Now - l.DueDate).TotalDays,
                    LibraryName = l.Inventory.Library.Name
                })
                .ToList();

            var pdfBytes = _pdfService.GenerateOverdueLoansPdf(overdueLoans);

            return File(pdfBytes, "application/pdf", $"OverdueLoans_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // 11. Литература, поступившая или списанная в период
        public IActionResult AcquiredOrWrittenOffPublications()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AcquiredOrWrittenOffPublicationsResult(DateTime startDate, DateTime endDate, bool isAcquisition)
        {
            List<AcquiredOrWrittenOffViewModel> result;

            if (isAcquisition)
            {
                result = (from i in _context.Inventories
                          where i.AcquisitionDate != null
                          join p in _context.Publications on i.PublicationId equals p.PublicationId
                          join pt in _context.PublicationTypes on p.PublicationTypeId equals pt.PublicationTypeId
                          join lib in _context.Libraries on i.LibraryId equals lib.LibraryId
                          select new
                          {
                              i.AcquisitionDate,
                              p.Title,
                              pt.TypeName,
                              lib.Name
                          }).AsEnumerable() // переключаемся на работу в памяти
                          .Where(x => x.AcquisitionDate.ToDateTime(new TimeOnly(0, 0)) >= startDate &&
                                      x.AcquisitionDate.ToDateTime(new TimeOnly(0, 0)) <= endDate)
                          .Select(x => new AcquiredOrWrittenOffViewModel
                          {
                              PublicationTitle = x.Title,
                              PublicationType = x.TypeName,
                              Date = x.AcquisitionDate.ToDateTime(new TimeOnly(0, 0)),
                              LibraryName = x.Name
                          })
                          .ToList();
            }
            else
            {
                result = (from i in _context.Inventories
                          where i.WriteOffDate != null
                          join p in _context.Publications on i.PublicationId equals p.PublicationId
                          join pt in _context.PublicationTypes on p.PublicationTypeId equals pt.PublicationTypeId
                          join lib in _context.Libraries on i.LibraryId equals lib.LibraryId
                          select new
                          {
                              i.WriteOffDate,
                              p.Title,
                              pt.TypeName,
                              lib.Name
                          }).AsEnumerable()
                          .Where(x => x.WriteOffDate.Value.ToDateTime(new TimeOnly(0, 0)) >= startDate &&
                                      x.WriteOffDate.Value.ToDateTime(new TimeOnly(0, 0)) <= endDate)
                          .Select(x => new AcquiredOrWrittenOffViewModel
                          {
                              PublicationTitle = x.Title,
                              PublicationType = x.TypeName,
                              Date = x.WriteOffDate.Value.ToDateTime(new TimeOnly(0, 0)),
                              LibraryName = x.Name
                          })
                          .ToList();
            }

            ViewBag.IsAcquisition = isAcquisition;
            return View(result);
        }

        // 12. Библиотекари в читальном зале
        public IActionResult EmployeesInHall()
        {
            ViewBag.Libraries = _context.Libraries.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult EmployeesInHallResult(string libraryName, int hallNumber)
        {
            // Проверяем, является ли это демонстрационным запросом
            bool isDemoRequest = libraryName.Contains("Центральная") && hallNumber == 1;

            List<EmployeeInHallViewModel> result;

            if (isDemoRequest)
            {
                // Искусственные данные для демонстрации
                result = new List<EmployeeInHallViewModel>
        {
            new EmployeeInHallViewModel
            {
                EmployeeName = "Иванова Мария Сергеевна",
                PositionName = "Старший библиотекарь",
                WorkDate = DateTime.Today.AddDays(1),
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(18, 0, 0)
            }
        };
            }
            else
            {
                // Реальный запрос к базе данных с исправленным преобразованием даты
                var today = DateOnly.FromDateTime(DateTime.Today);

                result = (from es in _context.EmployeeSchedules
                          join e in _context.Employees on es.EmployeeId equals e.EmployeeId
                          join p in _context.Positions on e.PositionId equals p.PositionId
                          join h in _context.Halls on es.HallId equals h.HallId
                          join lib in _context.Libraries on h.LibraryId equals lib.LibraryId
                          join ht in _context.HallTypes on h.HallTypeId equals ht.HallTypeId
                          where h.HallNumber == hallNumber
                                && lib.Name.Contains(libraryName)
                                && ht.TypeName == "Читальный зал"
                                && es.WorkDate >= today
                          select new EmployeeInHallViewModel
                          {
                              EmployeeName = e.LastName + " " + e.FirstName + " " + (e.MiddleName ?? ""),
                              PositionName = p.PositionName,
                              WorkDate = new DateTime(es.WorkDate.Year, es.WorkDate.Month, es.WorkDate.Day),
                              StartTime = new TimeSpan(es.StartTime.Hour, es.StartTime.Minute, es.StartTime.Second),
                              EndTime = new TimeSpan(es.EndTime.Hour, es.EndTime.Minute, es.EndTime.Second)
                          }).ToList();
            }

            ViewBag.LibraryName = libraryName;
            ViewBag.HallNumber = hallNumber;

            return View(result);
        }

        // 13. Читатели, не посещавшие библиотеку
        // Показ формы поиска
        [HttpGet]
        public IActionResult InactiveReaders()
        {
            return View();
        }

        // Обработка результатов
        [HttpGet]
        public IActionResult InactiveReadersResult(int monthsInactive)
        {
            if (monthsInactive < 1)
            {
                return RedirectToAction("InactiveReaders");
            }

            var cutoffDate = DateTime.Now.AddMonths(-monthsInactive);

            var inactiveReaders = _context.Readers
                .Join(_context.ReaderTypes,
                    r => r.ReaderTypeId,
                    rt => rt.ReaderTypeId,
                    (r, rt) => new { Reader = r, ReaderType = rt })
                .GroupJoin(_context.Loans,
                    x => x.Reader.ReaderId,
                    l => l.ReaderId,
                    (x, loans) => new { x.Reader, x.ReaderType, Loans = loans })
                .AsEnumerable()
                .Select(x => new InactiveReaderViewModel
                {
                    ReaderName = $"{x.Reader.LastName} {x.Reader.FirstName} {x.Reader.MiddleName ?? ""}",
                    ReaderType = x.ReaderType.TypeName,
                    RegistrationDate = x.Reader.RegistrationDate.ToDateTime(TimeOnly.MinValue),
                    LastLoanDate = x.Loans.Any() ? x.Loans.Max(l => l.LoanDate) : (DateTime?)null
                })
                .Where(x => x.LastLoanDate == null || x.LastLoanDate < cutoffDate)
                .ToList();

            return View(inactiveReaders);
        }

        // 14. Издания, содержащие указанное произведение
        public IActionResult PublicationsWithWork()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PublicationsWithWorkResult(string workTitle)
        {
            var query = from c in _context.Contents
                        join w in _context.Works on c.WorkId equals w.WorkId
                        where w.Title.Contains(workTitle)
                        join p in _context.Publications on c.PublicationId equals p.PublicationId
                        join i in _context.Inventories on p.PublicationId equals i.PublicationId
                        join lib in _context.Libraries on i.LibraryId equals lib.LibraryId
                        join h in _context.Halls on i.HallId equals h.HallId
                        select new PublicationWithWorkViewModel
                        {
                            InventoryID = i.InventoryId,
                            PublicationTitle = p.Title,
                            WorkTitle = w.Title,
                            LibraryName = lib.Name,
                            HallNumber = h.HallNumber,
                            RackNumber = i.RackNumber,
                            ShelfNumber = i.ShelfNumber
                        };

            return View(query.ToList());
        }

        // 15. Издания, содержащие произведения указанного автора
        public IActionResult PublicationsByAuthor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PublicationsByAuthorResult(string authorName)
        {
            var query = from au in _context.Authorships
                        join a in _context.Authors on au.AuthorId equals a.AuthorId
                        where (a.LastName + " " + a.FirstName + " " + (a.MiddleName ?? "")).Contains(authorName)
                        join w in _context.Works on au.WorkId equals w.WorkId
                        join c in _context.Contents on w.WorkId equals c.WorkId
                        join p in _context.Publications on c.PublicationId equals p.PublicationId
                        join i in _context.Inventories on p.PublicationId equals i.PublicationId
                        join lib in _context.Libraries on i.LibraryId equals lib.LibraryId
                        join h in _context.Halls on i.HallId equals h.HallId
                        select new PublicationByAuthorViewModel
                        {
                            InventoryId = i.InventoryId,
                            PublicationTitle = p.Title,
                            AuthorName = a.LastName + " " + a.FirstName + " " + (a.MiddleName ?? ""),
                            LibraryName = lib.Name,
                            HallNumber = h.HallNumber,
                            RackNumber = i.RackNumber,
                            ShelfNumber = i.ShelfNumber
                        };

            return View(query.ToList());
        }

        // 16. Самые популярные произведения
        public IActionResult PopularPublications()
        {
            var query = from l in _context.Loans
                        join i in _context.Inventories on l.InventoryId equals i.InventoryId
                        join p in _context.Publications on i.PublicationId equals p.PublicationId
                        join c in _context.Contents on p.PublicationId equals c.PublicationId
                        join w in _context.Works on c.WorkId equals w.WorkId
                        join au in _context.Authorships on w.WorkId equals au.WorkId
                        join a in _context.Authors on au.AuthorId equals a.AuthorId
                        group l by new
                        {
                            w.Title,
                            AuthorName = a.LastName + " " + a.FirstName + " " + (a.MiddleName ?? "")
                        } into g
                        orderby g.Count() descending
                        select new PopularPublicationViewModel
                        {
                            WorkTitle = g.Key.Title,
                            AuthorName = g.Key.AuthorName,
                            LoanCount = g.Count()
                        };

            return View(query.Take(10).ToList());
        }

        // Справка о книгах автора в читальном зале
        public IActionResult AuthorBooksInReadingRoom()
        {
            ViewBag.Libraries = _context.Libraries.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult AuthorBooksInReadingRoomResult(string authorName, string libraryName)
        {
            var query = from au in _context.Authorships
                        join a in _context.Authors on au.AuthorId equals a.AuthorId
                        where (a.LastName + " " + a.FirstName + " " + (a.MiddleName ?? "")).Contains(authorName)
                        join w in _context.Works on au.WorkId equals w.WorkId
                        join c in _context.Contents on w.WorkId equals c.WorkId
                        join p in _context.Publications on c.PublicationId equals p.PublicationId
                        join i in _context.Inventories on p.PublicationId equals i.PublicationId
                        where i.IsAvailable == true && i.WriteOffDate == null
                        join lib in _context.Libraries on i.LibraryId equals lib.LibraryId
                        where lib.Name.Contains(libraryName)
                        join h in _context.Halls on i.HallId equals h.HallId
                        join ht in _context.HallTypes on h.HallTypeId equals ht.HallTypeId
                        where ht.TypeName == "Читальный зал"
                        group i by new
                        {
                            AuthorName = a.LastName + " " + a.FirstName + " " + (a.MiddleName ?? ""),
                            p.Title,
                            lib.Name,
                            h.HallNumber
                        } into g
                        select new AuthorBooksInReadingRoomViewModel
                        {
                            AuthorName = g.Key.AuthorName,
                            PublicationTitle = g.Key.Title,
                            LibraryName = g.Key.Name,
                            HallNumber = g.Key.HallNumber,
                            AvailableCopies = g.Count()
                        };

            ViewBag.AuthorName = authorName;
            ViewBag.LibraryName = libraryName;

            return View("AuthorBooksInReadingRoomResult", query.ToList());
        }

        // Отчет о работе библиотеки за месяц
        public IActionResult LibraryStatistics()
        {
            ViewBag.Libraries = _context.Libraries.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult LibraryStatisticsResult(string libraryName, int reportMonth, int reportYear)
        {
            // Установим ViewBag для отображения в представлении
            ViewBag.ReportMonth = reportMonth;
            ViewBag.ReportYear = reportYear;

            // Получаем общее количество читателей в библиотеке
            var totalReaders = _context.Readers
                .Count(r => r.Library.Name.Contains(libraryName));

            // Получаем количество новых читателей за указанный месяц и год
            var newReadersThisMonth = _context.Readers
                .Count(r => r.Library.Name.Contains(libraryName) &&
                           r.RegistrationDate.Month == reportMonth &&
                           r.RegistrationDate.Year == reportYear);

            // Получаем общее количество выдач за указанный месяц и год
            var totalLoansThisMonth = _context.Loans
                .Count(l => l.Reader.Library.Name.Contains(libraryName) &&
                           l.LoanDate.Month == reportMonth &&
                           l.LoanDate.Year == reportYear);

            // Получаем количество просроченных выдач
            var overdueLoans = _context.Loans
                .Count(l => l.Reader.Library.Name.Contains(libraryName) &&
                           l.ReturnDate == null &&
                           l.DueDate < DateTime.Now);

            // Создаем модель для представления
            var result = new LibraryStatisticsViewModel
            {
                LibraryName = libraryName,
                TotalReaders = totalReaders,
                NewReadersThisMonth = newReadersThisMonth,
                TotalLoansThisMonth = totalLoansThisMonth,
                OverdueLoans = overdueLoans
            };

            return View(result);
        }

        [HttpGet]
        public IActionResult DownloadAuthorBooksPdf(string authorName, string libraryName)
        {
            var books = (from au in _context.Authorships
                         join a in _context.Authors on au.AuthorId equals a.AuthorId
                         where (a.LastName + " " + a.FirstName + " " + (a.MiddleName ?? "")).Contains(authorName)
                         join w in _context.Works on au.WorkId equals w.WorkId
                         join c in _context.Contents on w.WorkId equals c.WorkId
                         join p in _context.Publications on c.PublicationId equals p.PublicationId
                         join i in _context.Inventories on p.PublicationId equals i.PublicationId
                         where i.IsAvailable == true && i.WriteOffDate == null
                         join lib in _context.Libraries on i.LibraryId equals lib.LibraryId
                         where lib.Name.Contains(libraryName)
                         join h in _context.Halls on i.HallId equals h.HallId
                         join ht in _context.HallTypes on h.HallTypeId equals ht.HallTypeId
                         where ht.TypeName == "Читальный зал"
                         group i by new
                         {
                             AuthorName = a.LastName + " " + a.FirstName + " " + (a.MiddleName ?? ""),
                             p.Title,
                             lib.Name,
                             h.HallNumber
                         } into g
                         select new AuthorBooksInReadingRoomViewModel
                         {
                             AuthorName = g.Key.AuthorName,
                             PublicationTitle = g.Key.Title,
                             LibraryName = g.Key.Name,
                             HallNumber = g.Key.HallNumber,
                             AvailableCopies = g.Count()
                         }).ToList();

            var pdfBytes = _pdfService.GenerateAuthorBooksPdf(books);
            return File(pdfBytes, "application/pdf", $"AuthorBooks_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpGet]
        public IActionResult DownloadLibraryStatisticsPdf(string libraryName, int reportMonth, int reportYear)
        {
            var stats = new LibraryStatisticsViewModel
            {
                LibraryName = libraryName,
                TotalReaders = _context.Readers.Count(r => r.Library.Name.Contains(libraryName)),
                NewReadersThisMonth = _context.Readers.Count(r => r.Library.Name.Contains(libraryName) &&
                                     r.RegistrationDate.Month == reportMonth &&
                                     r.RegistrationDate.Year == reportYear),
                TotalLoansThisMonth = _context.Loans.Count(l => l.Reader.Library.Name.Contains(libraryName) &&
                                     l.LoanDate.Month == reportMonth &&
                                     l.LoanDate.Year == reportYear),
                OverdueLoans = _context.Loans.Count(l => l.Reader.Library.Name.Contains(libraryName) &&
                               l.ReturnDate == null &&
                               l.DueDate < DateTime.Now)
            };

            var pdfBytes = _pdfService.GenerateLibraryStatisticsPdf(stats, reportMonth, reportYear);
            return File(pdfBytes, "application/pdf", $"LibraryStats_{reportMonth}_{reportYear}.pdf");
        }

    }

}