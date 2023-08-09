using System;
using System.Collections.Generic;

namespace MigrationBot.Models;

public partial class User
{
    public string? FioEn { get; set; }

    public int? Country { get; set; }

    public int? Service { get; set; }

    public string? Comand { get; set; }

    public DateTime? Entry { get; set; }

    public long ChatId { get; set; }

    public string? FioRu { get; set; }
}
