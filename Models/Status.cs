using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Consumer> Consumers { get; set; } = new List<Consumer>();

    public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();
}
