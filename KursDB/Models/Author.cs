using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Author
{
    public int AuthorId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public int? BirthYear { get; set; }

    public int? DeathYear { get; set; }

    public string? Biography { get; set; }

    public virtual ICollection<Authorship>? Authorships { get; set; } = new List<Authorship>();
}
