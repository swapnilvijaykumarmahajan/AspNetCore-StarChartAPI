using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
            var obj = _context.CelestialObjects.Where(x => x.Name == name).FirstOrDefault();
            if (obj != null)
            {
                var orbitedObjects = _context.CelestialObjects.Where(x => x.OrbitedObjectId == obj.Id).ToList();
                if (orbitedObjects != null && orbitedObjects.Any())
                {
                    obj.Satellites.AddRange(orbitedObjects);
                }
                return this.Ok(obj);
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
    }
}
