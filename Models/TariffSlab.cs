using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class TariffSlab
{
    public int SlabId { get; set; }

    public int TariffId { get; set; }

    public decimal FromKwh { get; set; }

    public decimal ToKwh { get; set; }

    public decimal RatePerKwh { get; set; }

    public virtual Tariff Tariff { get; set; } = null!;
}
