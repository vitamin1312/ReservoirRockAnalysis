using System.ComponentModel.DataAnnotations;

namespace CsharpBackend.Models
{
    public class CoreSampleImage
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UploadDate { get; set; }

        [DataType(DataType.Date)] 
        public DateOnly? CreationDate { get; set; }

        public string PathToImage { get; set; }

        public string? PathToMask { get; set; }

        public int? FieldId { get; set; }
        virtual public Field? Field { get; set; }

    }
}
