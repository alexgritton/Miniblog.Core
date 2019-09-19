using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Miniblog.Core.Models
{
    public class Category
    {
        [Key]
        [MaxLength(128)]
        public string Name { get; set; }
        public List<PostCategory> PostCategories { get; set; }
    }
}