using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HiveMinds.Core;

public class ViewModelBase<T> : INotifyPropertyChanged where T : new()
{
    protected T Model { get; set; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected ViewModelBase(T model = default!)
    {
        model ??= CreateModel();

        Model = model;
    }

    private static T CreateModel()
    {
        return new T();
    }
}