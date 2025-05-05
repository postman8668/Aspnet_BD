using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class HallType
{
    public int HallTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Hall>? Halls { get; set; } = new List<Hall>();
}
