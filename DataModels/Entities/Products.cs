using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.Entities;

public class Products
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ProductId { get; set; }
    [MaxLength(255)]
    [Required]
    public string ProductName { get; set; }
    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public Categories Categories { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    [MaxLength(512)]
    public string ProductDescription { get; set; }
    [MaxLength(512)]
    public string ImageUrl { get; set; }
    [Required]
    public DateTime DateAdded { get; set; }
    public virtual ICollection<Orders> Orders { get; set; }
}