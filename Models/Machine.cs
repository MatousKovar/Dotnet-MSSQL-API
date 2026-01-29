using System;
using System.Collections.Generic;

namespace dotnet_simple_api.Models;

public partial class Machine
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public int? MachineTypeId { get; set; }

    public string? Status { get; set; }

    public string? Location { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public virtual MachineType? MachineType { get; set; }

    public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}
