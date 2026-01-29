using System;
using System.Collections.Generic;

namespace dotnet_simple_api.Models;

public partial class WorkLog
{
    public int Id { get; set; }

    public int MachineId { get; set; }

    public int OperatorId { get; set; }

    public int ProjectId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? OutputQuantity { get; set; }

    public string? Notes { get; set; }

    public virtual Machine Machine { get; set; } = null!;

    public virtual Operator Operator { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
