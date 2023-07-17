using TestWorkNotesApi.Models;

namespace TestWorkNotesApi.ModelsView
{
    public class NoteParticleView
    {
        public int Id { set; get; }
        public string Title { set; get; }

        public NoteParticleView(Note note)
        {
            Id = note.Id;
            Title = note.Title;
        }
    }
}
