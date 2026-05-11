using System.Collections.ObjectModel;
using ToDoList.Models;

namespace ToDoList;

public partial class MainPage : ContentPage
{
    public ObservableCollection<TodoItem> Tasks { get; set; }
    private ObservableCollection<TodoItem> filteredTasks = new();
    private string currentFilter = "Все";
    private string currentSort = "По названию";
    private bool isUpdating = false;

    public MainPage()
    {
        InitializeComponent();

        try
        {
            tasksCollection.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing = 6
            };

            Tasks = TaskService.Tasks;
            tasksCollection.ItemsSource = filteredTasks;

            Tasks.CollectionChanged += (s, e) =>
            {
                if (!isUpdating)
                {
                    isUpdating = true;
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ApplyFilterAndSort();
                        StorageService.Save(Tasks);
                        isUpdating = false;
                    });
                }
            };

            foreach (var task in Tasks.Where(t => t != null))
            {
                task.Changed += SaveTasks;
            }

            ApplyFilterAndSort();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in MainPage constructor: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void ApplyFilterAndSort()
    {
        try
        {
            if (filteredTasks == null) return;

            filteredTasks.Clear();

            var filtered = Tasks.Where(t => t != null).ToList();

            // Применение фильтра
            if (currentFilter != "Все")
            {
                filtered = filtered.Where(t => t.Category == currentFilter).ToList();
            }

            // Применение сортировки
            switch (currentSort)
            {
                case "По дате":
                    filtered = filtered.OrderBy(t => t.Date).ToList();
                    break;
                case "Избранное":
                    filtered = filtered.OrderByDescending(t => t.IsFavorite).ThenBy(t => t.Title).ToList();
                    break;
                case "По названию":
                default:
                    filtered = filtered.OrderBy(t => t.Title).ToList();
                    break;
            }

            // Добавление отфильтрованных задач
            foreach (var task in filtered)
            {
                if (task != null)
                    filteredTasks.Add(task);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ApplyFilterAndSort: {ex.Message}");
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            currentFilter = "Все";
            var text = e.NewTextValue?.ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(text))
            {
                ApplyFilterAndSort();
                return;
            }

            filteredTasks.Clear();

            var filtered = Tasks
                .Where(t => t != null && t.Title.ToLower().Contains(text))
                .ToList();

            switch (currentSort)
            {
                case "По дате":
                    filtered = filtered.OrderBy(t => t.Date).ToList();
                    break;
                case "Избранное":
                    filtered = filtered.OrderByDescending(t => t.IsFavorite).ToList();
                    break;
                default:
                    filtered = filtered.OrderBy(t => t.Title).ToList();
                    break;
            }

            foreach (var task in filtered)
            {
                if (task != null)
                    filteredTasks.Add(task);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in search: {ex.Message}");
        }
    }

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushModalAsync(new AddTaskPage());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnAddTaskClicked: {ex.Message}");
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
                Tasks.Remove(task);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnDeleteTaskClicked: {ex.Message}");
        }
    }

    private void OnClearAllClicked(object sender, EventArgs e)
    {
        try
        {
            if (Tasks.Count == 0) return;
            Tasks.Clear();
            OnCloseMenu(null, null);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnClearAllClicked: {ex.Message}");
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
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnFavoriteClicked: {ex.Message}");
        }
    }

    private void OnFilterClicked(object sender, EventArgs e)
    {
        try
        {
            var btn = sender as Button;
            if (btn?.CommandParameter is string filter)
            {
                currentFilter = filter;
                if (searchBar != null)
                    searchBar.Text = "";
                ApplyFilterAndSort();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnFilterClicked: {ex.Message}");
        }
    }

    private void OnSortChanged(object sender, EventArgs e)
    {
        try
        {
            if (sortPicker != null && sortPicker.SelectedItem is string sort)
            {
                currentSort = sort;
                ApplyFilterAndSort();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnSortChanged: {ex.Message}");
        }
    }

    private void SaveTasks()
    {
        try
        {
            StorageService.Save(Tasks);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving tasks: {ex.Message}");
        }
    }

    private void OnBurgerClicked(object sender, EventArgs e)
    {
        try
        {
            if (menuOverlay != null)
                menuOverlay.IsVisible = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnBurgerClicked: {ex.Message}");
        }
    }

    private void OnCloseMenu(object sender, EventArgs e)
    {
        try
        {
            if (menuOverlay != null)
                menuOverlay.IsVisible = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnCloseMenu: {ex.Message}");
        }
    }

    private void OnThemeChanged(object sender, ToggledEventArgs e)
    {
        try
        {
            bool isDark = e.Value;
            Application.Current.UserAppTheme = isDark ? AppTheme.Dark : AppTheme.Light;
            Preferences.Set("isDarkTheme", isDark);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnThemeChanged: {ex.Message}");
        }
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        try
        {
            await DisplayAlert("О приложении",
                "ToDo List\nВерсия 1.0\nПриложение для управления задачами",
                "Ок");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnAboutClicked: {ex.Message}");
        }
    }
}