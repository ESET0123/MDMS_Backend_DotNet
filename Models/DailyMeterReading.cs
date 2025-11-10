using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class DailyMeterReading
{
    public int ReadingId { get; set; }

    public int MeterId { get; set; }

    public DateOnly ReadingDate { get; set; }

    public int TodRuleId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public decimal PreviousReading { get; set; }

    public decimal CurrentReading { get; set; }

    public decimal ConsumptionKwh { get; set; }

    public decimal BaseRate { get; set; }

    public decimal SurgeChargePercent { get; set; }

    public decimal DiscountPercent { get; set; }

    public decimal EffectiveRate { get; set; }

    public decimal Amount { get; set; }

    public string RecordedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Meter Meter { get; set; } = null!;

    public virtual TodRule TodRule { get; set; } = null!;
}
