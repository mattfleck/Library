using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryData;
using LibraryData.Models;

namespace ASPCoreWebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/LibraryAssetts")]
    public class LibraryAssettsController : Controller
    {
        private readonly LibraryDbContext _context;

        public LibraryAssettsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/LibraryAssetts
        [HttpGet]
        public IEnumerable<LibraryAsset> GetLibraryAssets()
        {
            return _context.LibraryAssets;
        }

        // GET: api/LibraryAssetts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLibraryAssett([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var libraryAssett = await _context.LibraryAssets.SingleOrDefaultAsync(m => m.Id == id);

            if (libraryAssett == null)
            {
                return NotFound();
            }

            return Ok(libraryAssett);
        }

        // PUT: api/LibraryAssetts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLibraryAssett([FromRoute] int id, [FromBody] LibraryAsset libraryAssett)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != libraryAssett.Id)
            {
                return BadRequest();
            }

            _context.Entry(libraryAssett).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibraryAssettExists(id))
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

        // POST: api/LibraryAssetts
        [HttpPost]
        public async Task<IActionResult> PostLibraryAssett([FromBody] LibraryAsset libraryAssett)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LibraryAssets.Add(libraryAssett);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLibraryAssett", new { id = libraryAssett.Id }, libraryAssett);
        }

        // DELETE: api/LibraryAssetts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLibraryAssett([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var libraryAssett = await _context.LibraryAssets.SingleOrDefaultAsync(m => m.Id == id);
            if (libraryAssett == null)
            {
                return NotFound();
            }

            _context.LibraryAssets.Remove(libraryAssett);
            await _context.SaveChangesAsync();

            return Ok(libraryAssett);
        }

        private bool LibraryAssettExists(int id)
        {
            return _context.LibraryAssets.Any(e => e.Id == id);
        }
    }
}