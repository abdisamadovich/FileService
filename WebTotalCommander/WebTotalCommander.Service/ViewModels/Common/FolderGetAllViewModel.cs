using WebTotalCommander.FileAccess.Utils;

namespace WebTotalCommander.Service.ViewModels.Common;

public class FolderGetAllViewModel
{
    private List<FolderFileViewModel> _fileFolders;

    public List<FolderFileViewModel> FolderFile
    {
        get => _fileFolders ??= new List<FolderFileViewModel>();
        set => _fileFolders = value;
    }
    public PaginationMetaData PaginationMetaData { get; set; } = new PaginationMetaData();
}
