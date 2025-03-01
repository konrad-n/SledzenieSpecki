﻿using Microsoft.Extensions.Logging;
using SledzSpecke.App.Features.Authentication.ViewModels;
using SledzSpecke.App.Services;
using SledzSpecke.App.Services.Implementations;
using SledzSpecke.App.Views;
using SledzSpecke.App.Views.Auth;
using SledzSpecke.Infrastructure.Database;
using SledzSpecke.Infrastructure.Services;

namespace SledzSpecke.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FluentUI.ttf", "FluentUI");
                });

            // Register services
            RegisterServices(builder.Services);

            // Register pages
            RegisterPages(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Register singleton services
            services.AddSingleton<IFileSystemService, MauiFileSystemService>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IDataManager, DataManager>();
            services.AddSingleton<ISpecializationService, SpecializationService>();
            services.AddSingleton<IDutyShiftService, DutyShiftService>();
            services.AddSingleton<ISelfEducationService, SelfEducationService>();
            services.AddSingleton<IExportService, ExportService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IAppSettings, AppSettings>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ISpecializationDateCalculator, SpecializationDateCalculator>();

            // Register ViewModels
            services.AddTransient<LoginViewModel>();
        }

        private static void RegisterPages(IServiceCollection services)
        {
            // Register auth pages
            services.AddTransient<LoginPage>();
            services.AddTransient<RegistrationPage>();

            // Register main pages
            services.AddTransient<DashboardPage>();
            services.AddTransient<SettingsPage>();

            // Register feature pages
            services.AddTransient<CoursesPage>();
            services.AddTransient<CourseDetailsPage>();
            services.AddTransient<InternshipsPage>();
            services.AddTransient<InternshipDetailsPage>();
            services.AddTransient<ProceduresPage>();
            services.AddTransient<ProcedureDetailsPage>();
            services.AddTransient<ProcedureEntryPage>();
            services.AddTransient<DutyShiftsPage>();
            services.AddTransient<DutyShiftDetailsPage>();
            services.AddTransient<SelfEducationPage>();
            services.AddTransient<SelfEducationDetailsPage>();
            services.AddTransient<SMKExportPage>();
            services.AddTransient<AbsenceManagementPage>();
            services.AddTransient<AbsenceDetailsPage>();
        }
    }
}