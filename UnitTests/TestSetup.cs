using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace DemoUniversity.TestssMs.UnitTests
{
    public static class TestSetup
    {
        public static DbContextOptions<SchoolContext> GetInMemoryDbContextOptions()
        {
            // Generate a unique database name for each test run
            var databaseName = Guid.NewGuid().ToString();
            return new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
        }

        public static void SeedDatabase(SchoolContext context)
        {
            // Ensure the database is empty before seeding
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed the database with test data
            context.Students.AddRange(
                new Student { ID = 1, FirstMidName = "John", LastName = "Doe" },
                new Student { ID = 2, FirstMidName = "Jane", LastName = "Smith" }
            );
            context.SaveChanges();
        }
    }
}