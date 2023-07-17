using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkNotesClientWpf.Models;

namespace TestWorkNotesClientWpf.ModelsView
{
    public class TagModelView:Tag
    {
        public int NoteId { set; get; }
        public TagModelView(Tag tag, int noteId)
        {
            this.Id = tag.Id;
            this.Name = tag.Name;
            NoteId = noteId;
        }
    }
}
