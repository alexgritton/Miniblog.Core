namespace Miniblog.Core.Models
{
    public class PostCategory
    {
        public string CategoryName { get; set; }
        public Category Category { get; set; }
        public string PostId { get; set; }
        public Post Post { get; set; }
    }
}