using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Hall
{
    public int HallId { get; set; }

    public int LibraryId { get; set; }

    public int HallTypeId { get; set; }

    public int HallNumber { get; set; }

    public int? Capacity { get; set; }

    public virtual ICollection<EmployeeSchedule>? EmployeeSchedules { get; set; } = new List<EmployeeSchedule>();

    public virtual HallType? HallType { get; set; } = null!;

    public virtual ICollection<Inventory>? Inventories { get; set; } = new List<Inventory>();

    public virtual Library? Library { get; set; } = null!;
}
