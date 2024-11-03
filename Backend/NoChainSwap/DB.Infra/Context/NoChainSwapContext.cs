using System;
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
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
            entity.Property(e => e.CreateAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_at");
            entity.Property(e => e.ReceiverAddress)
                .IsRequired()
                .HasMaxLength(80)
                .HasColumnName("receiver_address");
            entity.Property(e => e.ReceiverAmount).HasColumnName("receiver_amount");
            entity.Property(e => e.ReceiverCoin)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("receiver_coin");
            entity.Property(e => e.ReceiverFee).HasColumnName("receiver_fee");
            entity.Property(e => e.ReceiverTxid)
                .HasMaxLength(80)
                .HasColumnName("receiver_txid");
            entity.Property(e => e.SenderAddress)
                .IsRequired()
                .HasMaxLength(80)
                .HasColumnName("sender_address");
            entity.Property(e => e.SenderAmount).HasColumnName("sender_amount");
            entity.Property(e => e.SenderCoin)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("sender_coin");
            entity.Property(e => e.SenderFee).HasColumnName("sender_fee");
            entity.Property(e => e.SenderTxid)
                .HasMaxLength(80)
                .HasColumnName("sender_txid");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("update_at");
        });

        modelBuilder.Entity<TransactionLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("pk_tx_log");

            entity.ToTable("transaction_logs");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
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
            entity.Property(e => e.BtcAddress)
                .IsRequired()
                .HasMaxLength(64)
                .HasColumnName("btc_address");
            entity.Property(e => e.CreateAt).HasColumnName("create_at");
            entity.Property(e => e.Hash)
                .HasMaxLength(64)
                .HasColumnName("hash");
            entity.Property(e => e.StxAddress)
                .IsRequired()
                .HasMaxLength(64)
                .HasColumnName("stx_address");
            entity.Property(e => e.UpdateAt).HasColumnName("update_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
