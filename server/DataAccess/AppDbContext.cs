using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<ClientFeature> ClientFeatures => Set<ClientFeature>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Username).HasMaxLength(255);
            entity.Property(u => u.Password).HasMaxLength(255);
        });

        modelBuilder.Entity<Client>()
            .Property(c => c.Name).HasMaxLength(255);

        modelBuilder.Entity<Device>()
            .Property(d => d.Name).HasMaxLength(255);

        modelBuilder.Entity<Feature>()
            .Property(f => f.Name).HasMaxLength(255);

        modelBuilder.Entity<Permission>()
            .Property(p => p.Name).HasMaxLength(255);

        
        modelBuilder.Entity<Client>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.Id)
            .HasPrincipalKey(u => u.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Device>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(d => d.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ClientFeature>()
            .HasOne<Client>()
            .WithMany()
            .HasForeignKey(cf => cf.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ClientFeature>()
            .HasOne<Feature>()
            .WithMany()
            .HasForeignKey(cf => cf.FeatureId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Permission>()
            .HasOne<Feature>()
            .WithMany()
            .HasForeignKey(p => p.FeatureId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserPermission>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserPermission>()
            .HasOne<Permission>()
            .WithMany()
            .HasForeignKey(up => up.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class User
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public bool Active { get; set; }
}

public class Client
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }
}

public class Device
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long ClientId { get; set; }
    public bool Active { get; set; }
}

public class Feature
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }
}

public class ClientFeature
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public long FeatureId { get; set; }
}

public class Permission
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long FeatureId { get; set; }
    public bool Active { get; set; }
}

public class UserPermission
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long PermissionId { get; set; }
}