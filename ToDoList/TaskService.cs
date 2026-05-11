using System.Collections.ObjectModel;
using ToDoList.Models;

namespace ToDoList;

public static class TaskService
{
    private static ObservableCollection<TodoItem> tasks;
    public static DateTime SelectedDate { get; set; } = DateTime.Today;

    public static ObservableCollection<TodoItem> Tasks
    {
        get
        {
            if (tasks == null)
            {
                InitializeTasks();
            }
            return tasks;
        }
    }

    private static void InitializeTasks()
    {
        try
        {
            var loadedTasks = StorageService.Load();
            tasks = new ObservableCollection<TodoItem>(loadedTasks ?? new List<TodoItem>());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"TaskService error: {ex.Message}");
            tasks = new ObservableCollection<TodoItem>();
        }
    }

    public static void ClearAllTasks()
    {
        try
        {
            tasks?.Clear();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ClearAllTasks error: {ex.Message}");
        }
    }
}