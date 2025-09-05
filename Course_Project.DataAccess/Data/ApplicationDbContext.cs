using Course_Project.Domain.Models.CustomElemsModels;
using Course_Project.Domain.Models.CustomIdModels;
using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Domain.Models.UserModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
namespace Course_Project.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInventoryAccess>()
        .HasKey(ua => new { ua.UserId, ua.InventoryId });

        modelBuilder.Entity<InventoryTag>()
        .HasKey(it => new { it.TagId, it.InventoryId });

        modelBuilder.Entity<InventoryTag>()
            .HasOne(ua => ua.Inventory)
            .WithMany(u => u.InventoryTags)
            .HasForeignKey(ua => ua.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventoryTag>()
            .HasOne(ua => ua.Tag)
            .WithMany(i => i.InventoryTags)
            .HasForeignKey(ua => ua.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserInventoryAccess>()
            .HasOne(ua => ua.User)
            .WithMany(u => u.InventoryAccesses)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserInventoryAccess>()
            .HasOne(ua => ua.Inventory)
            .WithMany(i => i.UserAccesses)
            .HasForeignKey(ua => ua.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Inventory>(entity =>
        { 
            entity.HasOne(i => i.User)
            .WithMany(u => u.UserInventories)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        }
        );

        modelBuilder.Entity<Item>()
            .HasOne(i => i.Inventory)
            .WithMany(u=>u.Items)
            .HasForeignKey(i => i.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CustomIdRule>()
            .HasOne(c => c.Inventory)
            .WithMany(i => i.CustomSetOfIds)
            .HasForeignKey(c => c.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CustomFieldOption>(entity =>
        {
            entity.Property(c => c.FieldType)
                  .IsRequired();
            entity.HasOne(i => i.Inventory)
            .WithMany(u => u.CustomElems)
            .HasForeignKey(i => i.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<CustomField>()
            .HasOne(c => c.Item)
            .WithMany(i => i.CustomFields)
            .HasForeignKey(c => c.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Like>()
        .HasKey(ua => new { ua.UserId, ua.ItemId });
        modelBuilder.Entity<Like>()
            .HasOne(c => c.Item)
            .WithMany(i => i.Likes)
            .HasForeignKey(c => c.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Like>()
            .HasOne(c => c.User)
            .WithMany(i => i.Likes)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Inventory)
            .WithMany(i => i.Comments)
            .HasForeignKey(c => c.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Item>()
        .HasIndex(u => u.CustomIdWithInventoryId).IsUnique();
        modelBuilder.Entity<CustomField>()
            .HasDiscriminator<string>("CustomField")
            .HasValue<CheckboxField>("CheckboxField")
            .HasValue<DocumentField>("DocumentField")
            .HasValue<MultiLineField>("MultiLineField")
            .HasValue<NumericField>("NumericField")
            .HasValue<SingleLineField>("SingleLineField");
        modelBuilder.Entity<Inventory>()
            .HasOne(i=>i.Category)
            .WithMany(i=>i.Inventories)
            .HasForeignKey(i=>i.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<UserInventoryAccess> UserInventoryAccess { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Comment> Comments { get; set; }
}
