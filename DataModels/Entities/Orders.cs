using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.Entities;

public class Orders
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int OrderId { get; set; }
	[Required]
	public DateTime OrderDate { get; set; }
	[MaxLength(100)]
	[Required]
	public string CustomerName { get; set; }
	public virtual ICollection<Products> Products { get; set; }
}
