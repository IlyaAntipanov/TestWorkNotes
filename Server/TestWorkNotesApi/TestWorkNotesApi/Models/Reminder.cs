namespace TestWorkNotesApi.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public int NoteId { set; get; }
        public DateTime ReminderDateTime {  set; get; }

        public virtual Note Note { get; set; }
    }
}
