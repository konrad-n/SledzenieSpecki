﻿using System.Text.Json;
using Microsoft.Extensions.Logging;
using SledzSpecke.Core.Models;
using SledzSpecke.Infrastructure.Database;
using SledzSpecke.Infrastructure.Database.Initialization;

namespace SledzSpecke.App.Services
{
    public class DataManager
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<DataManager> _logger;
        private static readonly string _appDataFolder = FileSystem.AppDataDirectory;
        private static readonly string _specializationFile = Path.Combine(_appDataFolder, "specialization.json");

        private Specialization _specialization;

        public DataManager(DatabaseService databaseService, ILogger<DataManager> logger)
        {
            _databaseService = databaseService;
            _logger = logger;

            if (!Directory.Exists(_appDataFolder))
            {
                Directory.CreateDirectory(_appDataFolder);
            }
        }

        public async Task<Specialization> LoadSpecializationAsync()
        {
            try
            {
                // First try to get from database
                var specialization = await _databaseService.GetCurrentSpecializationAsync();
                if (specialization != null)
                {
                    _specialization = specialization;

                    // Load related data
                    await LoadRelatedDataAsync(specialization);

                    return specialization;
                }

                // Try to load from file as fallback
                if (File.Exists(_specializationFile))
                {
                    string json = await File.ReadAllTextAsync(_specializationFile);
                    _specialization = JsonSerializer.Deserialize<Specialization>(json);

                    // Save to database for future use
                    await SaveSpecializationAsync(_specialization);
                }
                else
                {
                    // If nothing found, seed default data
                    _specialization = DataSeeder.SeedHematologySpecialization();
                    await SaveSpecializationAsync(_specialization);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading specialization data");
                _specialization = DataSeeder.SeedHematologySpecialization();
            }

            return _specialization;
        }

        private async Task LoadRelatedDataAsync(Specialization specialization)
        {
            try
            {
                // Load courses
                var courses = await _databaseService.GetAllAsync<Course>();
                specialization.RequiredCourses = courses
                    .Where(c => c.SpecializationId == specialization.Id)
                    .ToList();

                // Load internships
                var internships = await _databaseService.GetAllAsync<Internship>();
                specialization.RequiredInternships = internships
                    .Where(i => i.SpecializationId == specialization.Id)
                    .ToList();

                // Load procedures
                var procedures = await _databaseService.GetAllAsync<MedicalProcedure>();
                specialization.RequiredProcedures = procedures
                    .Where(p => p.SpecializationId == specialization.Id)
                    .ToList();

                // Load procedure entries
                foreach (var procedure in specialization.RequiredProcedures)
                {
                    var entries = await _databaseService.GetEntriesForProcedureAsync(procedure.Id);
                    procedure.Entries = entries;
                    procedure.CompletedCount = entries.Count;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading related data for specialization");
            }
        }

        public async Task SaveSpecializationAsync(Specialization specialization)
        {
            try
            {
                _specialization = specialization;

                // Save to database
                await _databaseService.SaveAsync(specialization);

                // Save related data
                foreach (var course in specialization.RequiredCourses)
                {
                    course.SpecializationId = specialization.Id;
                    await _databaseService.SaveAsync(course);
                }

                foreach (var internship in specialization.RequiredInternships)
                {
                    internship.SpecializationId = specialization.Id;
                    await _databaseService.SaveAsync(internship);
                }

                foreach (var procedure in specialization.RequiredProcedures)
                {
                    procedure.SpecializationId = specialization.Id;
                    await _databaseService.SaveAsync(procedure);

                    foreach (var entry in procedure.Entries)
                    {
                        entry.ProcedureId = procedure.Id;
                        await _databaseService.SaveAsync(entry);
                    }
                }

                // Save user settings
                var settings = await _databaseService.GetUserSettingsAsync();
                settings.CurrentSpecializationId = specialization.Id;
                await _databaseService.SaveUserSettingsAsync(settings);

                // Save to file as backup
                string json = JsonSerializer.Serialize(specialization, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                await File.WriteAllTextAsync(_specializationFile, json);

                _logger.LogInformation("Specialization saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving specialization data");
                throw;
            }
        }

        public async Task<bool> DeleteAllDataAsync()
        {
            try
            {
                // Delete from database
                await _databaseService.DeleteAllDataAsync();

                // Delete file backup
                if (File.Exists(_specializationFile))
                {
                    File.Delete(_specializationFile);
                }

                _specialization = null;
                _logger.LogInformation("All data deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all data");
                return false;
            }
        }

        public async Task<List<SpecializationType>> GetAllSpecializationTypesAsync()
        {
            try
            {
                var types = await _databaseService.GetAllAsync<SpecializationType>();

                if (types.Count == 0)
                {
                    // Seed specialization types if none exist
                    types = SeedSpecializationTypes();
                    foreach (var type in types)
                    {
                        await _databaseService.SaveAsync(type);
                    }
                }

                return types;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting specialization types");
                return SeedSpecializationTypes();
            }
        }

        private List<SpecializationType> SeedSpecializationTypes()
        {
            var specializationTypes = new List<SpecializationType>
            {
                new SpecializationType { Name = "Hematologia", BaseDurationWeeks = 261, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 157 },
                // Add more specialization types
                new SpecializationType { Name = "Alergologia", BaseDurationWeeks = 208, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 104 },
                new SpecializationType { Name = "Anestezjologia i intensywna terapia", BaseDurationWeeks = 312, BasicModuleDurationWeeks = 104, SpecialisticModuleDurationWeeks = 208 },
                // Add more specialization types as needed
            };

            return specializationTypes;
        }
    }
}