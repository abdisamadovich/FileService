using Dropbox.Api;
using Dropbox.Api.Files;
using System.IO.Compression;
using System.Text.Json;
using WebTotalCommander.Core.Errors;
using WebTotalCommander.FileAccess.Models.Common;
using WebTotalCommander.FileAccess.Models.Folder;
using WebTotalCommander.FileAccess.Utils;
using WebTotalCommander.Repository.Helpers.Interfaces;
using WebTotalCommander.Repository.Settings;

namespace WebTotalCommander.Repository.Folders;

public class FolderDropboxRepository : IFolderRepository
{
    private readonly DropBoxSettings _dropBoxSettings;
    private readonly ISorter _sorter;
    private readonly IFilter _filter;
    private readonly IPaginator _paginator;
    private readonly string _mainFolderName;
    public FolderDropboxRepository(ISorter sorter, IFilter filter, IPaginator paginator, DropBoxSettings dropBoxSettings, FolderSettings folderSettings)
    {
        this._sorter = sorter;
        this._filter = filter;
        this._paginator = paginator;
        this._mainFolderName = folderSettings.MainFolderName;
        this._dropBoxSettings = dropBoxSettings;
    }

    public async Task<FolderGetAllModel> GetAllFolderAsync(FolderGetAllQueryModel query)
    {
        var path = "/" + _mainFolderName.Trim('/');
        if (!String.IsNullOrEmpty(query.Path))
        {
            path += "/" + query.Path.Trim('/');
        }

        var result = new FolderGetAllModel();

        var accessToken = await _dropBoxSettings.GetAccessToken();
        using (var dropboxClient = new DropboxClient(accessToken))
        {
            try
            {
                var list = await dropboxClient.Files.ListFolderAsync(path);
                foreach (var item in list.Entries)
                {
                    var folderFileModel = new FolderFileModel();
                    if (item.IsFolder)
                    {
                        var folder = (FolderMetadata)item;
                        folderFileModel.Name = folder.Name;
                        folderFileModel.CreatedDate = new DateTime(year: 1901, month: 1, day: 1);
                        folderFileModel.Size = 0;
                        folderFileModel.Path = folder.PathDisplay;
                        folderFileModel.Extension = "folder";
                    }
                    else if (item.IsFile)
                    {
                        var file = (FileMetadata)item;
                        folderFileModel.Name = file.Name;
                        folderFileModel.CreatedDate = file.ClientModified;
                        folderFileModel.Size = Math.Round((double)file.Size / 1024);
                        folderFileModel.Path = file.PathLower;
                        folderFileModel.Extension = '.' + Path.GetExtension(file.Name)?.TrimStart('.');
                    }

                    result.FolderFile.Add(folderFileModel);
                }

                //Filter FolderGetAllModel list
                if (query.Filter != null)
                {
                    result.FolderFile = _filter.FilterFolder(query.Filter, result.FolderFile);
                }

                //Sort FolderGetAllModel list
                if (query.SortDir == "desc")
                {
                    result.FolderFile = _sorter.SortDesc(query, result.FolderFile);
                }
                else if (query.SortDir == "asc")
                {
                    result.FolderFile = _sorter.SortAsc(query, result.FolderFile);
                }

                var paginationMetaData = _paginator.Paginate(result.FolderFile.Count, new PaginationParams(query.Offset, query.Limit));
                result.PaginationMetaData = paginationMetaData;

                result.FolderFile = result.FolderFile.Skip(query.Offset).Take(query.Limit).ToList();

                return result;
            }
            catch (ApiException<ListFolderError> ex)
            {
                if (ex.ErrorResponse.AsPath.Value.AsNotFound != null)
                {
                    throw new EntryNotFoundException("Folder path not found!");
                }
                else
                {
                    throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
                }
            }
            catch (Exception ex)
            {
                throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
            }

        }
    }

    public async Task<bool> CreateFolderAsync(FolderModel folder)
    {
        try
        {
            var folderPath = GetFolderPath(folder.FolderName, folder.FolderPath);

            var folderCreateArg = new CreateFolderArg(folderPath);
            var accessToken = await _dropBoxSettings.GetAccessToken();

            using (var dropboxClient = new DropboxClient(accessToken))
            {
                var folderCreateResponse = await dropboxClient.Files.CreateFolderV2Async(folderCreateArg);
                return true;
            }
        }
        catch (ApiException<CreateFolderError> ex)
        {
            if (ex.ErrorResponse.AsPath.Value.AsConflict != null)
            {
                throw new AlreadeExsistException("Folder alreade exsist!");
            }
            else
            {
                throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
            }
        }
        catch (Exception ex)
        {
            throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
        }
    }

    public async Task<bool> DeleteFolderAsync(FolderModel folder)
    {
        var folderPath = GetFolderPath(folder.FolderName, folder.FolderPath);

        try
        {
            var accessToken = await _dropBoxSettings.GetAccessToken();
            using (var dropboxClient = new DropboxClient(accessToken))
            {
                var result = await dropboxClient.Files.DeleteV2Async(folderPath);
                return true;
            }
        }
        catch (ApiException<DeleteError> ex)
        {
            if (ex.ErrorResponse.AsPathLookup.Value.AsNotFound != null)
            {
                throw new EntryNotFoundException("Folder not found!");
            }
            else
            {
                throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
            }
        }
        catch (Exception ex)
        {
            throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
        }
    }

    public async Task<Stream> DownloadFolderZipAsync(string folderPath, string folderName)
    {
        // Create path for DropboxCloud
        var path = GetFolderPath(folderName, folderPath);

        try
        {
            //Create temp folders for download folder
            var tempFolderDownloadPath = Path.Combine(_mainFolderName, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolderDownloadPath);
            var tempFolderLocalPath = Path.Combine(tempFolderDownloadPath, folderName);
            Directory.CreateDirectory(tempFolderLocalPath);

            var accessToken = await _dropBoxSettings.GetAccessToken();
            using (var dropboxClient = new DropboxClient(accessToken))
            {
                await DownloadFolderRecursiveAsync(dropboxClient, path, tempFolderLocalPath);
            }

            var tempZipPath = Path.Combine(tempFolderDownloadPath, folderName + ".zip");

            // Create a new zip file using ZipArchive
            using (var zipFileStream = new FileStream(tempZipPath, FileMode.Create))
            {
                using (var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create))
                {
                    foreach (var file in Directory.GetFiles(tempFolderLocalPath, "*", SearchOption.AllDirectories))
                    {
                        var entry = archive.CreateEntry(Path.GetRelativePath(tempFolderLocalPath, file).Replace('\\', '/'));
                        using (var entryStream = entry.Open())
                        {
                            using (var fileStream = File.OpenRead(file))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }

                    }
                }
            }

            // Read the zip file into a memory stream
            var memory = new MemoryStream();

            await using (var stream = new FileStream(tempZipPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            // Clean up temporary files and directories
            File.Delete(tempZipPath);
            Directory.Delete(tempFolderDownloadPath, true);

            return memory;

        }
        catch (ApiException<DownloadError> ex)
        {
            if (ex.ErrorResponse.AsPath.Value.AsNotFound != null)
            {
                throw new EntryNotFoundException("Folder not found!");
            }
            else
            {
                throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
            }
        }
        catch (Exception ex)
        {
            throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
        }
    }

    public async Task<bool> RenameFolderAsync(FolderRenameModel folderRenameModel)
    {
        var accessToken = await _dropBoxSettings.GetAccessToken();
        using (var droboxClient = new DropboxClient(accessToken))
        {
            var oldPath = GetFolderPath(folderRenameModel.FolderOldName, folderRenameModel.FolderPath);
            var newPath = GetFolderPath(folderRenameModel.FolderNewName, folderRenameModel.FolderPath);
            try
            {
                var relocationArg = new RelocationArg(fromPath: oldPath, toPath: newPath);
                var response = await droboxClient.Files.MoveV2Async(relocationArg);
                return true;
            }
            catch (ApiException<RelocationError> ex)
            {
                if (ex.ErrorResponse.AsFromLookup.Value.AsNotFound != null)
                {
                    throw new EntryNotFoundException("Folder not found!");
                }
                else
                {
                    throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
                }
            }
            catch (Exception ex)
            {
                throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
            }
        }
    }
    private async Task DownloadFolderRecursiveAsync(DropboxClient dropboxClient, string path, string localFolderPath)
    {
        var list = await dropboxClient.Files.ListFolderAsync(path);

        foreach (var entry in list.Entries)
        {
            if (entry.IsFile)
            {
                var file = (FileMetadata)entry;
                await DownloadFileAsync(dropboxClient, file.PathDisplay, localFolderPath);
            }
            else if (entry.IsFolder)
            {
                var subfolder = (FolderMetadata)entry;
                var subfolderPath = Path.Combine(localFolderPath, subfolder.Name);
                Directory.CreateDirectory(subfolderPath);
                await DownloadFolderRecursiveAsync(dropboxClient, subfolder.PathDisplay, subfolderPath);
            }
        }
    }

    private async Task DownloadFileAsync(DropboxClient dropboxClient, string filePath, string localFolderPath)
    {
        var response = await dropboxClient.Files.DownloadAsync(filePath);

        using (var fileStream = File.Create(Path.Combine(localFolderPath, Path.GetFileName(filePath))))
        {
            (await response.GetContentAsStreamAsync()).CopyTo(fileStream);
        }
    }

    private string GetFolderPath(string folderName, string folderPath)
    {
        var path = "/" + _mainFolderName.Trim('/');
        if (!String.IsNullOrEmpty(folderPath))
        {
            path += "/" + folderPath.Trim('/');
        }
        return path + "/" + folderName;
    }
}
