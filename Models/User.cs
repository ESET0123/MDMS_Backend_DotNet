using System;
using System.Collections.Generic;

namespace MDMS_Backend.Models;

public partial class User
{
    public int UserNumber { get; set; }

    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public byte[] PasswordHashed { get; set; } = null!;

    public int RoleId { get; set; }

    public DateTime? LastLogin { get; set; }

    public bool Active { get; set; }

    public virtual Role Role { get; set; } = null!;
}
