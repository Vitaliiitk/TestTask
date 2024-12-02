using AppDirectoryInfo.Models;
using AppDirectoryInfo.Services;
using FluentAssertions;

namespace AppDirectoryInfo.UnitTests.Services
{
	public class DirectoryProcessorTests
	{
		private readonly DirectoryProcessor _processor;

		public DirectoryProcessorTests()
		{
			_processor = new DirectoryProcessor();
		}

		[Fact]
		public void LoadDirectory_ShouldReturnCorrectDirectoryInfo_WhenPathIsValid()
		{
			// Arrange
			string tempDirectory = Path.Combine(Path.GetTempPath(), "TestDirectory");
			Directory.CreateDirectory(tempDirectory);
			string testFile = Path.Combine(tempDirectory, "test.txt");
			File.WriteAllText(testFile, "Hello");

			// Act
			var result = _processor.LoadDirectory(tempDirectory);

			// Assert
			result.Should().NotBeNull();
			result.Name.Should().Be("TestDirectory");
			result.Files.Should().HaveCount(1);
			result.Files[0].Name.Should().Be("test");
			result.Files[0].Extension.Should().Be(".txt");

			// Cleanup
			File.Delete(testFile);
			Directory.Delete(tempDirectory);
		}

		[Fact]
		public void LoadDirectory_ShouldThrowDirectoryNotFoundException_WhenPathDoesNotExist()
		{
			// Arrange
			string invalidPath = @"C:\NonExistentDirectory";

			// Act
			Action act = () => _processor.LoadDirectory(invalidPath);

			// Assert
			act.Should().Throw<DirectoryNotFoundException>()
				.WithMessage($"The directory '{invalidPath}' does not exist.");
		}

		[Fact]
		public void SerializeToJson_ShouldCreateJsonFile_WhenGivenValidData()
		{
			// Arrange
			var directoryInfo = new DirectoryInfoModel
			{
				Name = "TestDirectory",
				Files = new List<FileInfoModel>
		{
			new FileInfoModel { Name = "test", Extension = ".txt" }
		}
			};
			string jsonPath = Path.Combine(Path.GetTempPath(), "directory.json");

			// Act
			_processor.SerializeToJson(directoryInfo, jsonPath);

			// Assert
			File.Exists(jsonPath).Should().BeTrue();
			var jsonContent = File.ReadAllText(jsonPath);
			jsonContent.Should().Contain("\"TestDirectory\"");

			// Cleanup
			File.Delete(jsonPath);
		}

		[Fact]
		public void SerializeToJson_ShouldThrowDirectoryNotFoundException_WhenOutputPathIsInvalid()
		{
			// Arrange
			var directoryInfo = new DirectoryInfoModel { Name = "TestDirectory" };
			string invalidPath = @"C:\Temp\InvalidPath\file.json";

			// Act
			Action act = () => _processor.SerializeToJson(directoryInfo, invalidPath);

			// Assert
			act.Should().Throw<DirectoryNotFoundException>();
		}

		[Fact]
		public void DeserializeFromJson_ShouldReturnDirectoryInfo_WhenJsonIsValid()
		{
			// Arrange
			string jsonPath = Path.Combine(Path.GetTempPath(), "directory.json");
			File.WriteAllText(jsonPath, "{\"Name\":\"TestDirectory\",\"Files\":[]}");

			// Act
			var result = _processor.DeserializeFromJson(jsonPath);

			// Assert
			result.Should().NotBeNull();
			result.Name.Should().Be("TestDirectory");

			// Cleanup
			File.Delete(jsonPath);
		}

		[Fact]
		public void DeserializeFromJson_ShouldThrowFileNotFoundException_WhenJsonFileDoesNotExist()
		{
			// Arrange
			string invalidPath = @"C:\Temp\NonExistentFile.json";

			// Act
			Action act = () => _processor.DeserializeFromJson(invalidPath);

			// Assert
			act.Should().Throw<FileNotFoundException>()
				.WithMessage($"The file '{invalidPath}' does not exist.");
		}

		[Fact]
		public void GetUniqueFileExtensions_ShouldReturnUniqueExtensions_WhenDirectoryInfoIsValid()
		{
			// Arrange
			var directoryInfo = new DirectoryInfoModel
			{
				Files = new List<FileInfoModel>
		{
			new FileInfoModel { Extension = ".txt" },
			new FileInfoModel { Extension = ".cs" },
			new FileInfoModel { Extension = ".txt" }
		}
			};

			// Act
			var result = _processor.GetUniqueFileExtensions(directoryInfo);

			// Assert
			result.Should().Contain(new[] { ".txt", ".cs" }).And.HaveCount(2);
		}
	}
}
