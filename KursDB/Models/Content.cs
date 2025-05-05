using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Content
{
    public int ContentId { get; set; }

    public int PublicationId { get; set; }

    public int WorkId { get; set; }

    public int? PageStart { get; set; }

    public int? PageEnd { get; set; }

    public virtual Publication? Publication { get; set; } = null!;

    public virtual Work? Work { get; set; } = null!;
}
