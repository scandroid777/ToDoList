using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToDoList.Models;

public class TodoItem : INotifyPropertyChanged
{
    private string title;
    private bool isCompleted;
    private bool isFavorite;
    private string category = "Без категории";
    private DateTime date = DateTime.Today;

    public string Title
    {
        get => title;
        set
        {
            if (title != value)
            {
                title = value;
                OnPropertyChanged();
                Changed?.Invoke();
            }
        }
    }

    public bool IsCompleted
    {
        get => isCompleted;
        set
        {
            if (isCompleted != value)
            {
                isCompleted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TextDecoration));
                Changed?.Invoke();
            }
        }
    }

    public TextDecorations TextDecoration =>
        IsCompleted ? TextDecorations.Strikethrough : TextDecorations.None;

    public bool IsFavorite
    {
        get => isFavorite;
        set
        {
            if (isFavorite != value)
            {
                isFavorite = value;
                OnPropertyChanged();
                Changed?.Invoke();
            }
        }
    }

    public string Category
    {
        get => category;
        set
        {
            if (category != value)
            {
                category = value;
                OnPropertyChanged();
                Changed?.Invoke();
            }
        }
    }

    public DateTime Date
    {
        get => date;
        set
        {
            if (date != value)
            {
                date = value;
                OnPropertyChanged();
                Changed?.Invoke();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public event Action Changed;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}