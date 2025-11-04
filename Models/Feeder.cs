using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class Feeder
{
    public int FeederId { get; set; }

    public string FeederName { get; set; } = null!;

    public int SubstationId { get; set; }

    public virtual ICollection<Dtr> Dtrs { get; set; } = new List<Dtr>();

    public virtual Substation Substation { get; set; } = null!;
}
