using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using DemoUniversity.TestssMs.UnitTests;

namespace DemoUniversity.TestssMs.UnitTests
{
    [TestClass]
    public class CrudTests
    {
        private SchoolContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the in-memory database with a unique name
            var options = TestSetup.GetInMemoryDbContextOptions();
            _context = new SchoolContext(options);

            // Seed the database
            TestSetup.SeedDatabase(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Dispose of the context after each test
            _context.Dispose();
        }

        [DataTestMethod]
        [DataRow(1, "John", "Doe")]  // Existing student
        [DataRow(3, "Alice", "Brown")]  // New student
        public void CreateOrUpdateStudent_ShouldAddOrUpdateStudent(int id, string firstName, string lastName)
        {
            // Arrange
            var student = new Student { ID = id, FirstMidName = firstName, LastName = lastName };

            // Act
            if (_context.Students.Any(s => s.ID == id))
            {
                // Update existing student
                var existingStudent = _context.Students.Find(id);
                existingStudent.FirstMidName = firstName;
                existingStudent.LastName = lastName;
            }
            else
            {
                // Add new student
                _context.Students.Add(student);
            }
            _context.SaveChanges();

            // Assert
            var result = _context.Students.Find(id);
            Assert.IsNotNull(result);
            Assert.AreEqual(firstName, result.FirstMidName);
            Assert.AreEqual(lastName, result.LastName);
        }

        [DataTestMethod]
        [DataRow(1)]  // Existing student
        [DataRow(999)]  // Non-existent student
        public void DeleteStudent_ShouldRemoveStudentIfExists(int id)
        {
            // Arrange
            var student = _context.Students.Find(id);

            // Act
            if (student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }

            // Assert
            var result = _context.Students.Find(id);
            if (id == 1) // Existing student
            {
                Assert.IsNull(result);
            }
            else // Non-existent student
            {
                Assert.IsNull(result); // Should remain null
            }
        }

        [DataTestMethod]
        [DataRow(1, "John", "Doe")]  // Existing student
        [DataRow(999, null, null)]  // Non-existent student
        public void ReadStudent_ShouldRetrieveStudentIfExists(int id, string expectedFirstName, string expectedLastName)
        {
            // Act
            var student = _context.Students.Find(id);

            // Assert
            if (expectedFirstName != null && expectedLastName != null)
            {
                Assert.IsNotNull(student);
                Assert.AreEqual(expectedFirstName, student.FirstMidName);
                Assert.AreEqual(expectedLastName, student.LastName);
            }
            else
            {
                Assert.IsNull(student);
            }
        }
    }
}