using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class Meter
{
    public int MeterId { get; set; }

    public int ConsumerId { get; set; }

    public int Dtrid { get; set; }

    public string Ipaddress { get; set; } = null!;

    public string? Iccid { get; set; }

    public string? Imsi { get; set; }

    public int ManufacturerId { get; set; }

    public string? Firmware { get; set; }

    public int TariffId { get; set; }

    public DateOnly InstallDate { get; set; }

    public int StatusId { get; set; }

    public decimal LatestReading { get; set; }

    public virtual Consumer Consumer { get; set; } = null!;

    public virtual Dtr Dtr { get; set; } = null!;

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<MonthlyBill> MonthlyBills { get; set; } = new List<MonthlyBill>();

    public virtual Status Status { get; set; } = null!;

    public virtual Tariff Tariff { get; set; } = null!;
}
