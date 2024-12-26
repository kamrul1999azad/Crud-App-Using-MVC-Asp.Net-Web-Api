using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Evi_Mon_CP_V01.Models
{
    public enum ProductType { Computer = 1, Laptop, NoteBook }
    public class Product
    {
        public int ProductId { get; set; }
        [Required, StringLength(50)]
        public string ProductName { get; set; } = default!;

        [Required, EnumDataType(typeof(ProductType))]
        public ProductType ProductType { get; set; } = default!;
        [Required, Column(TypeName = "money")]
        public decimal? Price { get; set; }
        [Required, Column(TypeName = "date")]
        public DateTime? MfgDate { get; set; }
        [Required, StringLength(50)]

        public string Picture { get; set; } = default!;
        public bool InStock { get; set; }
        //
        public virtual ICollection<Sale> Sales { get; set; } = [];
    }
    public class Sale
    {
        public int SaleId { get; set; }
        [Required, StringLength(50)]

        public string SellerName { get; set; } = default!;
        [Required]
        public int Quantity { get; set; }
        [Required, ForeignKey("Product")]
        //fk
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

    }
   
    public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        
    }

}
