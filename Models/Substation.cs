using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class Substation
{
    public int SubstationId { get; set; }

    public string SubstationName { get; set; } = null!;

    public int ZoneId { get; set; }

    public virtual ICollection<Feeder> Feeders { get; set; } = new List<Feeder>();

    public virtual Zone Zone { get; set; } = null!;
}
