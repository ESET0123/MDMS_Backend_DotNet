using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class Tariff
{
    public int TariffId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public decimal BaseRate { get; set; }

    public decimal TaxRate { get; set; }

    public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();

    public virtual ICollection<TariffSlab> TariffSlabs { get; set; } = new List<TariffSlab>();

    public virtual ICollection<TodRule> TodRules { get; set; } = new List<TodRule>();
}
