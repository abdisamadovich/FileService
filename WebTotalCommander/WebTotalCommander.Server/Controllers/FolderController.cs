using Microsoft.AspNetCore.Mvc;
using WebTotalCommander.Service.Services.FolderServices;
using WebTotalCommander.Service.ViewModels.Common;
using WebTotalCommander.Service.ViewModels.Folder;

namespace WebTotalCommander.Server.Controllers;

[Route("api/folder")]
[ApiController]
public class FolderController : ControllerBase
{
    private IFolderService _service;
    public FolderController(IFolderService fileService)
    {
        this._service = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> FolderGetAll([FromQuery] FolderGetAllQueryViewModel query)
    {
        var folderGetAllView = await _service.FolderGetAllAsync(query);
        return Ok(folderGetAllView);
    }

    [HttpGet("zip")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> FolderDownloadZipAsync(string folderName, string folderPath)
    {
        var result = await _service.DownloadFolderZipAsync(folderPath, folderName);
        return File(result.memoryStream, "application/zip", result.fileName);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFolder(FolderViewModel folderViewModel)
    {
        var result = await _service.CreateFolderAsync(folderViewModel);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteFolderAsync(FolderViewModel folderViewModel)
    {
        var result = await _service.DeleteFolderAsync(folderViewModel);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> RenameFolder(FolderRenameViewModel folderRenameViewModel)
    {
        var result = await _service.RenameFolderAsync(folderRenameViewModel);
        return Ok(result);
    }
}
