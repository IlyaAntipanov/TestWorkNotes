using Microsoft.AspNetCore.Mvc;
using TestWorkNotesApi.Models;
using TestWorkNotesApi.ModelsInput;
using TestWorkNotesApi.ModelsView;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestWorkNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        // GET: api/<Note>
        [HttpGet]
        public IEnumerable<NoteParticleView> Get()
        {
            using NotesDb db = new NotesDb();
            return db.Notes.Select(s => new NoteParticleView(s)).ToList();
        }

        // GET api/<Note>/5
        [HttpGet("{id}")]
        public NoteView? Get(int id)
        {
            using NotesDb db = new NotesDb();
            var note = db.Notes.First(w => w.Id == id);
            if (note == null)
                return null;
            return new NoteView(note);
        }

        // POST api/<Note>
        [HttpPost]
        public int Post([FromBody] NoteInput noteInput)
        {
            using NotesDb db = new NotesDb();
            Note note = new Note
            {
                Title = noteInput.Title,
                Body = noteInput.Body,
                CreatedDate = DateTime.UtcNow
            };
            db.Notes.Add(note);
            db.SaveChanges();
            foreach (int tagId in noteInput.Tags)
            {
                Tag tag = db.Tags.FirstOrDefault(t => t.Id == tagId);
                if (tag != null)
                {
                    note.NoteTags.Add(new NoteTag() { TagId = tag.Id, NoteId = note.Id });
                }
            }
            db.SaveChanges();
            return note.Id;
        }

        // PUT api/<Note>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] NoteInput noteInput)
        {
            using NotesDb db = new NotesDb();
            var note = db.Notes.FirstOrDefault(f => f.Id == id);
            if (note == null) return NotFound();
            note.Title = noteInput.Title;
            note.Body = noteInput.Body;
            db.NoteTags.RemoveRange(note.NoteTags);
            foreach (int tagId in noteInput.Tags)
            {
                Tag tag = db.Tags.FirstOrDefault(t => t.Id == tagId);
                if (tag != null)
                {
                    db.NoteTags.Add(new NoteTag() { TagId = tag.Id, NoteId = note.Id });
                }
            }
            db.SaveChanges();
            return Ok();
        }

        // DELETE api/<Note>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using NotesDb db = new NotesDb();
            var note = db.Notes.FirstOrDefault(f => f.Id == id);
            if (note == null) return NotFound();
            db.NoteTags.RemoveRange(note.NoteTags);
            db.Notes.Remove(note);
            db.SaveChanges();
            return Ok();
        }
    }
}
