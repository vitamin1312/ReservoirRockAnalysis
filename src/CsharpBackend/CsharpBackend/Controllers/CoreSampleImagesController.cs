using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsharpBackend.Data;
using CsharpBackend.Models;

namespace CsharpBackend
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoreSampleImagesController : ControllerBase
    {
        private readonly CsharpBackendContext _context;
        private int MaxFileLength = 15 * 1024 * 1024;

        public CoreSampleImagesController(CsharpBackendContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        [HttpPost]
        [Route("upload")]
        async public Task<ActionResult<CoreSampleImage>> UploadImage(IFormFile file,
            [FromForm] ImageInfo imageInfo)
        {

            if (file.Length > MaxFileLength)
                return StatusCode(StatusCodes.Status413PayloadTooLarge,
                    new { message = "File is too large" });

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
                return StatusCode(StatusCodes.Status415UnsupportedMediaType,
                    new { message = "Unsupported media type" });

            if (file == null)
                return NoContent();

            var coreSampleImage = new CoreSampleImage();

            using (var stream = new FileStream(coreSampleImage.PathToImage, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            checkFieldInInfo(imageInfo);

            coreSampleImage.ImageInfo = imageInfo;

            _context.CoreSampleImage.Add(coreSampleImage);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCoreSampleImage", new { id = coreSampleImage.Id }, coreSampleImage);
            
        }

        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImage()
        {
            return await _context.CoreSampleImage.Include(image => image.ImageInfo).ToListAsync();
        }

        [HttpGet("getitem/{id}")]
        public async Task<ActionResult<CoreSampleImage>> GetCoreSampleImage(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);

            if (coreSampleImage == null)
            {
                return NotFound();
            }

            return coreSampleImage;
        }

        [HttpGet("getfromfield/{fieldId}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageFromField(int fieldId)
        {
            return await _context.CoreSampleImage
                                                .Include(image => image.ImageInfo)
                                                .Where(image => image.ImageInfo.FieldId == fieldId)
                                                .ToListAsync();
        }

        [HttpGet("getwithmask")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageWithMask()
        {
            return await _context.CoreSampleImage
                                                .Include(image => image.ImageInfo)
                                                .Where(image => image.PathToMask != null)
                                                .ToListAsync();
        }

        [HttpGet("getwithoutmask")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageWithoutMask()
        {
            return await _context.CoreSampleImage
                                                .Include(image => image.ImageInfo)
                                                .Where(image => image.PathToMask == null)
                                                .ToListAsync();
        }


        [HttpGet("getimagefile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageFile(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);

            if (coreSampleImage == null)
                return NotFound();

            var result = PhysicalFile(coreSampleImage.PathToImage, "image/jpeg");

            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Can't read file" });
            return result;

        }

        [HttpGet("getmaskfile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetMaskFile(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);

            if (coreSampleImage == null || coreSampleImage.PathToMask == null)
                return NotFound();

            try
            {
                return PhysicalFile(coreSampleImage.PathToMask, "image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Can't read file" });
            }
        }

        [HttpGet("getmaskimagefile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetMaskImageFile(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);

            if (coreSampleImage == null || coreSampleImage.PathToMask == null)
                return NotFound();

            try
            {
                return PhysicalFile(coreSampleImage.PathToMask, "image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }



        // PUT: api/CoreSampleImages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("putitem/{id}")]
        public async Task<IActionResult> PutImageInfo(int id, [FromForm] ImageInfo imageInfo)
        {
            if (id != imageInfo.Id)
            {
                return BadRequest();
            }

            checkFieldInInfo(imageInfo);

            _context.Entry(imageInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoreSampleImageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/CoreSampleImages/5
        [HttpDelete]
        [Route("deleteitem/{id}")]
        public async Task<IActionResult> DeleteCoreSampleImage(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage
                .Include(ci => ci.ImageInfo)
                .FirstOrDefaultAsync(ci => ci.Id == id);

            if (coreSampleImage == null)
            {
                return NotFound();
            }

            coreSampleImage.DeleteItemFiles();
            var imageInfo = coreSampleImage.ImageInfo;

            _context.ImageInfo.Remove(imageInfo);
            _context.CoreSampleImage.Remove(coreSampleImage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CoreSampleImageExists(int id)
        {
            return _context.CoreSampleImage.Any(e => e.Id == id);
        }

        private bool FieldExists(int id)
        {
            return _context.Field.Any(e => e.Id == id);
        }

        private void checkFieldInInfo (ImageInfo imageInfo)
        {
            int fieldId = Convert.ToInt32(imageInfo.FieldId);
            if (imageInfo.FieldId != null && !FieldExists(fieldId))
                imageInfo.FieldId = null;
        }
    }
}
