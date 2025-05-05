using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Loan
{
    public int LoanId { get; set; }

    public int InventoryId { get; set; }

    public int ReaderId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime LoanDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public virtual Employee? Employee { get; set; } = null!;

    public virtual Inventory? Inventory { get; set; } = null!;

    public virtual Reader? Reader { get; set; } = null!;
}
