using System;
using System.IO;
using System.Linq;
using Stellinator.FileSystem;
using Stellinator.Interfaces;
using Xunit;

namespace StellinatorTests
{
    public sealed class FileSystemHandlerTests : IDisposable, IClassFixture<WriterSinkFixture>
    {
        private readonly string tempFolder;
        private readonly string testFolder;
        private readonly string testFile;
        private readonly string testFile2;
        private readonly IFileSystem fileSystem;
        private const string TEST_FILE = "test.txt";
        private const string TEST_FILE2 = "test2.txt";

        public FileSystemHandlerTests(WriterSinkFixture _)
        {
            var testId = Guid.NewGuid().ToString();
            tempFolder = Path.GetTempPath();
            testFolder = Path.Combine(tempFolder, testId);
            Directory.CreateDirectory(testFolder);
            testFile = Path.Combine(testFolder, TEST_FILE);
            File.WriteAllText(testFile, TEST_FILE);
            testFile2 = Path.Combine(testFolder, TEST_FILE2);
            File.WriteAllText(testFile2, TEST_FILE2);
            fileSystem = new FileSystemHandler();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DirectoryExists_Returns_Valid_Result(bool shouldExist)
        {
            // arrange
            var dir = shouldExist ? testFolder :
                Path.Combine(tempFolder, Guid.NewGuid().ToString());

            // act
            var actual = fileSystem.DirectoryExists(dir);

            // assert
            Assert.Equal(shouldExist, actual);
        }

        [Fact]
        public void CopyFile_Copies_To_Destination()
        {
            // arrange
            var target = Path.Combine(testFolder, "test-copy.txt");
            var srcContent = File.ReadAllText(testFile);

            // act
            fileSystem.CopyFile(testFile, target);
            var tgtContent = File.ReadAllText(target);

            // assert
            Assert.True(File.Exists(target));
            Assert.Equal(srcContent, tgtContent);
        }

        [Fact]
        public void CopyFile_Appends_Sequence_When_Target_File_Exists()
        {
            // arrange
            var target = testFile2;
            var srcContent = File.ReadAllText(testFile);

            // act
            fileSystem.CopyFile(testFile, target);
            var targetFile = Directory.EnumerateFiles(testFolder)
                .Where(f => Path.GetFileName(f).StartsWith("test2") && f != testFile2)
                .Single();
            var tgtContent = File.ReadAllText(targetFile);

            // assert
            Assert.True(string.IsNullOrWhiteSpace(targetFile) == false);
            Assert.Equal(srcContent, tgtContent);
        }

        [Fact]
        public void CreateDirectory_Creates_Directory()
        {
            // arrange
            var folder = Guid.NewGuid().ToString();
            var directory = Path.Combine(tempFolder, folder);

            // act
            fileSystem.CreateDirectory(directory);

            // assert
            Assert.True(Directory.Exists(directory));
        }
        
        [Theory]
        [InlineData(TEST_FILE, true)]
        [InlineData(TEST_FILE2, true)]
        [InlineData("x" + TEST_FILE, false)]
        public void File_Exists_Returns_True_When_File_Exists(
            string file,
            bool expected)
        {
            // arrange
            var fileToCheck = Path.Combine(testFolder, file);

            // act
            var actual = fileSystem.FileExists(fileToCheck);

            // assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFiles_Gets_Files_In_Directory()
        {
            // arrange
            var expected = new[] { testFile, testFile2 };

            // act
            var actual = fileSystem.GetFiles(testFolder);

            // assert
            Assert.Equal(expected.OrderBy(e => e), actual.OrderBy(a => a));
        }

        [Fact]
        public void GetSubdirectories_Gets_Subdirectories_In_Directory()
        {
            // arrange
            var subdir1 = Path.Combine(testFolder, Guid.NewGuid().ToString());
            var subdir2 = Path.Combine(testFolder, Guid.NewGuid().ToString());
            Directory.CreateDirectory(subdir1);
            Directory.CreateDirectory(subdir2);
            var expected = new[] { subdir1, subdir2 };

            // act
            var actual = fileSystem.GetSubdirectories(testFolder);

            // assert
            Assert.Equal(expected.OrderBy(e => e), actual.OrderBy(a => a));
        }

        public void Dispose()
        {
            Directory.Delete(testFolder, true);
        }
    }
}
