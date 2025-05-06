namespace MinimalHabitsBlazor.Models;

public class Habit
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public List<HabitEntry> Entries { get; set; } = new();
}

public class HabitDto
{
    public string Name { get; set; } = string.Empty;
}

public class HabitEntry
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool Completed { get; set; }
    public int HabitId { get; set; }
    public Habit Habit { get; set; } = null!;
}

public class HabitEntryDto
{
    public DateTime Date { get; set; }
    public bool Completed { get; set; }
}

// Note: User model is not needed in the client as it's not used in any DTOs 