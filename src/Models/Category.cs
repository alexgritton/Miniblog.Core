using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Miniblog.Core.Models
{
    public class Category
    {
        [Key]
        public string Name { get; set; }
        public List<Post> Posts { get; set; }
    }
}