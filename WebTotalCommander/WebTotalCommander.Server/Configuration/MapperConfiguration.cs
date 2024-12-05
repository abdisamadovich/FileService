using AutoMapper;
using WebTotalCommander.FileAccess.Models.Common;
using WebTotalCommander.FileAccess.Models.File;
using WebTotalCommander.FileAccess.Models.Folder;
using WebTotalCommander.Service.ViewModels.Common;
using WebTotalCommander.Service.ViewModels.File;
using WebTotalCommander.Service.ViewModels.Folder;

namespace WebTotalCommander.Server.Configuration;

public class MapperConfiguration : Profile
{
    public MapperConfiguration()
    {
        CreateMap<FolderGetAllQueryModel, FolderGetAllQueryViewModel>().ReverseMap();
        CreateMap<FolderGetAllModel, FolderGetAllViewModel>().ReverseMap();
        CreateMap<FilterDefinitionModel, FilterDefinitionViewModel>().ReverseMap();
        CreateMap<FilterModel, FilterViewModel>().ReverseMap();
        CreateMap<SubFilterModel, SubFilterViewModel>().ReverseMap();
        CreateMap<FolderFileModel, FolderFileViewModel>().ReverseMap();
        CreateMap<FolderModel, FolderViewModel>().ReverseMap();
        CreateMap<FolderRenameModel, FolderRenameViewModel>().ReverseMap();
        CreateMap<FileModel, FileViewModel>().ReverseMap();
        CreateMap<FileDeleteModel, FileDeleteViewModel>().ReverseMap();
        CreateMap<FileDownloadModel, FileDownloadViewModel>().ReverseMap();
    }
}
