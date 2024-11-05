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

        public CoreSampleImagesController(CsharpBackendContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        // GET: api/CoreSampleImages
        [HttpGet]
        [Route("images/get")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImage()
        {
            return await _context.CoreSampleImage.Include(image => image.ImageInfo).ToListAsync();
        }

        // GET: api/CoreSampleImages/5
        [HttpGet("images/getitem")]
        public async Task<ActionResult<CoreSampleImage>> GetCoreSampleImage(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);

            if (coreSampleImage == null)
            {
                return NotFound();
            }

            return coreSampleImage;
        }

        [HttpGet]
        [Route("images/getwithmask")]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImageWithMask()
        {
            return await _context.CoreSampleImage.Include(image => image.PathToMask != null).ToListAsync();
        }

        // PUT: api/CoreSampleImages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("images/putitem/{id}")]
        public async Task<IActionResult> PutImageInfo(int id, [FromForm] ImageInfo imageInfo)
        {
            if (id != imageInfo.Id)
            {
                return BadRequest();
            }

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
        [Route("images/delete")]
        public async Task<IActionResult> DeleteCoreSampleImage(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);
            if (coreSampleImage == null)
            {
                return NotFound();
            }

            coreSampleImage.DeleteItemFiles();

            _context.CoreSampleImage.Remove(coreSampleImage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CoreSampleImageExists(int id)
        {
            return _context.CoreSampleImage.Any(e => e.Id == id);
        }

        // POST: api/CoreSampleImages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*        [HttpPost]
                public async Task<ActionResult<CoreSampleImage>> PostCoreSampleImage(CoreSampleImage coreSampleImage)
                {
                    _context.CoreSampleImage.Add(coreSampleImage);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetCoreSampleImage", new { id = coreSampleImage.Id }, coreSampleImage);
                }*/

        [HttpPost]
        [Route("images/upload")]
        async public Task<ActionResult<CoreSampleImage>> UploadImage(IFormFile file,
            [FromForm] ImageInfo imageInfo)
        {
            if (file != null)
            {
                var coreSampleImage = new CoreSampleImage();

                using (var stream = new FileStream(coreSampleImage.PathToImage, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                coreSampleImage.ImageInfo = imageInfo;

                _context.CoreSampleImage.Add(coreSampleImage);
                //_context.ImageInfo.Add(imageInfo);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetCoreSampleImage", new { id = coreSampleImage.Id }, coreSampleImage);
            }
            return NoContent();
        }
    }
}
