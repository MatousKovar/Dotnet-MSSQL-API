using System;
using System.Collections.Generic;

namespace dotnet_simple_api.Models;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ClientName { get; set; }

    public DateOnly? Deadline { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}
