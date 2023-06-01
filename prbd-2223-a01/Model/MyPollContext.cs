using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRBD_Framework;

namespace MyPoll.Model;

public class MyPollContext : DbContextBase {
    public DbSet<User> Users => Set<User>();
    public DbSet<Poll> Polls => Set<Poll>();
    public DbSet<Choice> Choices => Set<Choice>();
    public DbSet<Participation> Participations => Set<Participation>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Vote> Votes => Set<Vote>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=prbd-2223-a01.db")
            //.UseSqlite("Data Source=prbd-2223-a01.db")
            //.LogTo(Console.WriteLine, LogLevel.Information)
            //.EnableSensitiveDataLogging()
            .UseLazyLoadingProxies(true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Participation>()
            .HasKey(part => new { part.UserId, part.PollId });

        modelBuilder.Entity<Vote>()
            .HasKey(vote => new { vote.UserId, vote.ChoiceId });

        modelBuilder.Entity<Vote>()
            .HasOne(v => v.Choice)
            .WithMany(c => c.Votes)
            ;

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .OnDelete(DeleteBehavior.ClientCascade)
            ;

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Poll)
            .WithMany(p => p.Comments)
            .OnDelete(DeleteBehavior.ClientCascade)
            ;

        modelBuilder.Entity<Poll>()
            .HasMany(p => p.Participants)
            .WithMany(p => p.Polls)
            .UsingEntity<Participation>(
                right => right.HasOne(p => p.User).WithMany().HasForeignKey(nameof(Participation.UserId)).OnDelete(DeleteBehavior.ClientCascade),
                left => left.HasOne(p => p.Poll).WithMany().HasForeignKey(nameof(Participation.PollId)).OnDelete(DeleteBehavior.ClientCascade),
                joinEntity => joinEntity.HasKey(p => new { p.UserId, p.PollId })
            );

        modelBuilder.Entity<Poll>()
            .HasOne(p => p.Creator)
            .WithMany()
            .OnDelete(DeleteBehavior.ClientCascade)
            ;

        modelBuilder.Entity<Poll>()
            .HasMany(p => p.Choices)
            .WithOne(c => c.Poll)
            ;

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>()
            .HasData(
                new User { Id = 1, FullName = "Harry Covère", Email = "harry@test.com", Password = SecretHasher.Hash("harry") },
                new User { Id = 2, FullName = "Mélusine Enfayite", Email = "melusine@test.com", Password = SecretHasher.Hash("melusine") },
                new User { Id = 3, FullName = "John Deuf", Email = "john@test.com", Password = SecretHasher.Hash("john") },
                new User { Id = 4, FullName = "Alain Terrieur", Email = "alain@test.com", Password = SecretHasher.Hash("alain") },
                new User { Id = 5, FullName = "Camille Honnête", Email = "camille@test.com", Password = SecretHasher.Hash("camille") },
                new User { Id = 6, FullName = "Jim Nastik", Email = "jim@test.com", Password = SecretHasher.Hash("jim") },
                new User { Id = 7, FullName = "Mehdi Cament", Email = "mehdi@test.com", Password = SecretHasher.Hash("mehdi") },
                new User { Id = 8, FullName = "Ali Gator", Email = "ali@test.com", Password = SecretHasher.Hash("ali") }
            );

        modelBuilder.Entity<Administrator>()
            .HasData(
                new Administrator { Id = 9, FullName = "Admin", Email = "admin@test.com", Password = SecretHasher.Hash("admin") }
            );

        modelBuilder.Entity<Poll>()
            .HasData(
                new Poll { Id = 1, Title = "Meilleure citation ?", CreatorId = 1 },
                new Poll { Id = 2, Title = "Meilleur film de série B ?", CreatorId = 3 },
                new Poll { Id = 3, Title = "Plus belle ville du monde ?", CreatorId = 1, Type = PollType.Simple },
                new Poll { Id = 4, Title = "Meilleur animé japonais ?", CreatorId = 5 },
                new Poll { Id = 5, Title = "Sport pratiqué", CreatorId = 3, Status = PollStatus.Closed },
                new Poll { Id = 6, Title = "Langage informatique préféré", CreatorId = 7 }
            );

        modelBuilder.Entity<Comment>()
            .HasData(
                new Comment {
                    Id = 1, AuthorId = 1, PollId = 1, Text = "M'en fout",
                    CreationDate = DateTime.Parse("2022-12-10 16:40")
                },
                new Comment {
                    Id = 2, AuthorId = 1, PollId = 2, Text = "Bonne question!",
                    CreationDate = DateTime.Parse("2022-12-01 12:33")
                },
                new Comment {
                    Id = 3, AuthorId = 2, PollId = 1, Text = "Moi aussi",
                    CreationDate = DateTime.Parse("2022-12-11 22:07")
                },
                new Comment {
                    Id = 4, AuthorId = 3, PollId = 1, Text = "Bla bla bla",
                    CreationDate = DateTime.Parse("2022-12-27 08:15")
                },
                new Comment {
                    Id = 5, AuthorId = 1, PollId = 6, Text = "I love C#",
                    CreationDate = DateTime.Parse("2022-12-31 23:59")
                },
                new Comment {
                    Id = 6, AuthorId = 3, PollId = 6, Text = "I hate WPF",
                    CreationDate = DateTime.Parse("2023-01-01 00:01")
                },
                new Comment {
                    Id = 7, AuthorId = 2, PollId = 1,
                    Text =
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi pulvinar, dolor non commodo commodo, " +
                        "felis libero sagittis tellus, at tristique orci risus hendrerit lorem. Maecenas varius hendrerit lacinia. " +
                        "Vestibulum dapibus, libero nec accumsan pulvinar, felis velit imperdiet libero, sed venenatis massa risus " +
                        "gravida dolor. In et lobortis massa.",
                    CreationDate = DateTime.Parse("2023-01-02 08:45")
                }
            );

        modelBuilder.Entity<Choice>()
            .HasData(
                new Choice {
                    Id = 1, PollId = 1,
                    Label =
                        "La science est ce que nous comprenons suffisamment bien pour l'expliquer à un ordinateur. L'art, c'est tout ce que nous faisons d'autre. (Knuth)"
                },
                new Choice {
                    Id = 2, PollId = 1,
                    Label =
                        "La question de savoir si les machines peuvent penser... est à peu près aussi pertinente que celle de savoir si les sous-marins peuvent nager. (Dijkstra)"
                },
                new Choice {
                    Id = 3, PollId = 1,
                    Label =
                        "Nous ne savons pas où nous allons, mais du moins il nous reste bien des choses à faire. (Turing)"
                },
                new Choice {
                    Id = 4, PollId = 1, Label = "La constante d’une personne est la variable d’une autre. (Perlis)"
                },
                new Choice {
                    Id = 5, PollId = 1,
                    Label =
                        "There are only two kinds of [programming] languages: the ones people complain about and the ones nobody uses. (Stroustrup)"
                },
                new Choice { Id = 6, PollId = 2, Label = "Massacre à la tronçonneuse" },
                new Choice { Id = 7, PollId = 2, Label = "Braindead" },
                new Choice { Id = 8, PollId = 2, Label = "La Nuit des morts-vivants" },
                new Choice { Id = 9, PollId = 2, Label = "Psychose" },
                new Choice { Id = 10, PollId = 2, Label = "Evil Dead" },
                new Choice { Id = 11, PollId = 3, Label = "Charleroi" },
                new Choice { Id = 12, PollId = 3, Label = "Charleville-Mézières" },
                new Choice { Id = 13, PollId = 3, Label = "Pyongyang" },
                new Choice { Id = 14, PollId = 3, Label = "Détroit" },
                new Choice { Id = 15, PollId = 4, Label = "One piece" },
                new Choice { Id = 16, PollId = 4, Label = "Hunter x Hunter" },
                new Choice { Id = 17, PollId = 4, Label = "Fullmetal Alchemist" },
                new Choice { Id = 18, PollId = 4, Label = "Death Note" },
                new Choice { Id = 19, PollId = 4, Label = "Naruto Shippûden" },
                new Choice { Id = 20, PollId = 4, Label = "Dragon Ball Z" },
                new Choice { Id = 21, PollId = 5, Label = "Curling" },
                new Choice { Id = 22, PollId = 5, Label = "Swamp Football" },
                new Choice { Id = 23, PollId = 5, Label = "Fléchettes" },
                new Choice { Id = 24, PollId = 5, Label = "Kin-ball" },
                new Choice { Id = 25, PollId = 5, Label = "Pétanque" },
                new Choice { Id = 26, PollId = 5, Label = "Lancer de tronc" },
                new Choice { Id = 27, PollId = 6, Label = "Brainfuck" },
                new Choice { Id = 28, PollId = 6, Label = "Java" },
                new Choice { Id = 29, PollId = 6, Label = "C#" },
                new Choice { Id = 30, PollId = 6, Label = "PHP" },
                new Choice { Id = 31, PollId = 6, Label = "Whitespace" },
                new Choice { Id = 32, PollId = 6, Label = "Aargh!" }
            );

        modelBuilder.Entity<Vote>()
            .HasData(
                new Vote { UserId = 1, ChoiceId = 1, Value = VoteValue.Yes },
                new Vote { UserId = 1, ChoiceId = 2, Value = VoteValue.Maybe },
                new Vote { UserId = 1, ChoiceId = 5, Value = VoteValue.No },
                new Vote { UserId = 1, ChoiceId = 11, Value = VoteValue.Yes },
                new Vote { UserId = 1, ChoiceId = 16, Value = VoteValue.Yes },
                new Vote { UserId = 1, ChoiceId = 17, Value = VoteValue.No },
                new Vote { UserId = 1, ChoiceId = 27, Value = VoteValue.Yes },
                new Vote { UserId = 2, ChoiceId = 3, Value = VoteValue.Yes },
                new Vote { UserId = 2, ChoiceId = 9, Value = VoteValue.Maybe },
                new Vote { UserId = 2, ChoiceId = 10, Value = VoteValue.Yes },
                new Vote { UserId = 2, ChoiceId = 16, Value = VoteValue.Yes },
                new Vote { UserId = 2, ChoiceId = 29, Value = VoteValue.Yes },
                new Vote { UserId = 3, ChoiceId = 2, Value = VoteValue.Yes },
                new Vote { UserId = 3, ChoiceId = 4, Value = VoteValue.Maybe },
                new Vote { UserId = 3, ChoiceId = 16, Value = VoteValue.Maybe },
                new Vote { UserId = 3, ChoiceId = 20, Value = VoteValue.Yes },
                new Vote { UserId = 3, ChoiceId = 32, Value = VoteValue.No },
                new Vote { UserId = 4, ChoiceId = 29, Value = VoteValue.Yes },
                new Vote { UserId = 5, ChoiceId = 27, Value = VoteValue.Yes },
                new Vote { UserId = 5, ChoiceId = 28, Value = VoteValue.No },
                new Vote { UserId = 6, ChoiceId = 27, Value = VoteValue.Maybe },
                new Vote { UserId = 6, ChoiceId = 28, Value = VoteValue.Yes },
                new Vote { UserId = 6, ChoiceId = 29, Value = VoteValue.Maybe },
                new Vote { UserId = 7, ChoiceId = 27, Value = VoteValue.Maybe },
                new Vote { UserId = 7, ChoiceId = 29, Value = VoteValue.Yes },
                new Vote { UserId = 7, ChoiceId = 30, Value = VoteValue.Maybe },
                new Vote { UserId = 8, ChoiceId = 27, Value = VoteValue.Maybe },
                new Vote { UserId = 8, ChoiceId = 30, Value = VoteValue.Yes },
                new Vote { UserId = 8, ChoiceId = 32, Value = VoteValue.No }
            );

        modelBuilder.Entity<Participation>()
            .HasData(
                new Participation { PollId = 1, UserId = 1 },
                new Participation { PollId = 1, UserId = 2 },
                new Participation { PollId = 1, UserId = 3 },
                new Participation { PollId = 2, UserId = 2 },
                new Participation { PollId = 3, UserId = 1 },
                new Participation { PollId = 4, UserId = 1 },
                new Participation { PollId = 4, UserId = 2 },
                new Participation { PollId = 4, UserId = 3 },
                new Participation { PollId = 5, UserId = 1 },
                new Participation { PollId = 5, UserId = 2 },
                new Participation { PollId = 5, UserId = 3 },
                new Participation { PollId = 6, UserId = 1 },
                new Participation { PollId = 6, UserId = 2 },
                new Participation { PollId = 6, UserId = 3 },
                new Participation { PollId = 6, UserId = 4 },
                new Participation { PollId = 6, UserId = 5 },
                new Participation { PollId = 6, UserId = 6 },
                new Participation { PollId = 6, UserId = 7 },
                new Participation { PollId = 6, UserId = 8 }
            );
    }
}
