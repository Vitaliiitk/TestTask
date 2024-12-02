
using AppDirectoryInfo.Models;

namespace AppDirectoryInfo.Services.Interfaces
{
	public interface IDirectoryProcessor
	{
		DirectoryInfoModel LoadDirectory(string path);
		void SerializeToJson(DirectoryInfoModel directoryInfo, string outputPath);
		DirectoryInfoModel DeserializeFromJson(string jsonPath);
		IEnumerable<string> GetUniqueFileExtensions(DirectoryInfoModel directoryInfo);
	}
}
