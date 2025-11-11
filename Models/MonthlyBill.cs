using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class MonthlyBill
{
    public int BillId { get; set; }

    public int MeterId { get; set; }

    public int ConsumerId { get; set; }

    public DateOnly BillingDate { get; set; }

    public decimal TotalConsumptionKwh { get; set; }

    public decimal TotalAmount { get; set; }

    public string BillStatus { get; set; } = null!;

    public int BillingMonth { get; set; }

    public int BillingYear { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Consumer Consumer { get; set; } = null!;

    public virtual Meter Meter { get; set; } = null!;
}
