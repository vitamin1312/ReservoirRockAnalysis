
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsharpBackend.Data;
using CsharpBackend.Models;
using CsharpBackend.Repository;
using Microsoft.AspNetCore.Cors;

namespace CsharpBackend.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class FieldsController : ControllerBase
    {
        private readonly IImageRepository repository;

        public FieldsController(IImageRepository context)
        {
            repository = context;
        }

        // POST: api/Fields
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Field>> PostField([FromBody] Field @field)
        {
            await repository.CreateField(@field);
            await repository.Save();

            return CreatedAtAction("GetField", new { id = @field.Id }, @field);
        }

        // GET: api/Fields
        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<IEnumerable<Field>>> GetField()
        {
            return await repository.GetFieldsList();
        }

        // GET: api/Fields/5
        [HttpGet("getitem/{id}")]
        public async Task<ActionResult<Field>> GetField(int id)
        {
            var @field = await repository.GetField(id);

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

            repository.UpdateField(@field);

            try
            {
                await repository.Save();
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
            var @field = await repository.GetField(id);
            if (@field == null)
            {
                return NotFound();
            }

            await repository.DeleteField(id);
            await repository.Save();

            return NoContent();
        }

        private bool FieldExists(int id)
        {
            return repository.FieldExists(id);
        }
    }
}
