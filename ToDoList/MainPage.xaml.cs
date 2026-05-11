using System.Collections.ObjectModel;
using ToDoList.Models;

namespace ToDoList;

/// <summary>
/// Главная страница приложения с управлением задачами
/// Содержит фильтрацию, сортировку, поиск и управление избранными задачами
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>Коллекция всех задач</summary>
    public ObservableCollection<TodoItem> Tasks { get; set; }

    /// <summary>Отфильтрованные и отсортированные задачи для отображения</summary>
    private ObservableCollection<TodoItem> filteredTasks = new();

    /// <summary>Текущий активный фильтр категории</summary>
    private string currentFilter = "Все";

    /// <summary>Текущий тип сортировки</summary>
    private string currentSort = "По названию";

    /// <summary>Флаг для предотвращения циклических обновлений</summary>
    private bool isUpdating = false;

    /// <summary>
    /// Конструктор MainPage - инициализирует страницу и загружает задачи
    /// </summary>
    public MainPage()
    {
        InitializeComponent();

        try
        {
            // Оптимизация производительности CollectionView
            tasksCollection.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing = 6
            };

            // Загрузка задач из сервиса
            Tasks = TaskService.Tasks;
            tasksCollection.ItemsSource = filteredTasks;

            // Обновление при изменении коллекции
            Tasks.CollectionChanged += (s, e) =>
            {
                if (!isUpdating)
                {
                    isUpdating = true;
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ApplyFilterAndSort();
                        StorageService.Save(Tasks);
                        UpdateEmptyState();
                        isUpdating = false;
                    });
                }
            };

            // Подписка на изменение каждой задачи
            foreach (var task in Tasks.Where(t => t != null))
            {
                task.Changed += SaveTasks;
            }

            ApplyFilterAndSort();
            UpdateEmptyState();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка инициализации MainPage: {ex.Message}\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Применяет фильтр и сортировку к задачам
    /// </summary>
    private void ApplyFilterAndSort()
    {
        try
        {
            if (filteredTasks == null) return;

            filteredTasks.Clear();

            var filtered = Tasks.Where(t => t != null).ToList();

            // Применение фильтра по категории
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

            // Добавление отфильтрованных задач в коллекцию для отображения
            foreach (var task in filtered)
            {
                if (task != null)
                    filteredTasks.Add(task);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в ApplyFilterAndSort: {ex.Message}");
        }
    }

    /// <summary>
    /// Обновляет видимость пустого состояния (когда нет задач)
    /// </summary>
    private void UpdateEmptyState()
    {
        try
        {
            emptyTasksStack.IsVisible = filteredTasks.Count == 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в UpdateEmptyState: {ex.Message}");
        }
    }

    /// <summary>
    /// Обработчик изменения текста поиска
    /// </summary>
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            currentFilter = "Все";
            var text = e.NewTextValue?.ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(text))
            {
                ApplyFilterAndSort();
                UpdateEmptyState();
                return;
            }

            filteredTasks.Clear();

            // Поиск по названию задачи
            var filtered = Tasks
                .Where(t => t != null && t.Title.ToLower().Contains(text))
                .ToList();

            // Применение текущей сортировки к результатам поиска
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

            UpdateEmptyState();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в поиске: {ex.Message}");
        }
    }

    /// <summary>
    /// Открывает страницу добавления новой задачи
    /// </summary>
    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushModalAsync(new AddTaskPage());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в OnAddTaskClicked: {ex.Message}");
        }
    }

    /// <summary>
    /// Открывает страницу редактирования задачи при клике на задачу
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
    /// Удаляет задачу после подтверждения пользователя
    /// </summary>
    private async void OnDeleteTaskClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            var task = button?.CommandParameter as TodoItem;

            if (task == null) return;

            // Запрос подтверждения удаления
            bool confirm = await DisplayAlert("Удаление", $"Удалить задачу '{task.Title}'?", "Да", "Нет");

            if (confirm)
            {
                Tasks.Remove(task);
                await DisplayAlert("Успешно", "Задача удалена", "Ок");
                UpdateEmptyState();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в OnDeleteTaskClicked: {ex.Message}");
        }
    }

    /// <summary>
    /// Открывает меню настроек
    /// </summary>
    private async void OnBurgerClicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushModalAsync(new SettingsPage());
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в OnBurgerClicked: {ex.Message}");
        }
    }

    /// <summary>
    /// Добавляет/удаляет задачу из избранного
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
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в OnFavoriteClicked: {ex.Message}");
        }
    }

    /// <summary>
    /// Применяет фильтр по категории
    /// </summary>
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
                UpdateEmptyState();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в OnFilterClicked: {ex.Message}");
        }
    }

    /// <summary>
    /// Сохраняет все задачи в хранилище
    /// </summary>
    private void SaveTasks()
    {
        try
        {
            StorageService.Save(Tasks);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении задач: {ex.Message}");
        }
    }
}