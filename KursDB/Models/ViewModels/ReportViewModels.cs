using System;

namespace KursDB.Models.ViewModels
{
    public class ReaderDetailViewModel
    {
        public string FullName { get; set; }
        public string ReaderType { get; set; }
        public string LibraryName { get; set; }
        public string Attribute { get; set; }
    }

    public class ReaderWithWorkViewModel
    {
        public string ReaderName { get; set; }
        public string PublicationTitle { get; set; }
        public string WorkTitle { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class ReaderWithPublicationViewModel
    {
        public string ReaderName { get; set; }
        public string PublicationTitle { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class ReaderWithWorkInPeriodViewModel
    {
        public string ReaderName { get; set; }
        public string PublicationTitle { get; set; }
        public string WorkTitle { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class ReaderLoansViewModel
    {
        public string PublicationTitle { get; set; }
        public string PublicationType { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string LibraryName { get; set; }
    }

    public class LoanFromShelfViewModel
    {
        public string PublicationTitle { get; set; }
        public string ReaderName { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class ReaderServedByEmployeeViewModel
    {
        public string ReaderName { get; set; }
        public string PublicationTitle { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class OverdueLoanViewModel
    {
        public string ReaderName { get; set; }
        public string PublicationTitle { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysOverdue { get; set; }
        public string LibraryName { get; set; }
    }

    public class AcquiredOrWrittenOffViewModel
    {
        public string PublicationTitle { get; set; }
        public string PublicationType { get; set; }
        public DateTime Date { get; set; }
        public string LibraryName { get; set; }
    }

    public class EmployeeInHallViewModel
    {
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public DateTime WorkDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class InactiveReaderViewModel
    {
        public string ReaderName { get; set; }
        public string ReaderType { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLoanDate { get; set; }
    }

    public class PublicationByAuthorViewModel
    {
        public int InventoryId { get; set; }
        public string PublicationTitle { get; set; }
        public string AuthorName { get; set; }
        public string LibraryName { get; set; }
        public int HallNumber { get; set; }
        public int RackNumber { get; set; }
        public int ShelfNumber { get; set; }
    }

    public class AuthorBooksInReadingRoomViewModel
    {
        public string AuthorName { get; set; }
        public string PublicationTitle { get; set; }
        public string LibraryName { get; set; }
        public int HallNumber { get; set; }
        public int AvailableCopies { get; set; }
    }

    public class LibraryStatisticsViewModel
    {
        public string LibraryName { get; set; }
        public int TotalReaders { get; set; }
        public int NewReadersThisMonth { get; set; }
        public int TotalLoansThisMonth { get; set; }
        public int OverdueLoans { get; set; }
    }

    // Добавленные модели
    public class AuthorBooksViewModel
    {
        public string AuthorName { get; set; }
        public string PublicationTitle { get; set; }
        public int AvailableCopies { get; set; }
        public string LibraryName { get; set; }
        public int HallNumber { get; set; }
    }

    public class PopularWorkViewModel
    {
        public string WorkTitle { get; set; }
        public int LoanCount { get; set; }
        public string AuthorName { get; set; }
    }

    public class PopularPublicationViewModel
    {
        public string WorkTitle { get; set; }
        public string AuthorName { get; set; }
        public int LoanCount { get; set; }
    }


    // Объединенная версия EmployeePerformanceViewModel
    public class EmployeePerformanceViewModel
    {
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }
        public int ReadersServed { get; set; }
        public int LoansProcessed { get; set; }
    }

    public class PublicationWithWorkViewModel
    {
        public int InventoryID { get; set; }
        public string PublicationTitle { get; set; }
        public string WorkTitle { get; set; }
        public string LibraryName { get; set; }
        public int HallNumber { get; set; }
        public int RackNumber { get; set; }
        public int ShelfNumber { get; set; }
    }

}