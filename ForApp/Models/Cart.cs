using ForApp.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForApp.Models
{
    public class Cart
    {
        public string UId { get; set; }
        public string BookIsbn { get; set; }
        public int Quantity { get; set; }
        public AppUser? User { get; set; }
        public Book? Book { get; set; }
    }

}