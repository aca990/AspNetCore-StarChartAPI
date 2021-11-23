using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
                return NotFound();
            celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (celestialObjects.Count() == 0)
                return NotFound();
            celestialObjects.ForEach(item => item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList());
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            celestialObjects.ForEach(item => item.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == item.Id).ToList());

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var foundCelestialObject = _context.CelestialObjects.Find(id);
            if (foundCelestialObject == null)
                return NotFound();
            foundCelestialObject.Name = celestialObject.Name;
            foundCelestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(foundCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var foundCelestialObject = _context.CelestialObjects.Find(id);
            if (foundCelestialObject == null)
                return NotFound();
            foundCelestialObject.Name = name;
            _context.CelestialObjects.Update(foundCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var foundCelestialObject = _context.CelestialObjects.Find(id);
            if (foundCelestialObject == null)
                return NotFound();

            _context.RemoveRange(foundCelestialObject);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
