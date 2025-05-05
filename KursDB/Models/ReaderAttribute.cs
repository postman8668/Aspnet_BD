using System;
using System.Collections.Generic;

namespace KursDB.Models;

public partial class ReaderAttribute
{
    public int AttributeId { get; set; }

    public int ReaderId { get; set; }

    public string AttributeName { get; set; } = null!;

    public string AttributeValue { get; set; } = null!;

    public virtual Reader? Reader { get; set; } = null!;
}
