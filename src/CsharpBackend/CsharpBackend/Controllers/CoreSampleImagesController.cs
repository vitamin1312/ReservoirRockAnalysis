using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        }

        // GET: api/CoreSampleImages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoreSampleImage>>> GetCoreSampleImage()
        {
            return await _context.CoreSampleImage.ToListAsync();
        }

        // GET: api/CoreSampleImages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CoreSampleImage>> GetCoreSampleImage(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);

            if (coreSampleImage == null)
            {
                return NotFound();
            }

            return coreSampleImage;
        }

        // PUT: api/CoreSampleImages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoreSampleImage(int id, CoreSampleImage coreSampleImage)
        {
            if (id != coreSampleImage.Id)
            {
                return BadRequest();
            }

            _context.Entry(coreSampleImage).State = EntityState.Modified;

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

        // POST: api/CoreSampleImages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CoreSampleImage>> PostCoreSampleImage(CoreSampleImage coreSampleImage)
        {
            _context.CoreSampleImage.Add(coreSampleImage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCoreSampleImage", new { id = coreSampleImage.Id }, coreSampleImage);
        }

        // DELETE: api/CoreSampleImages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoreSampleImage(int id)
        {
            var coreSampleImage = await _context.CoreSampleImage.FindAsync(id);
            if (coreSampleImage == null)
            {
                return NotFound();
            }

            _context.CoreSampleImage.Remove(coreSampleImage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CoreSampleImageExists(int id)
        {
            return _context.CoreSampleImage.Any(e => e.Id == id);
        }
    }
}
