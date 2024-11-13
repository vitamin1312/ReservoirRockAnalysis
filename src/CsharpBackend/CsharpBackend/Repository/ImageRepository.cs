using CsharpBackend.Data;
using CsharpBackend.Models;
using CsharpBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CsharpBackend.Controllers.AccountController;

namespace CsharpBackend.Repository
{
    public class ImageRepository: IImageRepository
    {
        private CsharpBackendContext db;

        private bool disposed = false;

        public ImageRepository(CsharpBackendContext _context)
        {
            _context.Database.EnsureCreated();
            this.db = _context;
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetImagesList()
        {
            return await db.CoreSampleImage.Include(i => i.ImageInfo).ToListAsync();
        }

        public async Task<CoreSampleImage?> GetImage(int id)
        {
            return await db.CoreSampleImage
                .Include(i => i.ImageInfo)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task CreateImage(CoreSampleImage image)
        {
            await db.CoreSampleImage.AddAsync(image);
        }

        public void UpdateImage(CoreSampleImage image)
        {
            db.Entry(image).State = EntityState.Modified;
        }

        public async Task DeleteImage(int id)
        {
            var image = await db.CoreSampleImage.FindAsync(id);

            if (image != null)
            {
                if (image.ImageInfo != null)
                {
                    var info = image.ImageInfo;
                    db.ImageInfo.Remove(info);
                }

                image.DeleteItemFiles();
                db.CoreSampleImage.Remove(image);
            }  
        }

        public bool FieldExists(int id)
        {
            return db.Field.Any(f => f.Id == id);
        }

        public bool CoreSampleImageExists(int id)
        {
            return db.CoreSampleImage.Any(i => i.Id == id);
        }

        public async Task Save()
        {
            await db.SaveChangesAsync();
        }

        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageFromField(int fieldId)
        {
            return await db.CoreSampleImage
                .Include(image => image.ImageInfo)
                .Where(image => image.ImageInfo.FieldId == fieldId)
                .ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageWithMask()
        {
            return await db.CoreSampleImage
                .Include(image => image.ImageInfo)
                .Where(image => image.PathToMask != null)
                .ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageWithoutMask()
        {
            return await db.CoreSampleImage
                .Include(image => image.ImageInfo)
                .Where(image => image.PathToMask == null)
                .ToListAsync();
        }

        public void UpdateInfo(ImageInfo info)
        {
            db.Entry(info).State = EntityState.Modified;
        }

        public async Task DeleteInfo(int id)
        {
            var info = await db.ImageInfo.FindAsync(id);
            if (info != null)
                db.ImageInfo.Remove(info);
        }

        public async Task<User?> GetUser(LoginData ld)
        {
            return await db.User.FirstOrDefaultAsync(u => u.Login == ld.login && u.Password == ld.password);
        }

        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await db.User.ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Field>>> GetFieldsList()
        {
            return await db.Field.ToListAsync();
        }
        public async Task<Field?> GetField(int id)
        {
            return await db.Field.FindAsync(id);
        }
        public async Task CreateField(Field field)
        {
            field.SetName();
            await db.Field.AddAsync(field);
        }
        public void UpdateField(Field field)
        {
            db.Entry(field).State = EntityState.Modified;
        }
        public async Task DeleteField(int id)
        {
            var field = await db.Field.FindAsync(id);
            if (field != null)
                db.Field.Remove(field);
        }
    }
}
