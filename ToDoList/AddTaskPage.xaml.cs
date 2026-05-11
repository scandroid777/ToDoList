using ToDoList.Models;

namespace ToDoList;

public partial class AddTaskPage : ContentPage
{
    public AddTaskPage()
    {
        InitializeComponent();
        datePicker.Date = TaskService.SelectedDate;

        categoryPicker.ItemsSource = new List<string>
        {
            "Без категории",
            "Работа",
            "Личное",
            "Дни рождения"
        };

        categoryPicker.SelectedIndex = 0;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(titleEditor.Text))
        {
            await DisplayAlert("Ошибка", "Введите задачу", "Ок");
            return;
        }

        TaskService.Tasks.Add(new TodoItem 

        {
            Title = titleEditor.Text,
            Category = categoryPicker.SelectedItem?.ToString() ?? "Без категории",
            Date = datePicker.Date ?? DateTime.Today
        });
        StorageService.Save(TaskService.Tasks); 

        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}