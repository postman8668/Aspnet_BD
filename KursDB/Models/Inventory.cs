using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public int PublicationId { get; set; }

    public int LibraryId { get; set; }

    public int HallId { get; set; }

    public int ShelfNumber { get; set; }

    public int RackNumber { get; set; }

    public DateOnly AcquisitionDate { get; set; }

    public DateOnly? WriteOffDate { get; set; }

    public bool? IsAvailable { get; set; }

    public bool? CanTakeHome { get; set; }

    public int? LoanPeriodDays { get; set; }

    public virtual Hall? Hall { get; set; } = null!;

    public virtual Library? Library { get; set; } = null!;

    public virtual ICollection<Loan>? Loans { get; set; } = new List<Loan>();

    public virtual Publication? Publication { get; set; } = null!;
}
