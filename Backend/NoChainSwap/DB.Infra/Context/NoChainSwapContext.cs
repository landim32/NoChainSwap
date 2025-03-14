﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DB.Infra.Context;

public partial class NoChainSwapContext : DbContext
{
    public NoChainSwapContext()
    {
    }

    public NoChainSwapContext(DbContextOptions<NoChainSwapContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionLog> TransactionLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    public virtual DbSet<UserRecipient> UserRecipients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //=> optionsBuilder.UseNpgsql("Host=167.172.240.71;Port=5432;Database=crosschainswap;Username=postgres;Password=eaa69cpxy2");
        => optionsBuilder.UseNpgsql("Host=172.18.0.2;Port=5432;Database=crosschainswap;Username=postgres;Password=eaa69cpxy2");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TxId).HasName("pk_tx");

            entity.ToTable("transactions");

            entity.Property(e => e.TxId)
                .HasDefaultValueSql("nextval('transactions_tx_nid_seq'::regclass)")
                .HasColumnName("tx_id");
            entity.Property(e => e.ChainId).HasColumnName("chain_id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_at");
            entity.Property(e => e.Hash)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("hash");
            entity.Property(e => e.ReceiverAddress)
                .HasMaxLength(256)
                .HasColumnName("receiver_address");
            entity.Property(e => e.ReceiverAmount).HasColumnName("receiver_amount");
            entity.Property(e => e.ReceiverCoin)
                .HasMaxLength(6)
                .HasColumnName("receiver_coin");
            entity.Property(e => e.ReceiverFee).HasColumnName("receiver_fee");
            entity.Property(e => e.ReceiverTax).HasColumnName("receiver_tax");
            entity.Property(e => e.ReceiverTxid)
                .HasMaxLength(256)
                .HasColumnName("receiver_txid");
            entity.Property(e => e.RecipientAddress)
                .HasMaxLength(256)
                .HasColumnName("recipient_address");
            entity.Property(e => e.SenderAddress)
                .HasMaxLength(256)
                .HasColumnName("sender_address");
            entity.Property(e => e.SenderAmount).HasColumnName("sender_amount");
            entity.Property(e => e.SenderCoin)
                .HasMaxLength(6)
                .HasColumnName("sender_coin");
            entity.Property(e => e.SenderFee).HasColumnName("sender_fee");
            entity.Property(e => e.SenderTax).HasColumnName("sender_tax");
            entity.Property(e => e.SenderTxid)
                .HasMaxLength(256)
                .HasColumnName("sender_txid");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_transaction");
        });

        modelBuilder.Entity<TransactionLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("pk_tx_log");

            entity.ToTable("transaction_logs");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.LastStatus).HasColumnName("last_status");
            entity.Property(e => e.LogType)
                .HasDefaultValue(1)
                .HasColumnName("log_type");
            entity.Property(e => e.Message)
                .HasMaxLength(300)
                .HasColumnName("message");
            entity.Property(e => e.TxId).HasColumnName("tx_id");

            entity.HasOne(d => d.Tx).WithMany(p => p.TransactionLogs)
                .HasForeignKey(d => d.TxId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tx_log");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_at");
            entity.Property(e => e.Email)
                .HasMaxLength(180)
                .HasColumnName("email");
            entity.Property(e => e.Hash)
                .HasMaxLength(128)
                .HasColumnName("hash");
            entity.Property(e => e.IsAdmin)
                .HasDefaultValue(false)
                .HasColumnName("is_admin");
            entity.Property(e => e.Name)
                .HasMaxLength(120)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .HasColumnName("password");
            entity.Property(e => e.RecoveryHash)
                .HasMaxLength(128)
                .HasColumnName("recovery_hash");
            entity.Property(e => e.Token)
                .HasMaxLength(128)
                .HasColumnName("token");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_at");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("user_addresses_pkey");

            entity.ToTable("user_addresses");

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnName("address");
            entity.Property(e => e.ChainId).HasColumnName("chain_id");
            entity.Property(e => e.CreateAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_at");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_address");
        });

        modelBuilder.Entity<UserRecipient>(entity =>
        {
            entity.HasKey(e => e.RecipientId).HasName("user_recipients_pkey");

            entity.ToTable("user_recipients");

            entity.Property(e => e.RecipientId).HasColumnName("recipient_id");
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(120)
                .IsFixedLength()
                .HasColumnName("address");
            entity.Property(e => e.ChainId).HasColumnName("chain_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserRecipients)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_recipient");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
