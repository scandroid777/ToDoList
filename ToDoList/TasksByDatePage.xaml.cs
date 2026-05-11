using ToDoList.Models;

namespace ToDoList;

public partial class TasksByDatePage : ContentPage
{
    public TasksByDatePage(DateTime selectedDate)
    {
        InitializeComponent();

        dateLabel.Text = selectedDate.ToString("📅 dd MMMM yyyy");

        var tasksForDate = TaskService.Tasks
            .Where(t => t.Date.Date == selectedDate.Date)
            .ToList();

        tasksCollection.ItemsSource = tasksForDate;
    }
}