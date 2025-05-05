using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class ReaderType
{
    public int ReaderTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Reader>? Readers { get; set; } = new List<Reader>();
}
