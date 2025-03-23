using Microsoft.EntityFrameworkCore;

namespace WebapiProject.Models
{
    public class ApplicationDbContext : DbContext
    {


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
        {
            Database.EnsureCreated();
        }


    public DbSet<User> Users { get; set; } // O/RM to map objects to database tables
    public DbSet<Product> Products { get; set; }
    public DbSet<Stock> Stocks { get; set; }
     public DbSet<StockDto> ProductDtos { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Report> Reports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StockDto>().HasNoKey(); // Specify that ProductDto has no key // creating a new entity by overridiing and also w/o primary key
        }

    }
}
