namespace ToDoList;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Применяем системную тему при инициализации
        ApplySystemTheme();
    }

    public static void ApplySystemTheme()
    {
        try
        {
            if (Application.Current == null) return;

            // Определяем системную тему
            AppTheme systemTheme = Application.Current.PlatformAppTheme;
            if (systemTheme == AppTheme.Unspecified)
            {
                systemTheme = AppTheme.Light;
            }

            Application.Current.UserAppTheme = systemTheme;

            // Сохраняем в Preferences
            bool isDark = systemTheme == AppTheme.Dark;
            Preferences.Set("isDarkTheme", isDark);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ApplySystemTheme: {ex.Message}");
        }
    }

    public static void ApplyTheme()
    {
        if (Current?.Resources == null)
            return;

        var isDark = Preferences.Get("isDarkTheme", false);

        if (isDark)
        {
            Current.Resources["BackgroundColor"] = Color.FromArgb("#121212");
            Current.Resources["CardColor"] = Color.FromArgb("#1E1E1E");
            Current.Resources["TextColor"] = Colors.White;
        }
        else
        {
            Current.Resources["BackgroundColor"] = Color.FromArgb("#F4F6F8");
            Current.Resources["CardColor"] = Colors.White;
            Current.Resources["TextColor"] = Colors.Black;
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}