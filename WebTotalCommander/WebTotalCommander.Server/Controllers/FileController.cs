using Microsoft.AspNetCore.Mvc;
using WebTotalCommander.Service.Services.FileServices;
using WebTotalCommander.Service.ViewModels.File;

namespace WebTotalCommander.Server.Controllers;

[Route("api/file")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IFileService _service;

    public FileController(IFileService fileService)
    {
        this._service = fileService;
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> CreateFileAsync(IFormFile files, [FromForm] string filePath)
    {
        using (var stream = files.OpenReadStream())
        {
            FileViewModel fileViewModel = new FileViewModel();
            fileViewModel.FileName = files.FileName;
            fileViewModel.FilePath = filePath;
            fileViewModel.File = stream;
            var result = await _service.CreateFileAsync(fileViewModel);
            return Ok(result);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteFileAsync(FileDeleteViewModel fileDeleteView)
    {
        var result = await _service.DeleteFileAsync(fileDeleteView);
        return Ok(result);
    }

    [HttpGet]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> DownloadFileAsync([FromQuery] string filePath)
    {
        var result = await _service.DownloadFileAsync(filePath);
        return File(result.File, "application/octet-stream", result.FilePath);
    }

    [HttpPut("Text")]
    public async Task<IActionResult> EditTxtFileAsync(string filePath, IFormFile file)
    {
        using (var stream = file.OpenReadStream())
        {
            var result = await _service.EditTextTxtFileAsync(filePath, stream);
            return Ok(result);
        }
    }

    [HttpGet("Text")]
    public async Task<IActionResult> GetTxtFileAsync(string filePath)
    {
        var result = await _service.GetTxtFileAsync(filePath);
        return File(result, "application/txt");
    }
}
