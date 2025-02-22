﻿using SledzSpecke.App.Views.Dashboard;
using SledzSpecke.App.Views.Duties;
using SledzSpecke.App.Views.Procedures;
using SledzSpecke.App.Views.Profile;

namespace SledzSpecke.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Rejestracja tras dla procedur
            Routing.RegisterRoute("procedure/add", typeof(ProcedureAddPage));
            Routing.RegisterRoute("procedure/edit", typeof(ProcedureEditPage));

            // Rejestracja tras dla dyżurów
            Routing.RegisterRoute("duty/add", typeof(DutyAddPage));
            Routing.RegisterRoute("duty/edit", typeof(DutyEditPage));

            // Istniejące trasy
            Routing.RegisterRoute("dashboard", typeof(DashboardPage));
            Routing.RegisterRoute("procedures", typeof(ProceduresPage));
            Routing.RegisterRoute("duties", typeof(DutiesPage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("profile", typeof(ProfilePage));
        }
    }
}