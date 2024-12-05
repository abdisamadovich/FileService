using WebTotalCommander.FileAccess.Models.Common;
using WebTotalCommander.FileAccess.Models.Folder;

namespace WebTotalCommander.Repository.Folders;

public interface IFolderRepository
{
    public Task<FolderGetAllModel> GetAllFolderAsync(FolderGetAllQueryModel query);
    public Task<bool> CreateFolderAsync(FolderModel folder);
    public Task<bool> DeleteFolderAsync(FolderModel folder);
    public Task<bool> RenameFolderAsync(FolderRenameModel folderRenameModel);
    public Task<Stream> DownloadFolderZipAsync(string folderPath, string folderName);
}
