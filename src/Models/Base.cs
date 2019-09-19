using System.ComponentModel.DataAnnotations;

namespace Miniblog.Core.Models{
    public abstract class Base{
        [MaxLength(64)]
        public string Id { get; set; }
    }
}