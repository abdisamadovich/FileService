using WebTotalCommander.Service.ViewModels.File;

namespace WebTotalCommander.Service.Services.FileServices;

public interface IFileService
{
    public Task<bool> CreateFileAsync(FileViewModel fileView);
    public Task<bool> DeleteFileAsync(FileDeleteViewModel fileDeleteView);
    public Task<FileDownloadViewModel> DownloadFileAsync(string filePath);
    public Task<Stream> GetTxtFileAsync(string filePath);
    public Task<bool> EditTextTxtFileAsync(string filePath, Stream formFile);

}
