using ToDoList.Models;

namespace ToDoList;

/// <summary>
/// Страница редактирования выбранной задачи
/// </summary>
public partial class EditTaskPage : ContentPage
{
    /// <summary>Текущая редактируемая задача</summary>
    public TodoItem TaskItem { get; set; }

    /// <summary>
    /// Конструктор страницы редактирования с передачей задачи
    /// </summary>
    public EditTaskPage(TodoItem task)
    {
        InitializeComponent();

        try
        {
            TaskItem = task;

            if (TaskItem != null)
            {
                titleEditor.Text = task.Title;
                categoryPicker.SelectedItem = task.Category ?? "Без категории";
                datePicker.Date = task.Date;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в конструкторе EditTaskPage: {ex.Message}");
        }
    }

    /// <summary>
    /// Сохраняет изменения в задачу
    /// </summary>
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            if (TaskItem == null) return;

            // Проверка заполненности полей
            if (string.IsNullOrWhiteSpace(titleEditor.Text))
            {
                await DisplayAlert("Ошибка", "Пожалуйста, введите описание задачи", "Ок");
                return;
            }

            // Обновление задачи
            TaskItem.Title = titleEditor.Text;

            if (categoryPicker.SelectedItem != null)
                TaskItem.Category = categoryPicker.SelectedItem.ToString();

            TaskItem.Date = datePicker.Date ?? DateTime.Today;

            // Сохранение в хранилище
            StorageService.Save(TaskService.Tasks);

            await DisplayAlert("Успешно", "Задача обновлена", "Ок");
            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в OnSaveClicked: {ex.Message}");
            await DisplayAlert("Ошибка", "Не удалось сохранить задачу", "Ок");
        }
    }

    /// <summary>
    /// Закрывает страницу без сохранения
    /// </summary>
    private async void OnCancelClicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в OnCancelClicked: {ex.Message}");
        }
    }
}