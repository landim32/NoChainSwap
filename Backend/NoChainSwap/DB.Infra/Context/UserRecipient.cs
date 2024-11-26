using System;
using System.Collections.Generic;

namespace DB.Infra.Context;

public partial class UserRecipient
{
    public long RecipientId { get; set; }

    public long UserId { get; set; }

    public int ChainId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Address { get; set; }

    public virtual User User { get; set; }
}
