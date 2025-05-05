using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class EmployeeSchedule
{
    public int ScheduleId { get; set; }

    public int EmployeeId { get; set; }

    public int HallId { get; set; }

    public DateOnly WorkDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual Employee? Employee { get; set; } = null!;

    public virtual Hall? Hall { get; set; } = null!;
}
