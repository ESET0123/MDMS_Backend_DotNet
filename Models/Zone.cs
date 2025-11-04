using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class Zone
{
    public int ZoneId { get; set; }

    public string ZoneName { get; set; } = null!;

    public virtual ICollection<Substation> Substations { get; set; } = new List<Substation>();
}
