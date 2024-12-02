
namespace AppDirectoryInfo.Models
{
	public class DirectoryInfoModel
	{
		public string? Name { get; set; }
		public List<FileInfoModel> Files { get; set; } = new();
		public List<DirectoryInfoModel> NestedDirectories { get; set; } = new();
	}
}
