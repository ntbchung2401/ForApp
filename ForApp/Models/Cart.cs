using ForApp.Areas.Identity.Data;

namespace ForApp.Models
{
    public class Cart
    {
        public string UId { get; set; }
        public string BookIsbn { get; set; }
        public AppUser? User { get; set; }
        public Book? Book { get; set; }
    }

}