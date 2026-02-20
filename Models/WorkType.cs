using System;
using System.Collections.Generic;

namespace SimpleAPI.Models;

public partial class WorkType
{
    public int Id { get; set; }

    public string? WorkName { get; set; }

    public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}
