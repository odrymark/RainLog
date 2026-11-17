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
            entity.Property(u => u.Id).ValueGeneratedNever();
            entity.Property(u => u.Username).HasMaxLength(255);
            entity.Property(u => u.Password).HasMaxLength(255);
            entity.Property(u => u.AccessToken).HasMaxLength(255);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(c => c.Id).ValueGeneratedNever();
            entity.Property(c => c.Name).HasMaxLength(255);
            entity.Property(c => c.ShortName).HasMaxLength(255);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.Property(d => d.Id).ValueGeneratedNever();
            entity.Property(d => d.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.Property(f => f.Id).ValueGeneratedNever();
            entity.Property(f => f.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<ClientFeature>(entity =>
        {
            entity.Property(cf => cf.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(p => p.Id).ValueGeneratedNever();
            entity.Property(p => p.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.Property(up => up.Id).ValueGeneratedNever();
        });
        
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Users)
            .WithOne(u => u.Client)
            .HasForeignKey(u => u.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Device>()
            .HasOne(d => d.Client)
            .WithMany(c => c.Devices)
            .HasForeignKey(d => d.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ClientFeature>()
            .HasOne(cf => cf.Client)
            .WithMany(c => c.ClientFeatures)
            .HasForeignKey(cf => cf.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ClientFeature>()
            .HasOne(cf => cf.Feature)
            .WithMany(f => f.ClientFeatures)
            .HasForeignKey(cf => cf.FeatureId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Permission>()
            .HasOne(p => p.Feature)
            .WithMany(f => f.Permissions)
            .HasForeignKey(p => p.FeatureId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserPermission>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserPermissions)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserPermission>()
            .HasOne(up => up.Permission)
            .WithMany(p => p.UserPermissions)
            .HasForeignKey(up => up.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// ==========================================
// ENTITIES WITH GUID (UUID v7) IDENTIFIERS
// ==========================================

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ClientId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public long? TokenExpiry { get; set; }
    public bool Active { get; set; }

    public Client Client { get; set; } = null!;
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}

public class Client
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public bool Active { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Device> Devices { get; set; } = new List<Device>();
    public ICollection<ClientFeature> ClientFeatures { get; set; } = new List<ClientFeature>();
}

public class Device
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public bool Active { get; set; }

    public Client Client { get; set; } = null!;
}

public class Feature
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public bool Active { get; set; }

    public ICollection<ClientFeature> ClientFeatures { get; set; } = new List<ClientFeature>();
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}

public class ClientFeature
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ClientId { get; set; }
    public Guid FeatureId { get; set; }

    public Client Client { get; set; } = null!;
    public Feature Feature { get; set; } = null!;
}

public class Permission
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public Guid FeatureId { get; set; }
    public bool Active { get; set; }

    public Feature Feature { get; set; } = null!;
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}

public class UserPermission
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }

    public User User { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}