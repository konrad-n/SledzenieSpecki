﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SledzSpecke.App.ViewModels.Base;
using SledzSpecke.Core.Interfaces.Services;
using SledzSpecke.Core.Models.Domain;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SledzSpecke.App.ViewModels.Dashboard
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly IProcedureService _procedureService;
        private readonly ICourseService _courseService;
        private readonly IInternshipService _internshipService;
        private readonly IDutyService _dutyService;
        private readonly IUserService _userService;

        public DashboardViewModel(
            IProcedureService procedureService,
            ICourseService courseService,
            IInternshipService internshipService,
            IDutyService dutyService,
            IUserService userService)
        {
            _procedureService = procedureService;
            _courseService = courseService;
            _internshipService = internshipService;
            _dutyService = dutyService;
            _userService = userService;
            
            Title = "Dashboard";
            RecommendedCourses = new ObservableCollection<CourseDefinition>();
            RecommendedInternships = new ObservableCollection<InternshipDefinition>();
        }

        [ObservableProperty]
        private double proceduresProgress;
        
        [ObservableProperty]
        private string proceduresProgressText;
        
        [ObservableProperty]
        private double coursesProgress;
        
        [ObservableProperty]
        private string coursesProgressText;
        
        [ObservableProperty]
        private double internshipsProgress;
        
        [ObservableProperty]
        private string internshipsProgressText;
        
        [ObservableProperty]
        private double dutiesProgress;
        
        [ObservableProperty]
        private string dutiesProgressText;
        
        [ObservableProperty]
        private double overallProgress;
        
        [ObservableProperty]
        private string overallProgressText;
        
        [ObservableProperty]
        private string timeLeftText;
        
        [ObservableProperty]
        private ObservableCollection<CourseDefinition> recommendedCourses;
        
        [ObservableProperty]
        private ObservableCollection<InternshipDefinition> recommendedInternships;

        public override async Task LoadDataAsync()
        {
            if (IsBusy) return;
            
            try
            {
                IsBusy = true;
                
                // Obliczanie postępów
                ProceduresProgress = await _procedureService.GetProcedureCompletionPercentageAsync();
                ProceduresProgressText = $"{ProceduresProgress:P0}";
                
                CoursesProgress = await _courseService.GetCourseProgressAsync();
                CoursesProgressText = $"{CoursesProgress:P0}";
                
                InternshipsProgress = await _internshipService.GetInternshipProgressAsync();
                InternshipsProgressText = $"{InternshipsProgress:P0}";
                
                var dutyStats = await _dutyService.GetDutyStatisticsAsync();
                DutiesProgress = dutyStats.TotalHours > 0 
                    ? (double)dutyStats.TotalHours / dutyStats.RemainingHours 
                    : 0;
                DutiesProgressText = $"{dutyStats.TotalHours}/{dutyStats.RemainingHours}h";
                
                // Obliczanie ogólnego postępu
                OverallProgress = (ProceduresProgress + CoursesProgress + InternshipsProgress + DutiesProgress) / 4;
                OverallProgressText = $"{OverallProgress:P0} ukończone";
                
                // Obliczanie pozostałego czasu
                var user = await _userService.GetCurrentUserAsync();
                if (user != null)
                {
                    var daysLeft = (user.ExpectedEndDate - System.DateTime.Today).Days;
                    TimeLeftText = daysLeft switch
                    {
                        < 0 => "Specjalizacja zakończona",
                        0 => "Ostatni dzień specjalizacji",
                        1 => "Pozostał 1 dzień",
                        _ => $"Pozostało {daysLeft} dni"
                    };
                }
                
                // Ładowanie rekomendacji
                var recCourses = await _courseService.GetRecommendedCoursesForCurrentYearAsync();
                RecommendedCourses.Clear();
                foreach (var course in recCourses)
                {
                    RecommendedCourses.Add(course);
                }
                
                var recInternships = await _internshipService.GetRecommendedInternshipsForCurrentYearAsync();
                RecommendedInternships.Clear();
                foreach (var internship in recInternships)
                {
                    RecommendedInternships.Add(internship);
                }
                
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Błąd", $"Nie udało się załadować danych: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToProceduresAsync()
        {
            await Shell.Current.GoToAsync("//procedures");
        }

        [RelayCommand]
        private async Task NavigateToCoursesAsync()
        {
            await Shell.Current.GoToAsync("//courses");
        }

        [RelayCommand]
        private async Task NavigateToInternshipsAsync()
        {
            await Shell.Current.GoToAsync("//internships");
        }

        [RelayCommand]
        private async Task NavigateToDutiesAsync()
        {
            await Shell.Current.GoToAsync("//duties");
        }
    }
}
