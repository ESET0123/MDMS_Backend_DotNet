using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class TodRule
{
    public int TodRuleId { get; set; }

    public string RuleName { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int TariffId { get; set; }

    public decimal SurgeChargePercent { get; set; }

    public decimal DiscountPercent { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public virtual ICollection<DailyMeterReading> DailyMeterReadings { get; set; } = new List<DailyMeterReading>();

    public virtual Tariff Tariff { get; set; } = null!;
}
