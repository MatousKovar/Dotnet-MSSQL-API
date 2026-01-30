using System;
using System.Collections.Generic;

namespace SimpleAPI.Models;

public partial class MachineType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? MaintenanceIntervalHours { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Machine> Machines { get; set; } = new List<Machine>();
}
