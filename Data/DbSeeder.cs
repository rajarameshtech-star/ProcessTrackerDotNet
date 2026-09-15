using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Entities;
using ProcessTracker.API.Validators;

namespace ProcessTracker.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ProcessTrackerDbContext context, IDynamicProcessValidator validator)
    {
        await context.Database.EnsureCreatedAsync();

        // 1. Projects
        if (!await context.Projects.AnyAsync())
        {
            var projects = new[]
            {
                new Project { Id = 1, Name = "Payments Modernization", Description = "Modernization of the enterprise payment processing platform" },
                new Project { Id = 2, Name = "Customer Platform", Description = "Customer relationship and engagement platform" },
                new Project { Id = 3, Name = "Data Platform", Description = "Enterprise data ingestion and analytics platform" }
            };
            
            await context.Projects.AddRangeAsync(projects);
            await context.Database.OpenConnectionAsync();
            try {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Projects ON");
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Projects OFF");
            } finally {
                await context.Database.CloseConnectionAsync();
            }
        }

        // 2. Applications
        if (!await context.Applications.AnyAsync())
        {
            var apps = new[]
            {
                new Application { Id = 1, ProjectId = 1, Name = "Payments API", Description = "" },
                new Application { Id = 2, ProjectId = 1, Name = "Payment Gateway", Description = "" },
                new Application { Id = 3, ProjectId = 2, Name = "Customer Portal", Description = "" },
                new Application { Id = 4, ProjectId = 2, Name = "Customer Notifications", Description = "" },
                new Application { Id = 5, ProjectId = 3, Name = "Data Ingestion Service", Description = "" }
            };

            await context.Applications.AddRangeAsync(apps);
            await context.Database.OpenConnectionAsync();
            try {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Applications ON");
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Applications OFF");
            } finally {
                await context.Database.CloseConnectionAsync();
            }
        }

        // 3. ProcessDefinitions
        if (!await context.ProcessDefinitions.AnyAsync())
        {
            var pds = new[]
            {
                new ProcessDefinition { Id = 1, ProcessCode = "CR", ProcessName = "Change Request", Description = "Application change request and deployment tracking", IsActive = true, CreatedDate = DateTime.UtcNow },
                new ProcessDefinition { Id = 2, ProcessCode = "UT", ProcessName = "Unit Testing", Description = "Unit test execution and quality tracking", IsActive = true, CreatedDate = DateTime.UtcNow },
                new ProcessDefinition { Id = 3, ProcessCode = "INC", ProcessName = "Incident", Description = "Production incident tracking and resolution", IsActive = true, CreatedDate = DateTime.UtcNow }
            };

            await context.ProcessDefinitions.AddRangeAsync(pds);
            await context.Database.OpenConnectionAsync();
            try {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ProcessDefinitions ON");
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ProcessDefinitions OFF");
            } finally {
                await context.Database.CloseConnectionAsync();
            }
        }

        // 4. ProcessDefinitionProjectMappings
        if (!await context.ProcessDefinitionProjectMappings.AnyAsync())
        {
            var maps = new[]
            {
                new ProcessDefinitionProjectMapping { ProjectId = 1, ProcessDefinitionId = 1 }, // PAY -> CR
                new ProcessDefinitionProjectMapping { ProjectId = 1, ProcessDefinitionId = 2 }, // PAY -> UT
                new ProcessDefinitionProjectMapping { ProjectId = 1, ProcessDefinitionId = 3 }, // PAY -> INC
                new ProcessDefinitionProjectMapping { ProjectId = 2, ProcessDefinitionId = 1 }, // CRM -> CR
                new ProcessDefinitionProjectMapping { ProjectId = 2, ProcessDefinitionId = 3 }, // CRM -> INC
                new ProcessDefinitionProjectMapping { ProjectId = 3, ProcessDefinitionId = 1 }, // DATA -> CR
                new ProcessDefinitionProjectMapping { ProjectId = 3, ProcessDefinitionId = 2 }, // DATA -> UT
                new ProcessDefinitionProjectMapping { ProjectId = 3, ProcessDefinitionId = 3 }  // DATA -> INC
            };

            await context.ProcessDefinitionProjectMappings.AddRangeAsync(maps);
            await context.SaveChangesAsync();
        }

        // 5. ProcessFields
        if (!await context.ProcessFields.AnyAsync())
        {
            var crFields = new[]
            {
                new ProcessField { ProcessDefinitionId = 1, FieldName = "ChangeType", FieldType = FieldType.Select, IsRequired = true, SortOrder = 1, OptionsJson = "[\"Standard\",\"Normal\",\"Emergency\"]", IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "RiskLevel", FieldType = FieldType.Select, IsRequired = true, SortOrder = 2, OptionsJson = "[\"Low\",\"Medium\",\"High\",\"Critical\"]", IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "PullRequestUrl", FieldType = FieldType.Url, IsRequired = true, SortOrder = 3, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "PullRequestCreated", FieldType = FieldType.Boolean, IsRequired = true, SortOrder = 4, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "PullRequestMerged", FieldType = FieldType.Boolean, IsRequired = true, SortOrder = 5, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "CodeReviewCompleted", FieldType = FieldType.Boolean, IsRequired = true, SortOrder = 6, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "UnitTestingCompleted", FieldType = FieldType.Boolean, IsRequired = true, SortOrder = 7, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "DeploymentStatus", FieldType = FieldType.Select, IsRequired = true, SortOrder = 8, OptionsJson = "[\"NotStarted\",\"InProgress\",\"Completed\",\"Failed\",\"RolledBack\"]", IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "DeploymentDate", FieldType = FieldType.Date, IsRequired = false, SortOrder = 9, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "ChangeSuccessful", FieldType = FieldType.Boolean, IsRequired = true, SortOrder = 10, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "RollbackRequired", FieldType = FieldType.Boolean, IsRequired = true, SortOrder = 11, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "PlannedStartTime", FieldType = FieldType.DateTime, IsRequired = true, SortOrder = 12, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "PlannedEndTime", FieldType = FieldType.DateTime, IsRequired = true, SortOrder = 13, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "ActualStartTime", FieldType = FieldType.DateTime, IsRequired = false, SortOrder = 14, IsActive = true },
                new ProcessField { ProcessDefinitionId = 1, FieldName = "ActualEndTime", FieldType = FieldType.DateTime, IsRequired = false, SortOrder = 15, IsActive = true }
            };

            var utFields = new[]
            {
                new ProcessField { ProcessDefinitionId = 2, FieldName = "TestSuiteName", FieldType = FieldType.Text, IsRequired = true, SortOrder = 1, MinLength = 3, MaxLength = 100, IsActive = true },
                new ProcessField { ProcessDefinitionId = 2, FieldName = "TotalTests", FieldType = FieldType.Number, IsRequired = true, SortOrder = 2, IsActive = true },
                new ProcessField { ProcessDefinitionId = 2, FieldName = "PassedTests", FieldType = FieldType.Number, IsRequired = true, SortOrder = 3, IsActive = true },
                new ProcessField { ProcessDefinitionId = 2, FieldName = "FailedTests", FieldType = FieldType.Number, IsRequired = true, SortOrder = 4, IsActive = true },
                new ProcessField { ProcessDefinitionId = 2, FieldName = "SkippedTests", FieldType = FieldType.Number, IsRequired = true, SortOrder = 5, IsActive = true },
                new ProcessField { ProcessDefinitionId = 2, FieldName = "CodeCoveragePercentage", FieldType = FieldType.Decimal, IsRequired = true, SortOrder = 6, IsActive = true },
                new ProcessField { ProcessDefinitionId = 2, FieldName = "TestExecutionDate", FieldType = FieldType.Date, IsRequired = true, SortOrder = 7, IsActive = true },
                new ProcessField { ProcessDefinitionId = 2, FieldName = "BuildStatus", FieldType = FieldType.Select, IsRequired = true, SortOrder = 8, OptionsJson = "[\"Passed\",\"Failed\",\"PartiallyPassed\"]", IsActive = true },
                new ProcessField { ProcessDefinitionId = 2, FieldName = "TestResult", FieldType = FieldType.Select, IsRequired = true, SortOrder = 9, OptionsJson = "[\"Passed\",\"Failed\",\"Blocked\"]", IsActive = true },
                new ProcessField { ProcessDefinitionId = 2, FieldName = "DefectsFound", FieldType = FieldType.Number, IsRequired = true, SortOrder = 10, IsActive = true },
                new ProcessField { ProcessDefinitionId = 2, FieldName = "CriticalDefectsFound", FieldType = FieldType.Number, IsRequired = true, SortOrder = 11, IsActive = true }
            };

            var incFields = new[]
            {
                new ProcessField { ProcessDefinitionId = 3, FieldName = "Severity", FieldType = FieldType.Select, IsRequired = true, SortOrder = 1, OptionsJson = "[\"P1\",\"P2\",\"P3\",\"P4\"]", IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "Impact", FieldType = FieldType.Select, IsRequired = true, SortOrder = 2, OptionsJson = "[\"Low\",\"Medium\",\"High\",\"Critical\"]", IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "IncidentCategory", FieldType = FieldType.Select, IsRequired = true, SortOrder = 3, OptionsJson = "[\"Application\",\"Database\",\"Network\",\"Infrastructure\",\"Security\",\"Other\"]", IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "OccurredAt", FieldType = FieldType.DateTime, IsRequired = true, SortOrder = 4, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "AcknowledgedAt", FieldType = FieldType.DateTime, IsRequired = false, SortOrder = 5, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "ResolvedAt", FieldType = FieldType.DateTime, IsRequired = false, SortOrder = 6, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "ClosedAt", FieldType = FieldType.DateTime, IsRequired = false, SortOrder = 7, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "AssignedTeam", FieldType = FieldType.Text, IsRequired = true, SortOrder = 8, MinLength = 2, MaxLength = 100, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "RootCauseIdentified", FieldType = FieldType.Boolean, IsRequired = true, SortOrder = 9, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "ResolutionProvided", FieldType = FieldType.Boolean, IsRequired = true, SortOrder = 10, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "RelatedChangeReference", FieldType = FieldType.Text, IsRequired = false, SortOrder = 11, MaxLength = 50, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "BusinessImpact", FieldType = FieldType.Text, IsRequired = true, SortOrder = 12, MinLength = 10, MaxLength = 500, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "SlaTargetMinutes", FieldType = FieldType.Number, IsRequired = true, SortOrder = 13, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "ActualResolutionMinutes", FieldType = FieldType.Number, IsRequired = false, SortOrder = 14, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "SlaBreached", FieldType = FieldType.Boolean, IsRequired = true, SortOrder = 15, IsActive = true },
                new ProcessField { ProcessDefinitionId = 3, FieldName = "IsRecurring", FieldType = FieldType.Boolean, IsRequired = true, SortOrder = 16, IsActive = true }
            };

            // Fix Label property requirement by making it identical to FieldName cleanly
            foreach(var f in crFields.Concat(utFields).Concat(incFields))
            {
                f.Label = f.FieldName;
            }

            await context.ProcessFields.AddRangeAsync(crFields.Concat(utFields).Concat(incFields));
            await context.SaveChangesAsync();
        }

        // 6. ServiceItems
        if (!await context.ServiceItems.AnyAsync())
        {
            var sItems = new[]
            {
                new ServiceItem { Id = 1, ApplicationId = 1, ProcessDefinitionId = 1, ReferenceNumber = "CR-2026-0001", Title = "Upgrade payment authorization service", Status = "In Progress", Priority = "High", AssignedTo = "Rahul Sharma", CreatedAt = DateTime.UtcNow },
                new ServiceItem { Id = 2, ApplicationId = 2, ProcessDefinitionId = 1, ReferenceNumber = "CR-2026-0002", Title = "Update gateway certificate configuration", Status = "Completed", Priority = "Medium", AssignedTo = "Priya Nair", CreatedAt = DateTime.UtcNow },
                new ServiceItem { Id = 3, ApplicationId = 1, ProcessDefinitionId = 2, ReferenceNumber = "UT-2026-0001", Title = "Payment authorization regression suite", Status = "Completed", Priority = "High", AssignedTo = "Amit Kumar", CreatedAt = DateTime.UtcNow },
                new ServiceItem { Id = 4, ApplicationId = 3, ProcessDefinitionId = 1, ReferenceNumber = "CR-2026-0003", Title = "Deploy customer profile enhancements", Status = "Planned", Priority = "Medium", AssignedTo = "Neha Reddy", CreatedAt = DateTime.UtcNow },
                new ServiceItem { Id = 5, ApplicationId = 3, ProcessDefinitionId = 3, ReferenceNumber = "INC-2026-0001", Title = "Customer portal login failures", Status = "Resolved", Priority = "Critical", AssignedTo = "Operations Team", CreatedAt = DateTime.UtcNow },
                new ServiceItem { Id = 6, ApplicationId = 5, ProcessDefinitionId = 2, ReferenceNumber = "UT-2026-0002", Title = "Validate daily ingestion pipeline", Status = "Completed", Priority = "Medium", AssignedTo = "Vikram Rao", CreatedAt = DateTime.UtcNow },
                new ServiceItem { Id = 7, ApplicationId = 5, ProcessDefinitionId = 3, ReferenceNumber = "INC-2026-0002", Title = "Delayed customer data ingestion", Status = "Open", Priority = "High", AssignedTo = "Data Operations Team", CreatedAt = DateTime.UtcNow }
            };

            await context.ServiceItems.AddRangeAsync(sItems);
            
            await context.Database.OpenConnectionAsync();
            try {
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ServiceItems ON");
                await context.SaveChangesAsync();
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ServiceItems OFF");
            } finally {
                await context.Database.CloseConnectionAsync();
            }
        }

        // 7. ProcessRecords
        if (!await context.ProcessRecords.AnyAsync())
        {
            var records = new[]
            {
                new ProcessRecord { 
                    ServiceItemId = 1, ProcessDefinitionId = 1, CreatedDate = DateTime.UtcNow, CreatedBy = "System",
                    DataJson = "{\"ChangeType\": \"Normal\",\"RiskLevel\": \"Medium\",\"PullRequestUrl\": \"https://github.com/example/payments-api/pull/1842\",\"PullRequestCreated\": true,\"PullRequestMerged\": true,\"CodeReviewCompleted\": true,\"UnitTestingCompleted\": true,\"DeploymentStatus\": \"InProgress\",\"DeploymentDate\": \"2026-09-15\",\"ChangeSuccessful\": true,\"RollbackRequired\": false,\"PlannedStartTime\": \"2026-09-15T20:00:00\",\"PlannedEndTime\": \"2026-09-15T21:00:00\",\"ActualStartTime\": \"2026-09-15T20:05:00\"}" 
                },
                new ProcessRecord { 
                    ServiceItemId = 2, ProcessDefinitionId = 1, CreatedDate = DateTime.UtcNow, CreatedBy = "System",
                    DataJson = "{\"ChangeType\": \"Standard\",\"RiskLevel\": \"Low\",\"PullRequestUrl\": \"https://github.com/example/payment-gateway/pull/921\",\"PullRequestCreated\": true,\"PullRequestMerged\": true,\"CodeReviewCompleted\": true,\"UnitTestingCompleted\": true,\"DeploymentStatus\": \"Completed\",\"DeploymentDate\": \"2026-09-10\",\"ChangeSuccessful\": true,\"RollbackRequired\": false,\"PlannedStartTime\": \"2026-09-10T22:00:00\",\"PlannedEndTime\": \"2026-09-10T22:30:00\",\"ActualStartTime\": \"2026-09-10T22:02:00\",\"ActualEndTime\": \"2026-09-10T22:25:00\"}" 
                },
                new ProcessRecord { 
                    ServiceItemId = 3, ProcessDefinitionId = 2, CreatedDate = DateTime.UtcNow, CreatedBy = "System",
                    DataJson = "{\"TestSuiteName\": \"Payment Authorization Regression Suite\",\"TotalTests\": 248,\"PassedTests\": 244,\"FailedTests\": 2,\"SkippedTests\": 2,\"CodeCoveragePercentage\": 94.75,\"TestExecutionDate\": \"2026-09-14\",\"BuildStatus\": \"Passed\",\"TestResult\": \"Passed\",\"DefectsFound\": 2,\"CriticalDefectsFound\": 0}" 
                },
                new ProcessRecord { 
                    ServiceItemId = 4, ProcessDefinitionId = 1, CreatedDate = DateTime.UtcNow, CreatedBy = "System",
                    DataJson = "{\"ChangeType\": \"Normal\",\"RiskLevel\": \"Low\",\"PullRequestUrl\": \"https://github.com/example/customer-portal/pull/437\",\"PullRequestCreated\": true,\"PullRequestMerged\": false,\"CodeReviewCompleted\": true,\"UnitTestingCompleted\": true,\"DeploymentStatus\": \"NotStarted\",\"ChangeSuccessful\": false,\"RollbackRequired\": false,\"PlannedStartTime\": \"2026-09-16T18:00:00\",\"PlannedEndTime\": \"2026-09-16T19:00:00\"}" 
                },
                new ProcessRecord { 
                    ServiceItemId = 5, ProcessDefinitionId = 3, CreatedDate = DateTime.UtcNow, CreatedBy = "System",
                    DataJson = "{\"Severity\": \"P1\",\"Impact\": \"High\",\"IncidentCategory\": \"Application\",\"OccurredAt\": \"2026-09-15T09:15:00\",\"AcknowledgedAt\": \"2026-09-15T09:20:00\",\"ResolvedAt\": \"2026-09-15T10:05:00\",\"ClosedAt\": \"2026-09-15T12:00:00\",\"AssignedTeam\": \"Customer Platform Operations\",\"RootCauseIdentified\": true,\"ResolutionProvided\": true,\"RelatedChangeReference\": \"CR-2026-0003\",\"BusinessImpact\": \"Customers were unable to authenticate through the customer portal during the incident window.\",\"SlaTargetMinutes\": 60,\"ActualResolutionMinutes\": 50,\"SlaBreached\": false,\"IsRecurring\": false}" 
                },
                new ProcessRecord { 
                    ServiceItemId = 6, ProcessDefinitionId = 2, CreatedDate = DateTime.UtcNow, CreatedBy = "System",
                    DataJson = "{\"TestSuiteName\": \"Daily Data Ingestion Pipeline Tests\",\"TotalTests\": 132,\"PassedTests\": 132,\"FailedTests\": 0,\"SkippedTests\": 0,\"CodeCoveragePercentage\": 97.40,\"TestExecutionDate\": \"2026-09-15\",\"BuildStatus\": \"Passed\",\"TestResult\": \"Passed\",\"DefectsFound\": 0,\"CriticalDefectsFound\": 0}" 
                },
                new ProcessRecord { 
                    ServiceItemId = 7, ProcessDefinitionId = 3, CreatedDate = DateTime.UtcNow, CreatedBy = "System",
                    DataJson = "{\"Severity\": \"P2\",\"Impact\": \"Medium\",\"IncidentCategory\": \"Infrastructure\",\"OccurredAt\": \"2026-09-15T14:10:00\",\"AcknowledgedAt\": \"2026-09-15T14:18:00\",\"AssignedTeam\": \"Data Operations Team\",\"RootCauseIdentified\": false,\"ResolutionProvided\": false,\"BusinessImpact\": \"Daily customer data processing was delayed and downstream consumers received incomplete data.\",\"SlaTargetMinutes\": 120,\"SlaBreached\": false,\"IsRecurring\": true}" 
                }
            };
            
            // Runtime dynamic validation Check
            var pd1 = await context.ProcessDefinitions.Include(p => p.ProcessFields).FirstAsync(c => c.Id == 1);
            var pd2 = await context.ProcessDefinitions.Include(p => p.ProcessFields).FirstAsync(c => c.Id == 2);
            var pd3 = await context.ProcessDefinitions.Include(p => p.ProcessFields).FirstAsync(c => c.Id == 3);

            foreach(var record in records)
            {
                var pd = record.ProcessDefinitionId == 1 ? pd1 : (record.ProcessDefinitionId == 2 ? pd2 : pd3);
                var validation = validator.Validate(pd, record.DataJson);
                if (!validation.IsValid)
                {
                    throw new Exception("Seed JSON mapping invalidated against strictly defined ProcessFields criteria.");
                }
            }

            await context.ProcessRecords.AddRangeAsync(records);
            await context.SaveChangesAsync();
        }
    }
}
