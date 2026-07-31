using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Principle.Editor.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Principle.Editor.ViewModels;

public partial class ProjectHubViewModel : ViewModelBase
{
    public override string ViewModelTitle => base.ViewModelTitle + " - Project Hub";

    public ObservableCollection<ProjectCacheModel> RecentProjects { get; set; }
    public string CreateButtonText => IsCreatingProject ? "Create New Project" : "Open Project";

    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private string _projectDescription = string.Empty;

    [ObservableProperty]
    private bool _isProjectInputEditable = true;

    [ObservableProperty]
    private string _creationFormError = string.Empty;

    [ObservableProperty]
    private bool _hasCreationFormError = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CreateButtonText))]
    private bool _isCreatingProject = true;

    [ObservableProperty]
    private ProjectCacheModel? _selectedProject = null;

    public ProjectHubViewModel()
    {
        RecentProjects = new ObservableCollection<ProjectCacheModel>
        {
            new ProjectCacheModel { Name = "MooMayhem", Description = "Test", Path = @"E:\Projects\MooMayhem\MooMayhem.proj", Major = 0, Minor = 1, Patch = 5 },
            new ProjectCacheModel { Name = "Roblox", Description = "Test", Path = @"E:\Projects\Roblox\Roblox.proj", Major = 0, Minor = 1, Patch = 2 },
            new ProjectCacheModel { Name = "Minecraft", Description = "Test", Path = @"E:\Projects\Minecraft\Minecraft.proj", Major = 5, Minor = 1, Patch = 5 },
            new ProjectCacheModel { Name = "Factorio", Description = "Test", Path = @"E:\Projects\Factorio\Factorio.proj", Major = 0, Minor = 2, Patch = 8 },
            new ProjectCacheModel { Name = "Fortnite", Description = "Test", Path = @"E:\Projects\Fortnite\Fortnite.proj", Major = 0, Minor = 8, Patch = 4 },
            new ProjectCacheModel { Name = "Polytoria", Description = "Test", Path = @"E:\Projects\Polytoria\Polytoria.proj", Major = 0, Minor = 5, Patch = 5 },
            new ProjectCacheModel { Name = "Hexaball", Description = "Test", Path = @"E:\Projects\Hexaball\Hexaball.proj", Major = 0, Minor = 1, Patch = 5 },
            new ProjectCacheModel { Name = "Hytale", Description = "Test", Path = @"E:\Projects\Hytale\Hytale.proj", Major = 0, Minor = 1, Patch = 5 },
            new ProjectCacheModel { Name = "Rust", Description = "Test", Path = @"E:\Projects\Rust\Rust.proj", Major = 4, Minor = 1, Patch = 5 },
            new ProjectCacheModel { Name = "Garry's Mod", Description = "Test", Path = @"E:\Projects\GarrysMod\GarrysMod.proj", Major = 3, Minor = 1, Patch = 5 },
            new ProjectCacheModel { Name = "Team Fortress 2", Description = "Test", Path = @"E:\Projects\TF2\TF2.proj", Major = 2, Minor = 1, Patch = 5 },
        };
    }

    partial void OnSelectedProjectChanged(ProjectCacheModel? value)
    {
        if (value == null)
        {
            return;
        }

        IsCreatingProject = false;
        IsProjectInputEditable = false;
        ProjectName = value.Name;
        ProjectDescription = value.Description;
    }

    [RelayCommand]
    private void CreateOrOpen()
    {
        CreationFormError = string.Empty;
        HasCreationFormError = false;

        if (IsCreatingProject == true)
        {
            if (string.IsNullOrWhiteSpace(ProjectName) || ProjectName.Length < 4 || ProjectName.Length > 24)
            {
                CreationFormError += "Your project needs a name between 4-24 characters\n";
            }
            
            if (ProjectDescription.Length > 400)
            {
                CreationFormError += "Your description needs to be under 400 characters\n";
            }

            if (string.IsNullOrWhiteSpace(CreationFormError) == false)
            {
                HasCreationFormError = true;
                return;
            }

            ProjectCacheModel cacheModel = new ProjectCacheModel { Name = ProjectName, Description = ProjectDescription, Path = @"E:\", Major = 1, Minor = 2, Patch = 3 };
            IsProjectInputEditable = false;
            RecentProjects.Insert(0, cacheModel);
            SelectedProject = cacheModel;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(ProjectName) || ProjectName.Length < 4 || ProjectName.Length > 24)
            {
                CreationFormError += "The project has an invalid name\n";
                CreationFormError += "The project needs a name between 4-24 characters\n";
            }

            if (string.IsNullOrWhiteSpace(CreationFormError) == false)
            {
                HasCreationFormError = true;
                return;
            }
        }
    }

    [RelayCommand]
    private void SwitchToCreateForm()
    {
        IsCreatingProject = true;
        IsProjectInputEditable = true;
        ProjectName = string.Empty;
        ProjectDescription = string.Empty;
    }
}