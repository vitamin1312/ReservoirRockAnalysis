using System.ComponentModel.DataAnnotations;

namespace CsharpBackend.Models.DTO
{
    public class ImageInfoDTO
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? CreationDate { get; set; }

        public int? FieldId { get; set; }
    }
}
