using ToDoList.Models;

namespace ToDoList;

public partial class EditTaskPage : ContentPage
{
    public TodoItem TaskItem { get; set; }

    public EditTaskPage(TodoItem task)
    {
        InitializeComponent();

        TaskItem = task;

        titleEditor.Text = task.Title;

        categoryPicker.SelectedItem = task.Category ?? "Без категории";
        datePicker.Date = task.Date;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        TaskItem.Title = titleEditor.Text;

        if (categoryPicker.SelectedItem != null)
            TaskItem.Category = categoryPicker.SelectedItem.ToString();

        TaskItem.Date = datePicker.Date ?? DateTime.Today;

        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}