namespace CsharpBackend.Models
{
    public class Field
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public ICollection<ImageInfo>? FieldImages { get; set; }
    }
}
