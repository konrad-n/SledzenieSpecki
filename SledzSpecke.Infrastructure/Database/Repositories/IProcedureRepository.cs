﻿using SledzSpecke.Core.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SledzSpecke.Infrastructure.Database.Repositories
{
    public interface IProcedureRepository : IBaseRepository<ProcedureExecution>
    {
        Task<List<ProcedureExecution>> GetUserProceduresAsync(int userId);
        Task<Dictionary<string, int>> GetProcedureStatsAsync(int userId);
        Task<ProcedureExecution> GetProcedureWithDetailsAsync(int id);
        Task<List<ProcedureRequirement>> GetRequirementsForSpecializationAsync(int specializationId);
        Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByCategoryAsync(int userId, int specializationId);
        Task<Dictionary<string, (int Required, int Completed, int Assisted)>> GetProcedureProgressByStageAsync(int userId, int specializationId);
    }
}
