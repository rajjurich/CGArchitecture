using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewModels.RequestModel
{
    public class ProductCreateModel
    {
        [Required]
        [StringLength(50)]
        public string ProductName { get; set; }

        [StringLength(200)]
        public string ProductDescription { get; set; }
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})?$", ErrorMessage ="invalid price")]
        public decimal? ProductPrice { get; set; }

        [StringLength(500)]
        public string ProductImage { get; set; }        
    }
}
