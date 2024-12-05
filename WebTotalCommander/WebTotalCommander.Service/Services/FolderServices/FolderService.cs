using AutoMapper;
using System.Text.RegularExpressions;
using WebTotalCommander.Core.Errors;
using WebTotalCommander.FileAccess.Models.Common;
using WebTotalCommander.FileAccess.Models.Folder;
using WebTotalCommander.Repository.Folders;
using WebTotalCommander.Service.ViewModels.Common;
using WebTotalCommander.Service.ViewModels.Folder;

namespace WebTotalCommander.Service.Services.FolderServices;

public class FolderService : IFolderService
{
    private readonly IFolderRepository _repository;
    private readonly IMapper _mapper;

    public FolderService(IFolderRepository folderRepository, IMapper mapper)
    {
        this._repository = folderRepository;
        this._mapper = mapper;
    }

    public async Task<FolderGetAllViewModel> FolderGetAllAsync(FolderGetAllQueryViewModel folderGetAllQueryViewModel)
    {
        var folderGetAllQueryModel = _mapper.Map<FolderGetAllQueryModel>(folderGetAllQueryViewModel);
        var folderGetAllModel = await _repository.GetAllFolderAsync(folderGetAllQueryModel);

        var result = _mapper.Map<FolderGetAllViewModel>(folderGetAllModel);
        return result;
    }

    public async Task<bool> CreateFolderAsync(FolderViewModel folderViewModel)
    {
        if (IsValidFolderName(folderViewModel.FolderName))
        {
            throw new ParameterInvalidException("Cannot create folder with this name!");
        }

        var folder = _mapper.Map<FolderModel>(folderViewModel);

        var result = await _repository.CreateFolderAsync(folder);

        return result;
    }

    public Task<bool> DeleteFolderAsync(FolderViewModel folderViewModel)
    {
        var folder = _mapper.Map<FolderModel>(folderViewModel);
        var result = _repository.DeleteFolderAsync(folder);

        return result;
    }

    public async Task<(Stream memoryStream, string fileName)> DownloadFolderZipAsync(string folderPath, string folderName)
    {
        var memory = await _repository.DownloadFolderZipAsync(folderPath, folderName);

        return (memory, folderName);
    }

    public async Task<bool> RenameFolderAsync(FolderRenameViewModel folderRenameViewModel)
    {
        if (IsValidFolderName(folderRenameViewModel.FolderNewName))
        {
            throw new ParameterInvalidException("Cannot rename folder with this name!");
        }
        var folderRenameModel = _mapper.Map<FolderRenameModel>(folderRenameViewModel);

        var result = await _repository.RenameFolderAsync(folderRenameModel);
        return result;
    }

    private static bool IsValidFolderName(string folderName)
    {
        var dotCount = 0;
        for (var i = 0; i < folderName.Length; i++)
        {
            if (folderName[i] == '.')
            {
                dotCount++;
            }
        }
        if (dotCount == folderName.Length)
        {
            return true;
        }
        var pattern = @"^[^<>:""/\\|?*]+$";
        var regex = new Regex(pattern);
        return !regex.IsMatch(folderName);
    }
}
