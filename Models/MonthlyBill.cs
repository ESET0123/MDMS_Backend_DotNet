using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class MonthlyBill
{
    public int BillId { get; set; }

    public int MeterId { get; set; }

    public int ConsumerId { get; set; }

    public DateOnly BillingMonth { get; set; }

    public string ConsumerName { get; set; } = null!;

    public string TariffName { get; set; } = null!;

    public decimal MonthlyReadingKwh { get; set; }

    public decimal BaseRate { get; set; }

    public decimal TaxRate { get; set; }

    public decimal TotalBill { get; set; }

    public string BillStatus { get; set; } = null!;

    public virtual Meter Meter { get; set; } = null!;
}
