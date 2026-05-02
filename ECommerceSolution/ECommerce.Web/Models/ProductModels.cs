using System.ComponentModel.DataAnnotations;

namespace ECommerce.Web.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(120, ErrorMessage = "Product name must be 120 characters or fewer.")]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, 999999, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        [Display(Name = "Image URL")]
        [Url(ErrorMessage = "Enter a valid image URL.")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload image")]
        public IFormFile? ImageFile { get; set; }
    }
}
