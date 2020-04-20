using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var obj = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if (obj != null)
            {
                var orbitedObjects = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
                if (orbitedObjects != null && orbitedObjects.Any())
                {
                    obj.Satellites.AddRange(orbitedObjects);
                }
                return this.Ok(obj);
            }
            return this.NotFound();
        }

        [HttpGet("{name}", Name = "GetByName")]
        public IActionResult GetByName(string name)
        {
            var allObjects = _context.CelestialObjects.ToList();
            var allNamedObjects = _context.CelestialObjects.Where(x => x.Name == name).ToList();
            if (allNamedObjects != null && allNamedObjects.Any())
            {
                foreach (var obj in allNamedObjects)
                {
                    var satellites = allObjects.Where(x => x.OrbitedObjectId == obj.Id).ToList();
                    if (satellites != null && satellites.Any())
                        obj.Satellites.AddRange(satellites);
                }
                return this.Ok(allNamedObjects);
            }
            return this.NotFound();
        }
        [HttpGet(Name = "GetAll")]
        public IActionResult GetAll()
        {
            var allObjects = _context.CelestialObjects.ToList();
            foreach (var obj in allObjects)
            {
                var satellites = allObjects.Where(x => x.OrbitedObjectId == obj.Id).ToList();
                if (satellites != null && satellites.Any())
                    obj.Satellites.AddRange(satellites);
            }
            return this.Ok(allObjects);
        }

        [HttpPost()]
        public IActionResult Create([FromBody]CelestialObject obj)
        {
            if (obj == null)
            {
                return this.BadRequest(obj);
            }

            _context.CelestialObjects.Add(obj);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = obj.Id }, obj);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject newObj)
        {
            var obj = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if (obj != null)
            {
                obj.Name = newObj.Name;
                obj.OrbitalPeriod = newObj.OrbitalPeriod;
                obj.OrbitedObjectId = newObj.OrbitedObjectId;

                _context.CelestialObjects.Update(obj);

                _context.SaveChanges();

                return this.NoContent();
            }
            return this.NotFound();

        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var obj = _context.CelestialObjects.Where(x => x.Id == id).FirstOrDefault();
            if (obj != null)
            {
                obj.Name = name;

                _context.CelestialObjects.Update(obj);

                _context.SaveChanges();

                return this.NoContent();
            }
            return this.NotFound();

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objectsToDelete = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();
            if (objectsToDelete != null && objectsToDelete.Any())
            {
                _context.CelestialObjects.RemoveRange(objectsToDelete);
                _context.SaveChanges();
                return this.NoContent();
            }
            return this.NotFound();

        }
    }
}
