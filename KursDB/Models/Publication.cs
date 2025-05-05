using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class Publication
{
    public int PublicationId { get; set; }

    public string Title { get; set; } = null!;

    public int PublicationTypeId { get; set; }

    public string? Publisher { get; set; }

    public int? PublicationYear { get; set; }

    public string? Isbn { get; set; }

    public int? Pages { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Content>? Contents { get; set; } = new List<Content>();

    public virtual ICollection<Inventory>? Inventories { get; set; } = new List<Inventory>();

    public virtual PublicationType? PublicationType { get; set; } = null!;
}
