using AutoMapper;
using EmployeePortal.Business.Mapping;
using EmployeePortal.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace EmployeePortal.UnitTests.Fixtures;

public static class InMemoryPortalTestFixture
{
    public static async Task<PortalDbContext> CreateSeededDbContextAsync(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<PortalDbContext>()
            .UseInMemoryDatabase(dbName ?? $"PortalUnitTestDb_{Guid.NewGuid():N}")
            .Options;

        var context = new PortalDbContext(options);
        await PortalDbSeeder.SeedAsync(context);
        return context;
    }

    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<PortalMappingProfile>());
        return config.CreateMapper();
    }
}

