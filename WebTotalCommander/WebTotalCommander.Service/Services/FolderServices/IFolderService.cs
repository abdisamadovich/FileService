using WebTotalCommander.Service.ViewModels.Common;
using WebTotalCommander.Service.ViewModels.Folder;

namespace WebTotalCommander.Service.Services.FolderServices;

public interface IFolderService
{
    public Task<FolderGetAllViewModel> FolderGetAllAsync(FolderGetAllQueryViewModel folderGetAllQueryViewModel);
    public Task<bool> CreateFolderAsync(FolderViewModel folderViewModel);
    public Task<bool> DeleteFolderAsync(FolderViewModel folderViewModel);
    public Task<bool> RenameFolderAsync(FolderRenameViewModel folderRenameViewModel);
    public Task<(Stream memoryStream, string fileName)> DownloadFolderZipAsync(string folderPath, string folderName);
}
