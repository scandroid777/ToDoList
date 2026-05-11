using System.Collections.ObjectModel;
using ToDoList.Models;

namespace ToDoList;

/// <summary>
/// Страница избранных задач
/// Показывает только задачи отмеченные как избранные
/// </summary>
public partial class FavoritesPage : ContentPage
{
    /// <summary>Коллекция избранных задач</summary>
    private ObservableCollection<TodoItem> favorites = new();

    public FavoritesPage()
    {
        InitializeComponent();
        favoritesCollection.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
        {
            ItemSpacing = 6
        };
        favoritesCollection.ItemsSource = favorites;
    }

    /// <summary>
    /// Вызывается при отображении страницы
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshFavorites();
    }

    /// <summary>
    /// Обновляет список избранных задач
    /// </summary>
    private void RefreshFavorites()
    {
        try
        {
            favorites.Clear();

            foreach (var task in TaskService.Tasks.Where(t => t != null && t.IsFavorite))
            {
                favorites.Add(task);
            }

            UpdateEmptyState();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в RefreshFavorites: {ex.Message}");
        }
    }

    /// <summary>
    /// Обновляет видимость пустого состояния
    /// </summary>
    private void UpdateEmptyState()
    {
        try
        {
            emptyFavoritesStack.IsVisible = favorites.Count == 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в UpdateEmptyState: {ex.Message}");
        }
    }

    /// <summary>
    /// Открывает страницу редактирования задачи при клике
    /// </summary>
    private async void OnTaskTapped(object sender, TappedEventArgs e)
    {
        try
        {
            var frame = sender as Frame;
            var task = frame?.BindingContext as TodoItem;

            if (task != null)
            {
                await Navigation.PushModalAsync(new EditTaskPage(task));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в OnTaskTapped: {ex.Message}");
        }
    }

    /// <summary>
    /// Удаляет задачу из избранного и из списка
    /// </summary>
    private async void OnDeleteTaskClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var task = button?.CommandParameter as TodoItem;

            if (task == null) return;

            bool confirm = await DisplayAlert("Удаление", $"Удалить задачу '{task.Title}'?", "Да", "Нет");

            if (confirm)
            {
                TaskService.Tasks.Remove(task);
                RefreshFavorites();
                await DisplayAlert("Успешно", "Задача удалена", "Ок");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в OnDeleteTaskClicked: {ex.Message}");
        }
    }

    /// <summary>
    /// Удаляет задачу из избранного
    /// </summary>
    private void OnFavoriteClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var task = button?.CommandParameter as TodoItem;

            if (task != null)
            {
                task.IsFavorite = !task.IsFavorite;
                RefreshFavorites();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в OnFavoriteClicked: {ex.Message}");
        }
    }
}