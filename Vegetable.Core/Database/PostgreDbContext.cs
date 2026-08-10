using Microsoft.EntityFrameworkCore;
using System;
using Vegetable.Entities;
using Vegetable.Entities.DTO;

namespace Vegetable.Core.Database
{
    public class PostgreDbContext : DbContext
    {
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<EmployeeService> EmployeeService { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleOnDay> SchedulesOnDays { get; set; }
        public DbSet<Reservation> Reservations { get; set; }        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<SocialNetwork> SocialNetworks { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserData> UserData { get; set; }        
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ReservationService> ReservationService { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationMessage> NotificationMessages { get; set; }
        public DbSet<PaymentNotification> PaymentNotifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<SubscriptionType> SubscriptionTypes { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }

        public PostgreDbContext(DbContextOptions<PostgreDbContext> dbContextOptions) : base(dbContextOptions)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ReservationService>()
                .HasKey(t => new { t.ReservationId, t.ServiceId });

            modelBuilder.Entity<ReservationService>()
                .HasOne(pt => pt.Reservation)
                .WithMany(p => p.ReservationServices)
                .HasForeignKey(pt => pt.ReservationId);

            modelBuilder.Entity<ReservationService>()
                .HasOne(pt => pt.Service)
                .WithMany(t => t.ReservationServices)
                .HasForeignKey(pt => pt.ServiceId);

            modelBuilder.Entity<EmployeeService>()
                .HasKey(t => new { t.EmployeeId, t.ServiceId });

            modelBuilder.Entity<EmployeeService>()
                .HasOne(pt => pt.Employee)
                .WithMany(p => p.EmployeeServices)
                .HasForeignKey(pt => pt.EmployeeId);

            modelBuilder.Entity<EmployeeService>()
                .HasOne(pt => pt.Service)
                .WithMany(t => t.EmployeeServices)
                .HasForeignKey(pt => pt.ServiceId);

            modelBuilder.Entity<OwnerCustomer>()
                .HasKey(t => new { t.OwnerId, t.CustomerId });

            //modelBuilder.Entity<OwnerCustomer>()
            //    .HasOne(pt => pt.Owner)
            //    .WithMany(p => p.OwnerCustomers)
            //    .HasForeignKey(pt => pt.OwnerId);

            //modelBuilder.Entity<OwnerCustomer>()
            //    .HasOne(pt => pt.Customer)
            //    .WithMany(t => t.CustomerOwners)
            //    .HasForeignKey(pt => pt.CustomerId);

            modelBuilder.Entity<Reservation>()
                .HasOne(p => p.Employee)
                .WithMany(b => b.Reservations)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Schedules)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ScheduleOnDay>()
                .HasOne(so => so.Schedule)
                .WithMany(s => s.ScheduleOnDays)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserData>()
                .HasOne(um => um.User)
                .WithMany(u => u.UserData)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Currency>().HasData(
                new Currency() { Id = 1, CurrencyCode = "RUB", Name = "Russian ruble", Symbol = "₽" },
                new Currency() { Id = 2, CurrencyCode = "USD", Name = "United States dollar", Symbol = "$" });

            modelBuilder.Entity<SubscriptionType>().HasData(
               new SubscriptionType() { Id = 1, Name = "subscription.title-free", Description = "subscription.description-free", Price = 0, IsEnabled = true, IsDefault = true },
               new SubscriptionType() { Id = 2, Name = "subscription.title-premium", Description = "subscription.description-premium", Price = 20000, IsEnabled = true, IsDefault = false },
               new SubscriptionType() { Id = 3, Name = "subscription.title-ultra", Description = "subscription.description-ultra", Price = 50000, IsEnabled = false, IsDefault = false });

            modelBuilder.Entity<Discount>().HasData(
               new Discount() { Id = 1, Quantity = 1, TrialQuantity = 1, Percentage = 0, IsEnabled = true },
               new Discount() { Id = 2, Quantity = 3, TrialQuantity = 1, Percentage = 10, IsEnabled = true },
               new Discount() { Id = 3, Quantity = 6, TrialQuantity = 2, Percentage = 15, IsEnabled = true },
               new Discount() { Id = 4, Quantity = 12, TrialQuantity = 3, Percentage = 20, IsEnabled = true });

            modelBuilder.Entity<ReservationsCountByDay>().HasNoKey().ToView("dummy");
            modelBuilder.Entity<ReservationsTotalCostByMonth>().HasNoKey().ToView("totalcost");

            modelBuilder.Entity<UserToPush>().HasNoKey().ToView("UserToPush");

            modelBuilder.Entity<Owner>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<Address>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<Customer>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<Employee>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<Image>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<Notification>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<Notification>()
                .HasOne(i => i.Reservation)
                .WithMany(c => c.Notifications)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NotificationMessage>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<Order>()
               .Property(x => x.CreatedDateUTC)
               .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<PaymentNotification>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<Reservation>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<Schedule>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<ScheduleOnDay>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<Service>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<SocialNetwork>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<User>()
                .Property(x => x.CreatedDateUTC)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            modelBuilder.Entity<PhoneNumber>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.PhoneNumbers)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PaymentNotification>()
               .HasOne(x => x.Owner)
               .WithMany(x => x.PaymentNotifications)
               .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
               .HasOne(x => x.Owner)
               .WithMany(x => x.Orders)
               .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
