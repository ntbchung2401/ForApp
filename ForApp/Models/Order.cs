using ForApp.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForApp.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "OrderID")]
        public int Id { get; set; }
        public string UId { get; set; }
        public DateTime OrderDate { get; set; }
        public double Total { get; set; }
        public AppUser User { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }

}