using WebTotalCommander.FileAccess.Models.File;

namespace WebTotalCommander.Repository.Files;

public interface IFileRepository
{
    public Task<bool> CreateFileAsync(FileModel fileModel);
    public Task<bool> DeleteFileAsync(FileDeleteModel fileDeleteModel);
    public Task<FileDownloadModel> DownloadFileAsync(string filePath);
    public Task<Stream> GetTxtFileAsync(string filePath);
    public Task<bool> EditTextTxtFileAsync(string filePath, Stream fileStream);
}
