using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public int PositionId { get; set; }

    public DateOnly HireDate { get; set; }

    public int LibraryId { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<EmployeeSchedule>? EmployeeSchedules { get; set; } = new List<EmployeeSchedule>();

    public virtual Library? Library { get; set; } = null!;

    public virtual ICollection<Loan>? Loans { get; set; } = new List<Loan>();

    public virtual Position? Position { get; set; } = null!;
}
