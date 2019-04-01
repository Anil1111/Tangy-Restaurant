using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Tangy.Models
{
    public class SubCategory
    {
        [Required]
        public int ID { get; set; }

        [Required]
        [Display(Name="Sub Category")]
        public string Name { get; set; }

        [Required]
        [Display(Name="Category ID")]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
    }
}
