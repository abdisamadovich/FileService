using WebTotalCommander.FileAccess.Models.Common;

namespace WebTotalCommander.Repository.Helpers.Interfaces;

public interface ISorter
{
    public List<FolderFileModel> SortDesc(FolderGetAllQueryModel query, List<FolderFileModel> folderFileViewModels);
    public List<FolderFileModel> SortAsc(FolderGetAllQueryModel query, List<FolderFileModel> folderFileViewModels);
}