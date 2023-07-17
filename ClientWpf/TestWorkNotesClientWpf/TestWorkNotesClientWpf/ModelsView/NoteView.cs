using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkNotesClientWpf.Models;

namespace TestWorkNotesClientWpf.ModelsView
{
    public class NoteView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { set; get; }
        public DateTime CreatedDate { get; set; }
        public List<int> Tags { get; set; }
    }
}
