namespace ToDoList;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        // текущее состояние темы
        themeSwitch.IsToggled = Application.Current.UserAppTheme == AppTheme.Dark;
    }

    private void OnThemeChanged(object sender, ToggledEventArgs e)
    {
        Application.Current.UserAppTheme =
            e.Value ? AppTheme.Dark : AppTheme.Light;
    }
}