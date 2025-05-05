using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Work
{
    public int WorkId { get; set; }

    public string Title { get; set; } = null!;

    public int CategoryId { get; set; }

    public int? YearWritten { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Authorship>? Authorships { get; set; } = new List<Authorship>();

    public virtual WorkCategory? Category { get; set; } = null!;

    public virtual ICollection<Content>? Contents { get; set; } = new List<Content>();
}
