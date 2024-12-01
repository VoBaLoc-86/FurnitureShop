namespace FurnitureShop.Models
{
    public class Page:BaseModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
    }
}
