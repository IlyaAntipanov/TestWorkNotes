using Microsoft.AspNetCore.Mvc;
using TestWorkNotesApi.Models;
using TestWorkNotesApi.ModelsView;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestWorkNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        // GET: api/<TagController>
        [HttpGet]
        public IEnumerable<TagView> Get()
        {
            using NotesDb db = new NotesDb();
            return db.Tags.Select(s => new TagView(s)).ToList();
        }

        // GET api/<TagController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using NotesDb db = new NotesDb();
            var tag = db.Tags.FirstOrDefault(w => w.Id == id);
            if (tag == null)
                return NotFound();
            return Ok(tag);
        }

        // POST api/<TagController>
        [HttpPost]
        public int Post([FromBody] string name)
        {
            using NotesDb db = new NotesDb();
            var tag = new Tag()
            {
                Name = name
            };
            db.Tags.Add(tag);
            db.SaveChanges();
            return tag.Id;
        }

        // PUT api/<TagController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string name)
        {
            using NotesDb db = new NotesDb();
            var tag = db.Tags.FirstOrDefault(s => s.Id == id);
            if(tag == null)
                return NotFound();
            tag.Name = name;
            db.SaveChanges();
            return Ok();
        }

        // DELETE api/<TagController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using NotesDb db = new NotesDb();
            var tag = db.Tags.FirstOrDefault(s => s.Id == id);
            if (tag == null)
                return NotFound();
            db.Tags.Remove(tag);
            db.SaveChanges();
            return Ok();
        }
    }
}
