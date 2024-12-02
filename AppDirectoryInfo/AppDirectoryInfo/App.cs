using AppDirectoryInfo.Models;
using AppDirectoryInfo.Services.Interfaces;

namespace AppDirectoryInfo
{
	public class App
	{
		private readonly IDirectoryProcessor _processor;

		public App(IDirectoryProcessor processor)
		{
			_processor = processor;
		}

		public void Run()
		{
			string inputPath;
			do
			{
				Console.WriteLine("Please provide a folder or a JSON file path with folder information (or type 'exit' to quit):");
				inputPath = Console.ReadLine() ?? string.Empty;

				if (inputPath?.Trim().ToLower() == "exit")
				{
					Console.WriteLine("Exiting the program.");
					break;
				}

				if (string.IsNullOrWhiteSpace(inputPath))
				{
					Console.WriteLine("Invalid input. Please provide a valid folder path or JSON file path.");
					continue;
				}

				try
				{
					if (Directory.Exists(inputPath))
					{
						Console.WriteLine($"Processing directory: {inputPath}");
						var directoryInfo = _processor.LoadDirectory(inputPath);

						var uniqueExtensions = _processor.GetUniqueFileExtensions(directoryInfo);
						Console.WriteLine("Extensions found in the folder:");
						foreach (var ext in uniqueExtensions)
						{
							Console.WriteLine(ext);
						}

						HandleJsonSave(directoryInfo);
					}
					else if (File.Exists(inputPath) && Path.GetExtension(inputPath).Equals(".json", StringComparison.OrdinalIgnoreCase))
					{
						Console.WriteLine($"Processing JSON file: {inputPath}");
						var deserializedInfo = _processor.DeserializeFromJson(inputPath);

						var uniqueExtensions = _processor.GetUniqueFileExtensions(deserializedInfo);
						Console.WriteLine("Extensions found in the folder:");
						foreach (var ext in uniqueExtensions)
						{
							Console.WriteLine(ext);
						}

						HandleJsonSave(deserializedInfo);
					}
					else
					{
						Console.WriteLine("The provided input is neither a valid directory nor a JSON file.");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"An error occurred: {ex.Message}");
				}

				Console.WriteLine();

			} while (true);
		}

		private void HandleJsonSave(DirectoryInfoModel directoryInfo)
		{
			Console.WriteLine("Save to JSON? (y/n):");
			var saveToJson = Console.ReadLine()?.Trim().ToLower();

			if (saveToJson == "y")
			{
				Console.WriteLine("Please provide the JSON file location (e.g., C:\\Temp\\ExampleFolder\\MyFolderInfo.json):");
				var outputPath = Console.ReadLine()?.Trim();

				if (string.IsNullOrWhiteSpace(outputPath))
				{
					Console.WriteLine("Invalid path. Skipping saving to JSON.");
					return;
				}

				if (!outputPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine("The provided file path is not a valid JSON file. Please provide a valid .json file path.");
					return;
				}

				if (File.Exists(outputPath))
				{
					Console.WriteLine($"The file '{outputPath}' already exists. Do you want to overwrite it? (y/n):");
					var overwrite = Console.ReadLine()?.Trim().ToLower();
					if (overwrite != "y")
					{
						Console.WriteLine("Skipping saving to JSON.");
						return;
					}
				}

				try
				{
					_processor.SerializeToJson(directoryInfo, outputPath);
					Console.WriteLine($"Directory information serialized to {outputPath}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to save JSON: {ex.Message}");
				}
			}
			else
			{
				Console.WriteLine("Skipped saving to JSON.");
			}
		}
	}
}
