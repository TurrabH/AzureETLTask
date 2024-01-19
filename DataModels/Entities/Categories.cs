using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.Entities;

public class Categories
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CategoryId { get; set; }
    [MaxLength(100)]
    public string CategoryName { get; set; }
    public virtual ICollection<Products> Products { get; set; }
}
