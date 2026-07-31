using CommunityToolkit.Mvvm.ComponentModel;

namespace Principle.Editor.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public virtual string ViewModelTitle => App.EditorName;
}
