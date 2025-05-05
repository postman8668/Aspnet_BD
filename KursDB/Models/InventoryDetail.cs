using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class InventoryDetail
{
    public int InventoryId { get; set; }

    public string PublicationTitle { get; set; } = null!;

    public string PublicationType { get; set; } = null!;

    public string LibraryName { get; set; } = null!;

    public int HallNumber { get; set; }

    public string HallType { get; set; } = null!;

    public int RackNumber { get; set; }

    public int ShelfNumber { get; set; }

    public DateOnly AcquisitionDate { get; set; }

    public DateOnly? WriteOffDate { get; set; }

    public string Status { get; set; } = null!;

    public string CanTakeHome { get; set; } = null!;

    public int? LoanPeriodDays { get; set; }
}
