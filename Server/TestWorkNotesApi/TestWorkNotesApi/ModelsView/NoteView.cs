using TestWorkNotesApi.Models;

namespace TestWorkNotesApi.ModelsView
{
    public class NoteView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { set; get; }
        public DateTime CreatedDate { get; set; }
        public List<int> Tags { get; set; }
        public NoteView(Note note)
        {
            Id = note.Id;
            Title = note.Title;
            Body = note.Body;
            CreatedDate = note.CreatedDate;
            Tags = note.NoteTags.Select(s => s.TagId).ToList();
        }
    }
}
