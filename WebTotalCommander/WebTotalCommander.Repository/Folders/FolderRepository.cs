using System.IO.Compression;
using WebTotalCommander.Core.Errors;
using WebTotalCommander.FileAccess.Models.Common;
using WebTotalCommander.FileAccess.Models.Folder;
using WebTotalCommander.FileAccess.Utils;
using WebTotalCommander.Repository.Helpers.Interfaces;
using WebTotalCommander.Repository.Settings;

namespace WebTotalCommander.Repository.Folders;

public class FolderRepository : IFolderRepository
{
    private readonly ISorter _sorter;
    private readonly IFilter _filter;
    private readonly IPaginator _paginator;
    private readonly string _mainFolderName;

    public FolderRepository(ISorter sorter, IFilter filter, IPaginator paginator, FolderSettings folderSettings)
    {
        this._sorter = sorter;
        this._filter = filter;
        this._paginator = paginator;
        this._mainFolderName = folderSettings.MainFolderName;
    }
    public async Task<FolderGetAllModel> GetAllFolderAsync(FolderGetAllQueryModel query)
    {
        if (query.Path == null) { query.Path = ""; }
        var path = Path.Combine(_mainFolderName, query.Path);

        if (!Directory.Exists(path))
        {
            throw new EntryNotFoundException("Folder not found!");
        }

        try
        {
            //Get all folder and file paths
            var filesPath = Directory.GetFiles(path);
            var foldersPath = Directory.GetDirectories(path);

            var result = new FolderGetAllModel();

            //Add folder data to FolderGetAllModel list
            foreach (var folderPath in foldersPath)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                FolderFileModel folderView = new FolderFileModel()
                {
                    Name = directoryInfo.Name,
                    Path = folderPath,
                    Extension = "folder",
                    Size = 0,
                    CreatedDate = directoryInfo.CreationTime
                };
                result.FolderFile.Add(folderView);
            }

            //Add file data to FolderGetAllModel list
            foreach (var filePath in filesPath)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                FolderFileModel fileView = new FolderFileModel()
                {
                    Name = fileInfo.Name,
                    Extension = fileInfo.Extension,
                    Path = filePath,
                    Size = Math.Ceiling((double)fileInfo.Length / 1024),
                    CreatedDate = fileInfo.CreationTime,
                };
                result.FolderFile.Add(fileView);
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
        catch (Exception ex)
        {
            throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
        }
    }

    public async Task<bool> CreateFolderAsync(FolderModel folder)
    {
        var path = Path.Combine(_mainFolderName, folder.FolderPath, folder.FolderName);
        if (Directory.Exists(path))
        {
            throw new AlreadeExsistException("Folder alreade exsist!");
        }

        try
        {
            var result = Directory.CreateDirectory(path);
            return true;
        }
        catch (Exception ex)
        {
            throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
        }
    }

    public async Task<bool> DeleteFolderAsync(FolderModel folder)
    {
        var path = Path.Combine(_mainFolderName, folder.FolderPath, folder.FolderName);
        if (!Directory.Exists(path))
        {
            throw new EntryNotFoundException("Folder not found!");
        }
        try
        {
            Directory.Delete(path, true);
            return true;
        }
        catch (Exception ex)
        {
            throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
        }
    }

    public async Task<bool> RenameFolderAsync(FolderRenameModel folderRenameModel)
    {
        var oldPath = Path.Combine(_mainFolderName, folderRenameModel.FolderPath, folderRenameModel.FolderOldName);
        if (!Directory.Exists(oldPath))
        {
            throw new EntryNotFoundException("Folder not found!");
        }
        try
        {
            var newPath = Path.Combine(_mainFolderName, folderRenameModel.FolderPath, folderRenameModel.FolderNewName);

            Directory.Move(oldPath, newPath);

            return true;
        }
        catch (Exception ex)
        {
            throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
        }
    }

    public async Task<Stream> DownloadFolderZipAsync(string folderPath, string folderName)
    {
        if (folderPath == null) { folderPath = ""; }
        var path = Path.Combine(_mainFolderName, folderPath, folderName);
        if (!Directory.Exists(path))
        {
            throw new EntryNotFoundException("Folder not found!");
        }

        try
        {
            var zipPath = Path.Combine(path + ".zip");
            ZipFile.CreateFromDirectory(path, zipPath);

            var memory = new MemoryStream();
            await using (var stream = new FileStream(zipPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            File.Delete(zipPath);

            return memory;
        }
        catch (Exception ex)
        {
            throw new FolderUnexpectedException($"Folder Unexpected error!", ex);
        }
    }
}
