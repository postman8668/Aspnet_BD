using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class WorkCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Work>? Works { get; set; } = new List<Work>();
}
