using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Library
{
    public int LibraryId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Employee>? Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Hall>? Halls { get; set; } = new List<Hall>();

    public virtual ICollection<Inventory>? Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<Reader>? Readers { get; set; } = new List<Reader>();
}
