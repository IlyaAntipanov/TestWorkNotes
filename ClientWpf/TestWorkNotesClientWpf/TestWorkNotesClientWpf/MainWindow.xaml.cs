using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestWorkNotesClientWpf.Models;
using TestWorkNotesClientWpf.ModelsView;

namespace TestWorkNotesClientWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static ObservableCollection<Tag> Tags { set; get; } = new ObservableCollection<Tag>();
        //public ObservableCollection<NoteParticle> NoteParticles { set; get; }
        public static ObservableCollection<Note> Notes { set; get; } = new ObservableCollection<Note>();
        public ObservableCollection<Reminder> Reminders { set; get; } = new ObservableCollection<Reminder>();
        public ObservableCollection<string> Log { set; get; } = new ObservableCollection<string>();
        public ObservableCollection<Tag> TagNewNote { set; get; } = new ObservableCollection<Tag>();
        public static string BasePath = "https://localhost:7097";
        private bool IsLoadTags = false;
        private bool IsLoadNotes = false;
        private bool IsLoadReminders = false;
        public MainWindow()
        {
            InitializeComponent();
            Load();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public async void Load()
        {
            HttpClientApi.LogEvent += Log.Add;
            Tags.CollectionChanged += Tags_CollectionChanged;
            Notes.CollectionChanged += Notes_CollectionChanged;
            Reminders.CollectionChanged += Reminders_CollectionChanged;
            var list = await HttpClientApi.GetAsync<List<Tag>>($"{BasePath}/api/Tag");
            for (int i = 0; i < list.Count; i++)
            {
                Tags.Add(list[i]);
            }
            IsLoadTags = true;
        }

        private async void Reminders_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems?[0] is Reminder newReminder)
                    {
                        if (IsLoadReminders)
                            newReminder.Id = await HttpClientApi.PostAsync<int>($"{BasePath}/api/Reminder", JsonContent.Create(new
                            {
                                newReminder.NoteId,
                                newReminder.ReminderDateTime
                            }));
                        newReminder.PropertyChanged += async (sender, e) =>
                        {

                            await HttpClientApi.PutAsync($"{BasePath}/api/Reminder/{newReminder.Id}", JsonContent.Create(new
                            {
                                newReminder.NoteId,
                                newReminder.ReminderDateTime
                            }));
                        };
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems?[0] is Reminder oldReminder)
                    {
                        await HttpClientApi.DeleteAsync($"{BasePath}/api/Reminder/{oldReminder.Id}");
                    }
                    break;
            }
        }

        private async void Notes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems?[0] is Note newNote)
                    {
                        if (IsLoadNotes)
                            newNote.Id = await HttpClientApi.PostAsync<int>($"{BasePath}/api/Note", JsonContent.Create(new
                            {
                                newNote.Title,
                                newNote.Body,
                                Tags = newNote.TagIds
                            }));
                        newNote.PropertyChanged += async (sender, e) =>
                        {
                            if (newNote.IsActiveChanged && e.PropertyName != nameof(newNote.Tags))
                                await HttpClientApi.PutAsync($"{BasePath}/api/Note/{newNote.Id}", JsonContent.Create(new
                                {
                                    newNote.Title,
                                    newNote.Body,
                                    Tags = newNote.TagIds
                                }));
                        };
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems?[0] is Note oldNote)
                    {
                        await HttpClientApi.DeleteAsync($"{BasePath}/api/Note/{oldNote.Id}");
                    }
                    break;
            }
        }

        private async void Tags_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    if (e.NewItems?[0] is Tag newTag)
                    {
                        if (IsLoadTags)
                            newTag.Id = await HttpClientApi.PostAsync($"{BasePath}/api/Tag", newTag.Name);
                        newTag.PropertyChanged += async (sendenr, e) =>
                        {
                            if (e.PropertyName == nameof(newTag.Name))
                            {
                                Notes.Where(w => w.TagIds.Any(a => a == newTag.Id)).ToList().ForEach(f => f.UpdateTag());
                                await HttpClientApi.PutAsync($"{BasePath}/api/Tag/{newTag.Id}", new StringContent($"\"{newTag.Name}\"", Encoding.UTF8, "application/json"));
                            }
                        };
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.OldItems?[0] is Tag oldTag)
                    {
                        await HttpClientApi.DeleteAsync($"{BasePath}/api/Tag/{oldTag.Id}");
                    }
                    break;
            }
        }

        private async void TabItemNote_Selected(object sender, RoutedEventArgs e)
        {
            await NoteLoad();
        }

        private async Task NoteLoad()
        {
            if (!IsLoadNotes)
            {
                var noteParticles = await HttpClientApi.GetAsync<List<NoteParticle>>($"{BasePath}/api/Note");
                for (int i = 0; i < noteParticles.Count; i++) 
                {
                    Notes.Add(new Note(noteParticles[i]));
                }
                IsLoadNotes = true;
            }
        }

        private async Task RemindersLoad()
        {
            if (!IsLoadReminders)
            {
                var reminders = await HttpClientApi.GetAsync<List<Reminder>>($"{BasePath}/api/Reminder");
                for (int i = 0; i < reminders.Count; i++)
                {
                    Reminders.Add(reminders[i]);
                }
                IsLoadReminders = true;
            }
        }

        private async void TabItemReminder_Selected(object sender, RoutedEventArgs e)
        {
            await NoteLoad();
            await RemindersLoad();
        }

        private void ButtonAddTag_Click(object sender, RoutedEventArgs e)
        {
            Tags.Add(new Tag() { Name = TextBoxNewTagName.Text });
        }

        private void ButtonDeleteTag_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is int id)
            {
                var tag = Tags.Where(w => w.Id == id).FirstOrDefault();
                if (tag != null)
                {
                    Tags.Remove(tag);
                }
            }
        }

        private void ButtonAttachTag_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxSelectTag.SelectedItem is Tag tag)
            {
                if (TagNewNote.Where(w => w == tag).Count() == 0)
                    TagNewNote.Add(tag);
            }        
        }

        private void ButtonRemoveTagNewNote_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is int id)
            {
                var tag = TagNewNote.Where(w => w.Id == id).FirstOrDefault();
                if (tag != null)
                {
                    TagNewNote.Remove(tag);
                }
            }
        }

        private void ButtonAddNote_Click(object sender, RoutedEventArgs e)
        {
            var note = new Note()
            {
                Title = NewNoteTitleTextBox.Text,
                Body = NewNoteTextTextBox.Text,
                CreatedDate = new DateTime(),
                IsLoad = true
            };
            for(int i = 0; i < TagNewNote.Count; i++) 
            {
                note.TagIds.Add(TagNewNote[i].Id);
            }
            Notes.Add(note);
        }

        private void ButtonDeleteNote_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is int id)
            {
                var note = Notes.Where(w => w.Id == id).FirstOrDefault();
                if (note != null)
                {
                    Notes.Remove(note);
                }
            }
        }

        private void ExpanderOpenNote_Expanded(object sender, RoutedEventArgs e)
        {
            if (((Expander)sender).Tag is int id)
            {
                var note = Notes.Where(w => w.Id == id).FirstOrDefault();
                if (note != null&& !note.IsLoad)
                {
                    note.OnLoad();
                }
            }
        }

        private void ButtonRemoveTagNote_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is TagModelView tag)
            {
                var note = Notes.Where(w => w.Id == tag.NoteId).FirstOrDefault();
                if (note != null)
                {
                    note.TagIds.Remove(tag.Id);
                }
            }
        }

        private void ButtonAttachTagSelect_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button && button.Parent is StackPanel stackPanel && button.Tag is int id)
            {
                for (int i = 0; i < stackPanel.Children.Count; i++)
                {
                    if (stackPanel.Children[i] is ComboBox comboBox)
                    {
                        if (comboBox.SelectedItem is Tag tag)
                        {
                            var note = Notes.FirstOrDefault(s => s.Id == id);
                            if (note != null)
                            {
                                note.TagIds.Add(tag.Id);
                                return;
                            }
                        }
                    }
                }
            }
            
        }

        private void ButtonAddReminder_Click(object sender, RoutedEventArgs e)
        {
            if (DatePickerReminder.SelectedDate != null && ComboBoxNoteReminder.SelectedItem != null && ComboBoxNoteReminder.SelectedItem is Note note)
            {
                Reminders.Add(new Reminder() { NoteId = note.Id, ReminderDateTime = DatePickerReminder.SelectedDate.Value });
            }
        }

        private void ButtonDelereReminder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int id)
            {
                var reminder = Reminders.FirstOrDefault(f => f.Id == id);
                if (reminder != null)
                {
                    Reminders.Remove(reminder);
                }
            }
        }

        //private void ComboBoxSelectReminder_Selected(object sender, SelectionChangedEventArgs e)
        //{
        //    if (sender is ComboBox comboBox && comboBox.SelectedItem is Note note && comboBox.Tag is int id)
        //    {
        //        var reminder = Reminders.FirstOrDefault(f => f.Id == id);
        //        if (reminder != null)
        //        {
        //            reminder.NoteId = note.Id;
        //        }
        //    }
        //}
    }
}
