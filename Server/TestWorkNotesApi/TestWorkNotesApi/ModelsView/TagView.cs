using TestWorkNotesApi.Models;

namespace TestWorkNotesApi.ModelsView
{
    public class TagView
    {
        public int Id { get; set; }
        public string Name { set; get; }

        public TagView(Tag tag)
        {
            Id = tag.Id;
            Name = tag.Name;
        }
    }
}
