
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsharpBackend.Data;
using CsharpBackend.Models;

namespace CsharpBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldsController : ControllerBase
    {
        private readonly CsharpBackendContext _context;

        public FieldsController(CsharpBackendContext context)
        {
            _context = context;
        }

        // POST: api/Fields
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Field>> PostField([FromForm] Field @field)
        {
            _context.Field.Add(@field);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetField", new { id = @field.Id }, @field);
        }

        // GET: api/Fields
        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<IEnumerable<Field>>> GetField()
        {
            return await _context.Field.ToListAsync();
        }

        // GET: api/Fields/5
        [HttpGet("getitem/{id}")]
        public async Task<ActionResult<Field>> GetField(int id)
        {
            var @field = await _context.Field.FindAsync(id);

            if (@field == null)
            {
                return NotFound();
            }

            return @field;
        }

        // PUT: api/Fields/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("putitem/{id}")]
        public async Task<IActionResult> PutField(int id, [FromForm] Field @field)
        {
            if (id != @field.Id)
            {
                return BadRequest();
            }

            _context.Entry(@field).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FieldExists(id))
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

        // DELETE: api/Fields/5
        [HttpDelete("deleteitem/{id}")]
        public async Task<IActionResult> DeleteField(int id)
        {
            var @field = await _context.Field.FindAsync(id);
            if (@field == null)
            {
                return NotFound();
            }

            _context.Field.Remove(@field);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FieldExists(int id)
        {
            return _context.Field.Any(e => e.Id == id);
        }
    }
}
