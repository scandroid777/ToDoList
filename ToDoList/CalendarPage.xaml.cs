using ToDoList.Models;
using System.Collections.ObjectModel;

namespace ToDoList;

public partial class CalendarPage : ContentPage
{
    private DateTime currentDate = DateTime.Today;
    private bool isDrawing = false;
    private DateTime lastDrawnMonth = DateTime.MinValue;
    private AppTheme lastTheme = AppTheme.Unspecified;

    public CalendarPage()
    {
        InitializeComponent();
        DrawCalendar();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Перерисовываем если месяц изменился или тема изменилась
        var currentTheme = Application.Current?.UserAppTheme ?? AppTheme.Light;
        if (lastDrawnMonth.Year != currentDate.Year ||
            lastDrawnMonth.Month != currentDate.Month ||
            lastTheme != currentTheme)
        {
            lastTheme = currentTheme;
            DrawCalendar();
        }
    }

    private void DrawCalendar()
    {
        if (isDrawing) return;
        isDrawing = true;

        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                calendarGrid.Children.Clear();
                calendarGrid.RowDefinitions.Clear();

                var firstDay = new DateTime(currentDate.Year, currentDate.Month, 1);
                int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

                monthLabel.Text = firstDay.ToString("MMMM yyyy").ToUpper();

                int startDay = ((int)firstDay.DayOfWeek + 6) % 7;
                int totalCells = startDay + daysInMonth;
                int rows = (int)Math.Ceiling(totalCells / 7.0);

                for (int i = 0; i < rows; i++)
                    calendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var datesWithTasks = TaskService.Tasks
                    .Where(t => t != null)
                    .Select(t => t.Date.Date)
                    .Distinct()
                    .ToHashSet();

                int day = 1;
                DateTime today = DateTime.Today;
                bool isDark = Application.Current?.UserAppTheme == AppTheme.Dark;

                // Кэшируем цвета на основе текущей темы
                Color textColorRegular = isDark ? Colors.White : Colors.Black;
                Color textColorToday = Colors.White;
                Color bgColorRegular = isDark ? Color.FromArgb("#2A2A2A") : Colors.White;
                Color bgColorToday = Color.FromArgb("#52B788");
                Color dotColor = Color.FromArgb("#52B788");

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (i == 0 && j < startDay) continue;
                        if (day > daysInMonth) break;

                        int currentDay = day;
                        var date = new DateTime(currentDate.Year, currentDate.Month, currentDay);
                        bool hasTasks = datesWithTasks.Contains(date.Date);
                        bool isToday = date.Date == today.Date;

                        var frame = CreateDayFrame(currentDay, hasTasks, isToday, date,
                            textColorRegular, textColorToday, bgColorRegular, bgColorToday, dotColor);

                        calendarGrid.Add(frame, j, i);
                        day++;
                    }
                }

                lastDrawnMonth = currentDate;
            });
        }
        finally
        {
            isDrawing = false;
        }
    }

    private Frame CreateDayFrame(int day, bool hasTasks, bool isToday, DateTime date,
        Color textColorRegular, Color textColorToday, Color bgColorRegular, Color bgColorToday, Color dotColor)
    {
        var layout = new VerticalStackLayout
        {
            Spacing = 1,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var dayLabel = new Label
        {
            Text = day.ToString(),
            HorizontalOptions = LayoutOptions.Center,
            FontSize = 12,
            FontAttributes = isToday ? FontAttributes.Bold : FontAttributes.None,
            TextColor = isToday ? textColorToday : textColorRegular
        };

        layout.Children.Add(dayLabel);

        if (hasTasks)
        {
            layout.Children.Add(new BoxView
            {
                WidthRequest = 4,
                HeightRequest = 4,
                CornerRadius = 2,
                BackgroundColor = isToday ? Colors.White : dotColor,
                HorizontalOptions = LayoutOptions.Center
            });
        }

        var frame = new Frame
        {
            WidthRequest = 42,
            HeightRequest = 42,
            CornerRadius = 8,
            Padding = 2,
            BackgroundColor = isToday ? bgColorToday : bgColorRegular,
            HasShadow = false,
            BorderColor = Colors.Transparent,
            Content = layout
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (s, e) =>
        {
            await Navigation.PushAsync(new TasksByDatePage(date));
        };

        frame.GestureRecognizers.Add(tap);
        return frame;
    }

    private void OnPrevMonth(object sender, EventArgs e)
    {
        currentDate = currentDate.AddMonths(-1);
        DrawCalendar();
    }

    private void OnNextMonth(object sender, EventArgs e)
    {
        currentDate = currentDate.AddMonths(1);
        DrawCalendar();
    }
}