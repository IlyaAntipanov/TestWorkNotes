namespace TestWorkNotesApi.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { set;get; }
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>();
        public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
}
