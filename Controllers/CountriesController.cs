using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WorldCities.Data;
using WorldCities.Data.Models;

using System.Linq.Dynamic.Core;

namespace WorldCountries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Countries
        // GET: api/Countries/?pageIndex=0&pageSize=10
        // GET: api/Countries/?pageIndex=0&pageSize=10&sortColumn=name&sortOrder=asc&filterColumn=name&filterQuery=query
        [HttpGet]
        public async Task<ActionResult<ApiResult<Country>>> GetCountries(
            int pageIndex = 0,
            int pageSize = 10,
            string sortColumn = null,
            string sortOrder = null,
            string filterColumn = null,
            string filterQuery = null)
        {
            return await ApiResult<Country>.CreateAsync(
                _context.Countries,
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetCountry(int id)
        {
            var Country = await _context.Countries.FindAsync(id);

            if (Country == null)
            {
                return NotFound();
            }

            return Country;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, please enable the
        // specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, Country Country)
        {
            if (id != Country.Id)
            {
                return BadRequest();
            }

            _context.Entry(Country).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
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

        // POST: api/Countries
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(Country Country)
        {
            _context.Countries.Add(Country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = Country.Id }, Country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Country>> DeleteCountry(int id)
        {
            var Country = await _context.Countries.FindAsync(id);
            if (Country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(Country);
            await _context.SaveChangesAsync();

            return Country;
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }

        [HttpPost]
        [Route("IsDupeField")]
        public bool IsDupeField(int countryId, string fieldName, string fieldValue)
        {
            // default (faster method)
            //switch (fieldName)
            //{
            //    case "name":
            //        return _context.Countries.Any(c => c.Name == fieldValue && c.Id != countryId);
            //    case "iso2":
            //        return _context.Countries.Any(c => c.ISO2 == fieldValue && c.Id != countryId);
            //    case "iso3":
            //        return _context.Countries.Any(c => c.ISO3 == fieldValue && c.Id != countryId);
            //    default:
            //        return false;
            //}

            // more reusable method using System.Linq.Dynamic.Core;
            return (ApiResult<Country>.IsValidProperty(fieldName, true)) 
                ? _context.Countries.Any(string.Format("{0} == @0 && Id != @1", fieldName), fieldValue, countryId) 
                : false;
        }

    }
}
