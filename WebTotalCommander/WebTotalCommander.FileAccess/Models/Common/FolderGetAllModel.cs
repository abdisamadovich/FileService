using WebTotalCommander.FileAccess.Utils;

namespace WebTotalCommander.FileAccess.Models.Common;

public class FolderGetAllModel
{
    private List<FolderFileModel> _fileFolders;
    public List<FolderFileModel> FolderFile
    {
        get => _fileFolders ??= new List<FolderFileModel>();
        set => _fileFolders = value;
    }
    public PaginationMetaData PaginationMetaData { get; set; } = new PaginationMetaData();
}
