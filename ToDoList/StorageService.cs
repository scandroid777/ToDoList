using System.Text.Json;
using ToDoList.Models;
using System.Collections.ObjectModel;

namespace ToDoList;

public static class StorageService
{
    private static string FilePath =>
        Path.Combine(FileSystem.AppDataDirectory, "tasks.json");

    public static void Save(ObservableCollection<TodoItem> tasks)
    {
        try
        {
            // Убеждаемся, что директория существует
            var directory = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(tasks);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
        }
    }

    public static List<TodoItem> Load()
    {
        try
        {
            if (!File.Exists(FilePath))
                return new List<TodoItem>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load error: {ex.Message}");
            return new List<TodoItem>();
        }
    }
}