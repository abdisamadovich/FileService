using WebTotalCommander.Core.Errors;
using WebTotalCommander.FileAccess.Models.File;
using WebTotalCommander.Repository.Settings;

namespace WebTotalCommander.Repository.Files;

public class FileRepository : IFileRepository
{
    private readonly string _mainFolderName;

    public FileRepository(FolderSettings folderSettings)
    {
        this._mainFolderName = folderSettings.MainFolderName;
    }
    public async Task<bool> CreateFileAsync(FileModel fileModel)
    {
        var path = Path.Combine(_mainFolderName, fileModel.FilePath, fileModel.FileName);

        if (String.IsNullOrEmpty(fileModel.FileName))
        {
            throw new ParameterInvalidException("File name cannot be empty!");
        }
        if (File.Exists(path))
        {
            throw new AlreadeExsistException("File already exsist!");
        }

        if (String.IsNullOrEmpty(fileModel.FilePath))
        {
            fileModel.FilePath = "";
        }
        try
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await fileModel.File.CopyToAsync(stream);
                stream.Close();
            }
            return true;
        }
        catch (Exception ex)
        {
            throw new FileUnexpectedException($"File Unexpected error!", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(FileDeleteModel fileDeleteModel)
    {
        var path = Path.Combine(_mainFolderName, fileDeleteModel.FilePath, fileDeleteModel.FileName);

        if (String.IsNullOrEmpty(fileDeleteModel.FileName))
        {
            throw new ParameterInvalidException("File name cannot be empty!");
        }
        if (!File.Exists(path))
        {
            throw new EntryNotFoundException("File not found!");
        }

        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception ex)
        {
            throw new FileUnexpectedException($"File Unexpected error!.", ex);
        }
    }

    public async Task<FileDownloadModel> DownloadFileAsync(string filePath)
    {
        var path = Path.Combine(_mainFolderName, filePath);
        if (!File.Exists(path))
        {
            throw new EntryNotFoundException("File not found!");
        }
        try
        {
            using (var memory = new MemoryStream())
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Seek(0, SeekOrigin.Begin);

                var fileDownloadModel = new FileDownloadModel()
                {
                    File = memory,
                    FilePath = path
                };

                return fileDownloadModel;
            }

        }
        catch (Exception ex)
        {
            throw new FileUnexpectedException($"File Unexpected error!", ex);
        }
    }

    public async Task<bool> EditTextTxtFileAsync(string filePath, Stream file)
    {
        var path = Path.Combine(_mainFolderName, filePath);
        var fileInfo = new FileInfo(path);

        if (!File.Exists(path))
        {
            throw new EntryNotFoundException("File not found!");
        }
        if (fileInfo.Extension != ".txt")
        {
            throw new ParameterInvalidException("File not txt!");
        }

        try
        {
            File.Delete(path);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                stream.Close();
            }
            return true;
        }
        catch (Exception ex)
        {
            throw new FileUnexpectedException($"File Unexpected error!", ex);
        }
    }

    public async Task<Stream> GetTxtFileAsync(string filePath)
    {
        var path = Path.Combine(_mainFolderName, filePath);
        var fileInfo = new FileInfo(path);

        if (!File.Exists(path))
        {
            throw new EntryNotFoundException("File not found!");
        }
        if (fileInfo.Extension != ".txt")
        {
            throw new ParameterInvalidException("File not txt!");
        }

        try
        {
            var memory = new MemoryStream();
            await using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return memory;
        }
        catch (Exception ex)
        {
            throw new FileUnexpectedException($"File Unexpected error!", ex);
        }
    }
}
