using TestWorkNotesApi.Models;

namespace TestWorkNotesApi.ModelsView
{
    public class ReminderView
    {
        public int Id { get; set; }
        public int NoteId { set; get; }
        public DateTime ReminderDateTime { set; get; }
        public ReminderView(Reminder reminder)
        {
            Id = reminder.Id;
            NoteId = reminder.NoteId;
            ReminderDateTime = reminder.ReminderDateTime;
        }
    }
}
