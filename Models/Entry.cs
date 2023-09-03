using System;
using System.Collections.Generic;

namespace MigrationBot.Models;

public partial class Entry
{
    public long UserId { get; set; }

    public DateTime? Date { get; set; }

    public int? Service { get; set; }

    public virtual User User { get; set; } = null!;
}
