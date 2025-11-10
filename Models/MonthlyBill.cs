using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class MonthlyBill
{
    public int BillId { get; set; }

    public int MeterId { get; set; }

    public int ConsumerId { get; set; }

    public int BillingMonth { get; set; }

    public int BillingYear { get; set; }

    public DateOnly BillStartDate { get; set; }

    public DateOnly BillEndDate { get; set; }

    public decimal TotalConsumptionKwh { get; set; }

    public decimal BaseAmount { get; set; }

    public decimal TotalSurgeCharges { get; set; }

    public decimal TotalDiscounts { get; set; }

    public decimal NetAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string BillStatus { get; set; } = null!;

    public DateTime? PaidDate { get; set; }

    public DateTime GeneratedAt { get; set; }

    public string GeneratedBy { get; set; } = null!;

    public virtual Consumer Consumer { get; set; } = null!;

    public virtual Meter Meter { get; set; } = null!;
}
