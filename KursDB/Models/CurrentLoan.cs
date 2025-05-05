using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class CurrentLoan
{
    public int LoanId { get; set; }

    public string ReaderName { get; set; } = null!;

    public string PublicationTitle { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public DateTime LoanDate { get; set; }

    public DateTime DueDate { get; set; }

    public int? DaysOverdue { get; set; }

    public string LibraryName { get; set; } = null!;
}
