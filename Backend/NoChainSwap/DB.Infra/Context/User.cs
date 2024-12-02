using System;
using System.Collections.Generic;

namespace DB.Infra.Context;

public partial class User
{
    public long UserId { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public string Hash { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public string RecoveryHash { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

    public virtual ICollection<UserRecipient> UserRecipients { get; set; } = new List<UserRecipient>();
}
