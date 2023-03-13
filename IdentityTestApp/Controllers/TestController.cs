using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityTestApp.EntitiesAndModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityTestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public TestController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var test = _context.TestDbs.ToList();
            return Ok(test);
        }
        
        [HttpPost]
        public IActionResult Post(TestDb testDb)
        {
            _context.TestDbs.Add(testDb);
            _context.SaveChanges();
            return Ok();
        }
    }
}
