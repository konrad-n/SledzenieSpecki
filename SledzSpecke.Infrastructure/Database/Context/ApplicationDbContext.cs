﻿using SledzSpecke.Core.Models.Domain;
using SledzSpecke.Infrastructure.Database.Migrations;
using SQLite;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Context
{
    public class ApplicationDbContext : IApplicationDbContext
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly IMigrationRunner _migrationRunner;
        private bool _isInitialized;

        public ApplicationDbContext(string databasePath, IMigrationRunner migrationRunner)
        {
            _database = new SQLiteAsyncConnection(databasePath);
            _migrationRunner = migrationRunner;
        }

        public async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                // Tworzenie tabel
                await _database.CreateTableAsync<User>();
                await _database.CreateTableAsync<Specialization>();
                await _database.CreateTableAsync<ProcedureDefinition>();
                await _database.CreateTableAsync<ProcedureExecution>();
                await _database.CreateTableAsync<Duty>();
                await _database.CreateTableAsync<Internship>();
                await _database.CreateTableAsync<Course>();
                await _migrationRunner.RunMigrationsAsync();
                _isInitialized = true;
            }
        }

        public SQLiteAsyncConnection GetConnection()
        {
            return _database;
        }

        public async Task EnsureDatabaseCreatedAsync()
        {
            if (!_isInitialized)
            {
                await InitializeTablesAsync();
                await UpdateDatabaseSchemaAsync();
                _isInitialized = true;
            }
        }

        private async Task InitializeTablesAsync()
        {
            // Tworzenie tabel w odpowiedniej kolejności
            var tableTypes = new[]
            {
                typeof(User),
                typeof(Specialization),
                typeof(ProcedureDefinition),
                // pozostałe typy
            };

            foreach (var type in tableTypes)
            {
                await _database.CreateTableAsync(type);
            }
        }

        private async Task UpdateDatabaseSchemaAsync()
        {
            // Implementacja aktualizacji schematu bazy danych
            await _migrationRunner.RunMigrationsAsync();
        }
    }
}
