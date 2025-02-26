﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;

namespace SledzSpecke.App.ViewModels.Profile;

public partial class ProfileEditViewModel : BaseViewModel
{
    private readonly IUserService _userService;

    public ProfileEditViewModel(IUserService userService)
    {
        _userService = userService;
        Title = "Edytuj profil";
        // Set default dates
        StartDate = DateTime.Now.AddYears(-1);
        EndDate = DateTime.Now.AddYears(4);
    }

    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    private string pwz;

    [ObservableProperty]
    private DateTime startDate;

    [ObservableProperty]
    private DateTime endDate;

    public override async Task LoadDataAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var user = await _userService.GetCurrentUserAsync();
            if (user != null)
            {
                UserName = user.Name;
                Pwz = user.PWZ; // Note the capital P - ObservableProperty generates capitalized properties
                // Set dates if user has them
                if (user.SpecializationStartDate != default)
                    StartDate = user.SpecializationStartDate;
                if (user.ExpectedEndDate != default)
                    EndDate = user.ExpectedEndDate;
            }
        }
        catch (Exception ex)
        {
            // Log the error but don't show to user during development
            System.Diagnostics.Debug.WriteLine($"Error loading profile data: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        // For now, just return to previous page
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
