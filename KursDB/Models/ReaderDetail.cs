using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class ReaderDetail
{
    public int ReaderId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateOnly RegistrationDate { get; set; }

    public string ReaderType { get; set; } = null!;

    public string LibraryName { get; set; } = null!;

    public string? Attributes { get; set; }
}
