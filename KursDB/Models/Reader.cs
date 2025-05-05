using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Reader
{
    public int ReaderId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateOnly RegistrationDate { get; set; }

    public int LibraryId { get; set; }

    public int ReaderTypeId { get; set; }

    public virtual Library? Library { get; set; } = null!;

    public virtual ICollection<Loan>? Loans { get; set; } = new List<Loan>();

    public virtual ICollection<ReaderAttribute>? ReaderAttributes { get; set; } = new List<ReaderAttribute>();

    public virtual ReaderType? ReaderType { get; set; } = null!;
}
