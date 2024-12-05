using Microsoft.AspNetCore.Http;

namespace WebTotalCommander.DataAccess.Models;

public class File
{
    public string FileName { get; set; } = String.Empty;
    public IFormFile FileSource { get; set; } = default!;
}
