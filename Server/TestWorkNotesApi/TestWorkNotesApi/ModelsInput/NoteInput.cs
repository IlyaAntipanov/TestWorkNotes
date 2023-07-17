using System.Runtime.Serialization;

namespace TestWorkNotesApi.ModelsInput
{
    public class NoteInput
    {
        public string Title { set; get; }
        public string Body { set; get; }
        public List<int> Tags { set; get; }
    }
}
