using CsharpBackend.Models;
using Microsoft.AspNetCore.Mvc;
using static CsharpBackend.Controllers.AccountController;

namespace CsharpBackend.Repository
{
    public interface IImageRepository : IDisposable
    {
        // Images
        Task<ActionResult<IEnumerable<CoreSampleImage>>> GetImagesList();
        Task<CoreSampleImage?> GetImage(int id);
        Task CreateImage(CoreSampleImage image);
        void UpdateImage(CoreSampleImage image);
        Task DeleteImage(int id);
        Task Save();
        Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageFromField(int fieldId);
        Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageWithMask();
        Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageWithoutMask();
        bool CoreSampleImageExists(int id);

        // Images info
        void UpdateInfo(ImageInfo info);

        // Users
        Task<User?> GetUser(LoginData ld);
        Task<ActionResult<IEnumerable<User>>> GetUsers();

        // Field
        Task<ActionResult<IEnumerable<Field>>> GetFieldsList();
        Task<Field?> GetField(int id);
        Task CreateField(Field field);
        void UpdateField(Field field);
        Task DeleteField(int id);
        bool FieldExists(int id);

    }
}
