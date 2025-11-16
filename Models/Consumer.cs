using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class Consumer
{
    public int ConsumerId { get; set; }

    public string Name { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();

    public virtual ICollection<MonthlyBill> MonthlyBills { get; set; } = new List<MonthlyBill>();

    public virtual Status Status { get; set; } = null!;
}
