using Dropbox.Api;
using Dropbox.Api.Files;
using WebTotalCommander.Core.Errors;
using WebTotalCommander.FileAccess.Models.File;
using WebTotalCommander.Repository.Settings;

namespace WebTotalCommander.Repository.Files;

public class FileDropboxRepository : IFileRepository
{
    private readonly string _mainFolderName;
    private readonly DropBoxSettings _dropBoxSettings;

    public FileDropboxRepository(FolderSettings folderSettings, DropBoxSettings dropBoxSettings)
    {
        this._mainFolderName = folderSettings.MainFolderName;
        this._dropBoxSettings = dropBoxSettings;
    }
    public async Task<bool> CreateFileAsync(FileModel fileModel)
    {
        var filePath = GetFilePath(fileModel.FileName, fileModel.FilePath);

        try
        {
            var accessToken = await _dropBoxSettings.GetAccessToken();
            var config = new DropboxClientConfig(accessToken)
            {
                HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) }
            };

            using (var dropboxClient = new DropboxClient(accessToken, config))
            {
                if (fileModel.File.Length <= 150 * 1024 * 1024)
                {
                    var uploadResult = await dropboxClient.Files.UploadAsync(filePath, body: fileModel.File);
                }
                else
                {
                    const int chunkSize = 50 * 1024 * 1024;
                    var sessionId = string.Empty;
                    var buffer = new byte[chunkSize];
                    var offset = 0UL;
                    int bytesRead;

                    using (var stream = fileModel.File)
                    {
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            var chunk = new MemoryStream(buffer, 0, bytesRead);
                            var uploadSessionCursor = offset == 0
                                ? new UploadSessionCursor(sessionId, offset)
                                : new UploadSessionCursor(sessionId, offset);
                            if (offset == 0)
                            {
                                var startSessionResult = await dropboxClient.Files.UploadSessionStartAsync(body: chunk);
                                sessionId = startSessionResult.SessionId;
                            }
                            else
                            {
                                await dropboxClient.Files.UploadSessionAppendV2Async(uploadSessionCursor, body: chunk);
                            }

                            offset += (ulong)bytesRead;
                        }

                        var commitInfo = new CommitInfo(filePath, WriteMode.Overwrite.Instance, false, DateTime.UtcNow);
                        await dropboxClient.Files.UploadSessionFinishAsync(new UploadSessionCursor(sessionId, offset), commitInfo, body: stream);
                    }
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            throw new FileUnexpectedException($"File Unexpected error!", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(FileDeleteModel fileDeleteModel)
    {
        var filePath = GetFilePath(fileDeleteModel.FileName, fileDeleteModel.FilePath);

        try
        {
            var accessToken = await _dropBoxSettings.GetAccessToken();
            var config = new DropboxClientConfig(accessToken)
            {
                HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) }
            };

            using (var dropboxClient = new DropboxClient(accessToken, config))
            {
                var result = await dropboxClient.Files.DeleteV2Async(filePath);
                return true;
            }
        }
        catch (ApiException<DeleteError> ex)
        {
            if (ex.ErrorResponse.AsPathLookup.Value.IsNotFound)
            {
                throw new EntryNotFoundException("File path found!");
            }
            else
            {
                throw new FileUnexpectedException($"File Unexpected error!", ex);
            }
        }
        catch (Exception ex)
        {
            throw new FileUnexpectedException($"File Unexpected error!", ex);
        }
    }

    public async Task<FileDownloadModel> DownloadFileAsync(string filePath)
    {
        var path = "/" + _mainFolderName.Trim('/');
        if (!String.IsNullOrEmpty(filePath))
        {
            path += "/" + filePath.Trim('/');
        }

        try
        {
            var accessToken = await _dropBoxSettings.GetAccessToken();
            var config = new DropboxClientConfig(accessToken)
            {
                HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) }
            };

            using (var dropboxClient = new DropboxClient(accessToken, config))
            {
                var response = await dropboxClient.Files.DownloadAsync(path);

                var fileStream = await response.GetContentAsStreamAsync();
                return new FileDownloadModel
                {
                    File = fileStream,
                    FilePath = path
                };

            }
        }
        catch (ApiException<DownloadError> ex)
        {
            if (ex.ErrorResponse.AsPath.Value.IsNotFound)
            {
                throw new EntryNotFoundException("File not found!");
            }
            else
            {
                throw new FileUnexpectedException($"File Unexpected error!", ex);
            }
        }
        catch (Exception ex)
        {
            throw new FileUnexpectedException($"File Unexpected error!", ex);
        }
    }

    public async Task<bool> EditTextTxtFileAsync(string filePath, Stream fileStream)
    {
        var fileExtension = Path.GetExtension(filePath)?.TrimStart('.');

        var path = "/" + _mainFolderName.Trim('/');
        if (!String.IsNullOrEmpty(filePath))
        {
            path += "/" + filePath.Trim('/');
        }
        else
        {
            throw new ParameterInvalidException("File name required!");
        }

        if (fileExtension != "txt")
        {
            throw new ParameterInvalidException("File not txt!");
        }

        var accessToken = await _dropBoxSettings.GetAccessToken();
        var config = new DropboxClientConfig(accessToken)
        {
            HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) }
        };

        using (var dropboxClient = new DropboxClient(accessToken, config))
        {
            try
            {
                var fileMetadata = await dropboxClient.Files.GetMetadataAsync(path);
            }
            catch (ApiException<GetMetadataError> ex)
            {
                if (ex.ErrorResponse.IsPath && ex.ErrorResponse.AsPath.Value.IsNotFound)
                {
                    throw new EntryNotFoundException("File not found!");
                }
            }

            try
            {
                var updatedFile = await dropboxClient.Files.UploadAsync(path, WriteMode.Overwrite.Instance, body: fileStream);
                return true;
            }
            catch (ApiException<UploadError> ex)
            {
                throw new FileUnexpectedException("Failed to upload file to Dropbox.", ex);
            }
            catch (Exception ex)
            {
                throw new FileUnexpectedException("An unexpected error occurred.", ex);
            }
        }
    }

    public async Task<Stream> GetTxtFileAsync(string filePath)
    {
        var fileExtension = Path.GetExtension(filePath)?.TrimStart('.');

        var path = "/" + _mainFolderName.Trim('/');
        if (!String.IsNullOrEmpty(filePath))
        {
            path += "/" + filePath.Trim('/');
        }
        else
        {
            throw new ParameterInvalidException("File name required!");
        }

        if (fileExtension != "txt")
        {
            throw new ParameterInvalidException("File not txt!");
        }

        try
        {
            var accessToken = await _dropBoxSettings.GetAccessToken();
            var config = new DropboxClientConfig(accessToken)
            {
                HttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) }
            };

            using (var dropboxClient = new DropboxClient(accessToken, config))
            {
                var response = await dropboxClient.Files.DownloadAsync(path);

                var fileStream = await response.GetContentAsStreamAsync();
                return fileStream;
            }
        }
        catch (ApiException<DownloadError> ex)
        {
            if (ex.ErrorResponse.AsPath.Value.IsNotFound)
            {
                throw new EntryNotFoundException("File not found!");
            }
            else
            {
                throw new FileUnexpectedException($"File Unexpected error!", ex);
            }
        }
        catch (Exception ex)
        {
            throw new FileUnexpectedException($"File Unexpected error!", ex);
        }
    }

    private string GetFilePath(string fileName, string filePath)
    {
        var path = "/" + _mainFolderName.Trim('/');
        if (!String.IsNullOrEmpty(filePath))
        {
            path += "/" + filePath.Trim('/');
        }
        return path + "/" + fileName;
    }
}
