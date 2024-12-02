using AppDirectoryInfo.Models;
using AppDirectoryInfo.Services.Interfaces;
using System.Text.Json;

namespace AppDirectoryInfo.Services
{
	public class DirectoryProcessor : IDirectoryProcessor
	{
		public DirectoryInfoModel LoadDirectory(string path)
		{
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException($"The directory '{path}' does not exist.");

			var directoryInfo = new DirectoryInfoModel
			{
				Name = Path.GetFileName(path)
			};

			foreach (var file in Directory.GetFiles(path))
			{
				var fileInfo = new FileInfo(file);
				directoryInfo.Files.Add(new FileInfoModel
				{
					Name = Path.GetFileNameWithoutExtension(fileInfo.Name),
					Extension = fileInfo.Extension
				});
			}

			foreach (var dir in Directory.GetDirectories(path))
			{
				directoryInfo.NestedDirectories.Add(LoadDirectory(dir));
			}

			return directoryInfo;
		}

		public void SerializeToJson(DirectoryInfoModel directoryInfo, string outputPath)
		{
			var json = JsonSerializer.Serialize(directoryInfo, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(outputPath, json);
		}

		public DirectoryInfoModel DeserializeFromJson(string jsonPath)
		{
			if (!File.Exists(jsonPath))
				throw new FileNotFoundException($"The file '{jsonPath}' does not exist.");

			var json = File.ReadAllText(jsonPath);
			var result = JsonSerializer.Deserialize<DirectoryInfoModel>(json);
			return result == null ? throw new InvalidOperationException($"Failed to deserialize JSON from the file '{jsonPath}'.") : result;
		}

		public IEnumerable<string> GetUniqueFileExtensions(DirectoryInfoModel directoryInfo)
		{
			var extensions = new HashSet<string>();

			void CollectExtensions(DirectoryInfoModel dir)
			{
				foreach (var file in dir.Files)
				{
					if (!string.IsNullOrEmpty(file.Extension))
						extensions.Add(file.Extension);
				}
				foreach (var nestedDir in dir.NestedDirectories)
				{
					CollectExtensions(nestedDir);
				}
			}

			CollectExtensions(directoryInfo);
			return extensions;
		}
	}
}
