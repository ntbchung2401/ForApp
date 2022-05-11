using System.ComponentModel.DataAnnotations;

namespace ForApp.Models
{
    public class Book
    {
        [Key]
        public string Isbn { get; set; }
        public string Title { get; set; }
        public int Pages { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        [Display(Name = "Price/book")]
        public double Price { get; set; }
        [Display(Name = "Description")]
        public string Desc { get; set; }
        public string ImgUrl { get; set; }
        public int StoreId { get; set; }
        public Store? Store { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }
    }

}