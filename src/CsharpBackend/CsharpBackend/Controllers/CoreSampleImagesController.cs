using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsharpBackend.Data;
using CsharpBackend.Models;
using CsharpBackend.NeuralNetwork;
using Emgu.CV.Structure;
using Emgu.CV;
using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Authorization;

namespace CsharpBackend
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoreSampleImagesController : ControllerBase
    {
        private readonly CsharpBackendContext _context;
        private int MaxFileLength = 15 * 1024 * 1024;
        private INetworkManager _networkManager;

        public CoreSampleImagesController(CsharpBackendContext context, INetworkManager networkManager)
        {
            _context = context;
            _context.Database.EnsureCreated();
            _networkManager = networkManager;
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
        [Authorize]
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

            return PhysicalFile(coreSampleImage.PathToImage, "image/jpeg");



        }

        [HttpGet("getmaskfile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetMaskFile(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);

            if (coreSampleImage == null || coreSampleImage.PathToMask == null)
                return NotFound();

            return PhysicalFile(coreSampleImage.PathToMask, "image/png");
        }


        [HttpGet("getmaskimagefile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetMaskImageFile(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);

            if (coreSampleImage == null)
                return NotFound();

            var maskImage = coreSampleImage.GetMaskImageMat();

            if (maskImage == null)
                return NotFound();

            var tempFiles = new TempFileCollection();
            string file = tempFiles.AddExtension("jpg");
            maskImage.Save(file);
            return PhysicalFile(file, "image/jpeg");
        }

        [HttpGet("getimagewithmaskfile/{id}")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetImageWithMaskFile(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);

            if (coreSampleImage == null)
                return NotFound();

            var maskImage = coreSampleImage.GetImageWithMaskMat();

            if (maskImage == null)
                return NotFound();

            var tempFiles = new TempFileCollection();
            string file = tempFiles.AddExtension("jpg");
            maskImage.Save(file);
            return PhysicalFile(file, "image/jpeg");
        }

        [HttpGet("predict/{id}")]
        public async Task<ActionResult<CoreSampleImage>> PredictMaskForItem(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);

            if (coreSampleImage == null)
            {
                return NotFound();
            }
            
            var pathToMask = coreSampleImage.GenerateMaskPath();

            _networkManager.Predict(coreSampleImage.PathToImage, pathToMask);
            coreSampleImage.PathToMask = pathToMask;

            _context.Entry(coreSampleImage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                    throw;
            }

            return coreSampleImage;
        }



        // PUT: api/CoreSampleImages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("putitem/{id}")]
        [Authorize]
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
        [Authorize(Roles = "admin")]
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
