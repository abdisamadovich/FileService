using WebTotalCommander.FileAccess.Models.Common;
using WebTotalCommander.Repository.Helpers.Interfaces;

namespace WebTotalCommander.Repository.Helpers.Services;
public class Sorter : ISorter
{
    public List<FolderFileModel> SortAsc(FolderGetAllQueryModel query, List<FolderFileModel> folderFileViewModels)
    {
        return query.SortField switch
        {
            "name" => folderFileViewModels.OrderBy(x => x.Name).ToList(),
            "extension" => folderFileViewModels.OrderBy(x => x.Extension).ToList(),
            "createdDate" => folderFileViewModels.OrderBy(x => x.CreatedDate).ToList(),
            "size" => folderFileViewModels.OrderBy(x => x.Size).ToList(),
            _ => throw new ArgumentException($"Invalid sort field: {query.SortField}")
        };
    }

    public List<FolderFileModel> SortDesc(FolderGetAllQueryModel query, List<FolderFileModel> folderFileViewModels)
    {
        return query.SortField switch
        {
            "name" => folderFileViewModels.OrderByDescending(x => x.Name).ToList(),
            "extension" => folderFileViewModels.OrderByDescending(x => x.Extension).ToList(),
            "createdDate" => folderFileViewModels.OrderByDescending(x => x.CreatedDate).ToList(),
            "size" => folderFileViewModels.OrderByDescending(x => x.Size).ToList(),
            _ => throw new ArgumentException($"Invalid sort field: {query.SortField}")
        };
    }
}
