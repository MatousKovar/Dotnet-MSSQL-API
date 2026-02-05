using SimpleAPI.Models;
using SimpleAPI.Data;
namespace SimpleAPI.DTOs;

public class MachineDto
{
    public string Code { get; set; } = null!;

    public string? Status { get; set; }

    public string? Location { get; set; }

    public string MachineTypeName { get; set; } = null!;
}