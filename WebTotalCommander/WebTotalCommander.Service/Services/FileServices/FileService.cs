using AutoMapper;
using WebTotalCommander.FileAccess.Models.File;
using WebTotalCommander.Repository.Files;
using WebTotalCommander.Repository.Settings;
using WebTotalCommander.Service.ViewModels.File;

namespace WebTotalCommander.Service.Services.FileServices;

public class FileService : IFileService
{
    private readonly IFileRepository _repository;
    private readonly string _mainFolderName;
    private readonly IMapper _mapper;

    public FileService(IFileRepository fileRepository, FolderSettings folderSettings, IMapper mapper)
    {
        this._repository = fileRepository;
        this._mainFolderName = folderSettings.MainFolderName;
        this._mapper = mapper;
    }

    public async Task<bool> CreateFileAsync(FileViewModel fileView)
    {
        var fileModel = _mapper.Map<FileModel>(fileView);
        var result = await _repository.CreateFileAsync(fileModel);

        return result;
    }

    public async Task<bool> DeleteFileAsync(FileDeleteViewModel fileView)
    {
        var fileDeleteModel = _mapper.Map<FileDeleteModel>(fileView);
        var result = await _repository.DeleteFileAsync(fileDeleteModel);

        return result;
    }

    public async Task<FileDownloadViewModel> DownloadFileAsync(string filePath)
    {
        var fileDownloadModel = await _repository.DownloadFileAsync(filePath);
        var fileDownloadViewModel = _mapper.Map<FileDownloadViewModel>(fileDownloadModel);

        return fileDownloadViewModel;
    }

    public async Task<bool> EditTextTxtFileAsync(string filePath, Stream file)
    {
        var result = await _repository.EditTextTxtFileAsync(filePath, file);
        return result;
    }

    public async Task<Stream> GetTxtFileAsync(string filePath)
    {
        var result = await _repository.GetTxtFileAsync(filePath);
        return result;
    }
}
