using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWorkNotesClientWpf.Models
{
    public class Reminder : INotifyPropertyChanged
    {
        private int id;
        private int noteId;
        private DateTime reminderDateTime;

        public int Id
        {
            get => id; set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public int NoteId
        {
            get => noteId; set
            {
                noteId = value;
                OnPropertyChanged(nameof(NoteId));
            }
        }
        public DateTime ReminderDateTime
        {
            get => reminderDateTime; set
            {
                reminderDateTime = value;
                OnPropertyChanged(nameof(ReminderDateTime));
            }
        }

        public Note Note
        {
            get
            {
                return MainWindow.Notes.FirstOrDefault(f => f.Id == NoteId);
            }
            set
            {
                NoteId = value.Id;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
