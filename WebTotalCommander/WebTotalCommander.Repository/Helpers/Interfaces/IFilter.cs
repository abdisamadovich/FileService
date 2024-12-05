using WebTotalCommander.FileAccess.Models.Common;

namespace WebTotalCommander.Repository.Helpers.Interfaces;

public interface IFilter
{
    public List<FolderFileModel> FilterFolder(FilterModel filter, List<FolderFileModel> folderList);
}
