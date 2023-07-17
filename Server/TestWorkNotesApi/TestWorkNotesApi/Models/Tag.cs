namespace TestWorkNotesApi.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { set; get; }
        public virtual ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>();
    }
}
