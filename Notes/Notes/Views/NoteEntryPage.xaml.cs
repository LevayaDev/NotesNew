using System;
using System.IO;
using Notes.Models;
using Xamarin.Forms;

namespace Notes.Views
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class NoteEntryPage : ContentPage
    {
        bool alive = true;
        private DateTime StartTime;
        public string ItemId
        {
            set
            {
                LoadNote(value);
            }
        }

        public NoteEntryPage()
        {
            InitializeComponent();

            // Set the BindingContext of the page to a new Note.
            BindingContext = new Note();

            StartTime = DateTime.Now;
            Device.StartTimer(TimeSpan.FromSeconds(0), OnTimerTick);
        }

        private bool OnTimerTick()
        {
            TimerText.Text = (DateTime.Now - StartTime).ToString(@"hh':'mm':'ss'.'f");
            return alive;
        }

        async void LoadNote(string itemId)
        {
            try
            {
                int id = Convert.ToInt32(itemId);
                // Retrieve the note and set it as the BindingContext of the page.
                Note note = await App.Database.GetNoteAsync(id);
                BindingContext = note;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load note.");
            }
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (alive == true)
            {
                alive = false;
            }
            else
            {
                alive = true;
                Device.StartTimer(TimeSpan.FromSeconds(0), OnTimerTick);
            }
            var note = (Note)BindingContext;
            note.Date = DateTime.UtcNow;
            note.Result = TimerText.Text;
            if ((!string.IsNullOrWhiteSpace(note.Text)) && (!string.IsNullOrWhiteSpace(note.Result)))
            {
                await App.Database.SaveNoteAsync(note);
            }

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;
            await App.Database.DeleteNoteAsync(note);

            // Navigate backwards
            await Shell.Current.GoToAsync("..");
        }
    }
}