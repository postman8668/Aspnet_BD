using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Authorship
{
    public int AuthorshipId { get; set; }

    public int AuthorId { get; set; }

    public int WorkId { get; set; }

    public virtual Author? Author { get; set; } = null!;

    public virtual Work? Work { get; set; } = null!;
}
