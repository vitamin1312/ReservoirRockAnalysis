using System.ComponentModel.DataAnnotations;

namespace CsharpBackend.Models
{
    public class ImageInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UploadDate { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? CreationDate { get; set; }

        public int? FieldId { get; set; }
        virtual public Field? Field { get; set; }
    }
}
