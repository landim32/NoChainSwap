using System;
using System.Collections.Generic;

namespace DB.Infra.Context;

public partial class UserAddress
{
    public long AddressId { get; set; }

    public long UserId { get; set; }

    public int ChainId { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public string Address { get; set; }

    public virtual User User { get; set; }
}
