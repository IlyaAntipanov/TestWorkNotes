using Microsoft.AspNetCore.Mvc;
using TestWorkNotesApi.Models;
using TestWorkNotesApi.ModelsInput;
using TestWorkNotesApi.ModelsView;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestWorkNotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        // GET: api/<ReminderController>
        [HttpGet]
        public IEnumerable<ReminderView> Get()
        {
            using NotesDb db = new NotesDb();
            return db.Reminders.Select(s => new ReminderView(s)).ToList();
        }

        // POST api/<ReminderController>
        [HttpPost]
        public int Post([FromBody] ReminderInput reminderInput)
        {
            using NotesDb db = new NotesDb();
            if (db.Notes.FirstOrDefault(f => f.Id == reminderInput.NoteId) == null)
                return -1;
            var reminder = new Reminder()
            {
                NoteId = reminderInput.NoteId,
                ReminderDateTime = reminderInput.ReminderDateTime.ToLocalTime().ToUniversalTime()
            };
            db.Reminders.Add(reminder);
            db.SaveChanges();
            return reminder.Id;
        }

        // PUT api/<ReminderController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ReminderInput reminderInput)
        {
            using NotesDb db = new NotesDb();
            var reminder = db.Reminders.FirstOrDefault(f => f.Id == id);
            if (reminder == null)
                return NotFound();
            reminder.NoteId = reminderInput.NoteId;
            reminder.ReminderDateTime = reminderInput.ReminderDateTime.ToLocalTime().ToUniversalTime();
            db.SaveChanges();
            return Ok();
        }

        // DELETE api/<ReminderController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using NotesDb db = new NotesDb();
            var reminder = db.Reminders.FirstOrDefault(f => f.Id == id);
            if (reminder == null)
                return NotFound();
            db.Reminders.Remove(reminder);
            db.SaveChanges();
            return Ok();
        }
    }
}
