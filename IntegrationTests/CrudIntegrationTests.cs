using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoUniversity.TestssMs.IntegrationTests
{
    [TestClass]
    public class CrudIntegrationTests
    {
        private SchoolContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the in-memory database with a unique name
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SchoolContext(options);

            // Seed the database
            _context.Students.AddRange(
                new Student { ID = 1, FirstMidName = "John", LastName = "Doe" },
                new Student { ID = 2, FirstMidName = "Jane", LastName = "Smith" }
            );
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Dispose of the context after each test
            _context.Dispose();
        }

        [TestMethod]
        public void CreateOrUpdateStudent_ShouldAddOrUpdateStudent()
        {
            // Arrange
            var student = new Student { ID = 3, FirstMidName = "Alice", LastName = "Brown" };

            // Act
            if (_context.Students.Any(s => s.ID == student.ID))
            {
                // Update existing student
                var existingStudent = _context.Students.Find(student.ID);
                existingStudent.FirstMidName = student.FirstMidName;
                existingStudent.LastName = student.LastName;
            }
            else
            {
                // Add new student
                _context.Students.Add(student);
            }
            _context.SaveChanges();

            // Assert
            var result = _context.Students.Find(student.ID);
            Assert.IsNotNull(result);
            Assert.AreEqual("Alice", result.FirstMidName);
            Assert.AreEqual("Brown", result.LastName);
        }

        [TestMethod]
        public void DeleteStudent_ShouldRemoveStudentIfExists()
        {
            // Arrange
            var id = 1; // Existing student

            // Act
            var student = _context.Students.Find(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }

            // Assert
            var result = _context.Students.Find(id);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ReadStudent_ShouldRetrieveStudentIfExists()
        {
            // Arrange
            var id = 1; // Existing student

            // Act
            var student = _context.Students.Find(id);

            // Assert
            Assert.IsNotNull(student);
            Assert.AreEqual("John", student.FirstMidName);
            Assert.AreEqual("Doe", student.LastName);
        }
    }
}