using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class Manufacturer
{
    public int ManufacturerId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();
}
