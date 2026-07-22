using Katlog.Consumer.Models;
using Microsoft.EntityFrameworkCore;

namespace Katlog.Consumer.Data;

public class ConsumerDbContext : DbContext
{
    public ConsumerDbContext(
        DbContextOptions<ConsumerDbContext> options)
        : base(options) { }

    public DbSet<NotificationLog> NotificationLogs
    { get; set; }
}