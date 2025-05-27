using CsharpBackend.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace CsharpBackend.Models
{
    public class ImageInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UploadDate { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? CreationDate { get; set; }

        public int? FieldId { get; set; }
        public Field? Field { get; set; }

        public ImageInfo() {
            UploadDate = DateTime.Now;
        }

        public double pixelLengthRatio { get; set; }

        public ImageInfo(ImageInfoDTO dto)
        {
            if (!double.TryParse(dto.pixelLengthRatio.Replace('.', ','), out double result))
            {
                if (result <= 0)
                    throw new Exception("pixelLengthRatio must be greater than zero");
            }

            pixelLengthRatio = result;
            Name = (dto.Name != null) ? dto.Name : $"Image{Id}";
            Description = dto.Description;
            CreationDate = dto.CreationDate;
            UploadDate = DateTime.Now;
            FieldId = dto.FieldId;
        }
    }
}
