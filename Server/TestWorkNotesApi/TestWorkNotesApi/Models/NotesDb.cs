using Microsoft.EntityFrameworkCore;

namespace TestWorkNotesApi.Models
{
    public partial class NotesDb : DbContext
    {
        public NotesDb()
        {
        }

        public NotesDb(DbContextOptions<NotesDb> options)
            : base(options)
        {
        }

        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Reminder> Reminders { get; set; }
        public virtual DbSet<NoteTag> NoteTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseNpgsql($"Host={Config.config.DB.Host};" +
                    $"Port={Config.config.DB.Port};" +
                    $"Database={Config.config.DB.Database};" +
                    $"Username={Config.config.DB.Username};" +
                    $"Password={Config.config.DB.Password}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NoteTag>(entity =>
            {
                entity.HasKey(h => new { h.NoteId, h.TagId }).HasName("NoteTag_pkey");
                entity.HasOne(s => s.Tag).WithMany(s => s.NoteTags).HasForeignKey(s => s.TagId).HasConstraintName("NoteTag_TagId_fkey");
                entity.HasOne(s => s.Note).WithMany(s => s.NoteTags).HasForeignKey(s => s.NoteId).HasConstraintName("NoteTag_NoteId_fkey");
            });

            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.HasOne(s => s.Note).WithMany(s => s.Reminders).HasForeignKey(s => s.NoteId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
