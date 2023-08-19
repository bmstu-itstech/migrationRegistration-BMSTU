using System;
using System.Collections.Generic;

namespace MigrationBot.Models;

public partial class User
{
    public long ChatId { get; set; }

    public string? Comand { get; set; }

    public DateOnly? ArrivalDate { get; set; }

    public int? Country { get; set; }

    public DateTime? Entry { get; set; }

    public string? FioEn { get; set; }

    public string? FioRu { get; set; }

    public int? Service { get; set; }

    public virtual Entry? EntryNavigation { get; set; }
}
