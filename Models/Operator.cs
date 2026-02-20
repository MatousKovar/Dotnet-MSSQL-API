using System;
using System.Collections.Generic;

namespace SimpleAPI.Models;

public partial class Operator
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string BadgeNumber { get; set; } = null!;

    public string? Email { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}
