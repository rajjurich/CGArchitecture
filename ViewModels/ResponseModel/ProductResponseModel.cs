using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.ResponseModel
{
    public class ProductResponseModel
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        [StringLength(200)]
        public string ProductDescription { get; set; }

        public decimal? ProductPrice { get; set; }

        [StringLength(500)]
        public string ProductImage { get; set; }
    }
}
