using System.Collections.ObjectModel;
using ToDoList.Models;

namespace ToDoList;

/// <summary>
/// Страница отображения задач на конкретную дату
/// </summary>
public partial class TasksByDatePage : ContentPage
{
    /// <summary>Коллекция задач на выбранную дату</summary>
    private ObservableCollection<TodoItem> tasksForDate = new();

    /// <summary>
    /// Конструктор страницы с передачей выбранной даты
    /// </summary>
    public TasksByDatePage(DateTime selectedDate)
    {
        InitializeComponent();

        try
        {
            // Форматирование и отображение даты
            dateLabel.Text = selectedDate.ToString("dd MMMM yyyy");

            // Получение задач на выбранную дату
            var tasks = TaskService.Tasks
                .Where(t => t != null && t.Date.Date == selectedDate.Date)
                .ToList();

            // Добавление в коллекцию
            foreach (var task in tasks)
            {
                tasksForDate.Add(task);
            }

            tasksCollection.ItemsSource = tasksForDate;
            UpdateEmptyState();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в конструкторе TasksByDatePage: {ex.Message}");
        }
    }

    /// <summary>
    /// Обновляет видимость пустого состояния
    /// </summary>
    private void UpdateEmptyState()
    {
        try
        {
            emptyDateTasksStack.IsVisible = tasksForDate.Count == 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в UpdateEmptyState: {ex.Message}");
        }
    }
}