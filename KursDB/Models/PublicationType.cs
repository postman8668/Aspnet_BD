using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class PublicationType
{
    public int PublicationTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Publication>? Publications { get; set; } = new List<Publication>();
}
