using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorkNotesClientWpf.ModelsView;

namespace TestWorkNotesClientWpf.Models
{
    public class Note : INotifyPropertyChanged
    {
        private string body;
        private DateTime createdDate;
        private ObservableCollection<int> tagsId = new ObservableCollection<int>();
        private int id;
        private string title;
        public bool IsActiveChanged { private set; get; } = true;
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id
        {
            get => id; set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string Title
        {
            get => title; set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Body
        {
            get => body; set
            {
                body = value;
                OnPropertyChanged(nameof(Body));
            }
        }
        public DateTime CreatedDate
        {
            get => createdDate; set
            {
                createdDate = value;
                OnPropertyChanged(nameof(CreatedDate));
            }
        }
        public ObservableCollection<int> TagIds
        {
            get => tagsId; set
            {
                tagsId.CollectionChanged -= TagIds_CollectionChanged;
                tagsId = value;
                tagsId.CollectionChanged += TagIds_CollectionChanged;
                UpdateTag();
            }
        }

        private void TagIds_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateTag();
        }

        public void UpdateTag()
        {
            OnPropertyChanged(nameof(TagIds));
            OnPropertyChanged(nameof(Tags));
        }

        public List<TagModelView> Tags 
        {
            get
            {
                return TagIds.Select(s => new TagModelView(MainWindow.Tags.FirstOrDefault(f => f.Id == s), Id)).ToList();
            } 
        }
        public bool IsLoad { set; get; } = false;

        public Note(NoteParticle noteParticle) : this()
        {
            Id = noteParticle.Id;
            Title = noteParticle.Title;
        }
        public Note()
        {
            tagsId.CollectionChanged += TagIds_CollectionChanged;
        }

        public async void OnLoad()
        {
            IsActiveChanged = false;
            var note = await HttpClientApi.GetAsync<NoteView>($"{MainWindow.BasePath}/api/Note/{Id}");
            if (note != null) 
            {
                IsLoad = true;
                Body = note.Body;
                CreatedDate=note.CreatedDate;
                for( int i = 0; i < note.Tags.Count; i++ )
                {
                    TagIds.Add(note.Tags[i]);
                }
                
            }
            IsActiveChanged = true;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
