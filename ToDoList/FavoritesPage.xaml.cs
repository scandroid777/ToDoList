using System.Collections.ObjectModel;
using ToDoList.Models;

namespace ToDoList;

public partial class FavoritesPage : ContentPage
{
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshFavorites();
    }

    private void RefreshFavorites()
    {
        try
        {
            favorites.Clear();

            foreach (var task in TaskService.Tasks.Where(t => t != null && t.IsFavorite))
            {
                favorites.Add(task);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in RefreshFavorites: {ex.Message}");
        }
    }

    private async void OnDeleteTaskClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var task = button?.CommandParameter as TodoItem;

            if (task == null) return;

            bool confirm = await DisplayAlert("Удаление", $"Удалить задачу?", "Да", "Нет");

            if (confirm)
            {
                TaskService.Tasks.Remove(task);
                RefreshFavorites();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnDeleteTaskClicked: {ex.Message}");
        }
    }

    private void OnFavoriteClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var task = button?.CommandParameter as TodoItem;

            if (task != null)
            {
                task.IsFavorite = !task.IsFavorite;
                // Мгновенное обновление списка
                RefreshFavorites();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnFavoriteClicked: {ex.Message}");
        }
    }
}