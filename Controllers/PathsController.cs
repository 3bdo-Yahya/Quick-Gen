using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PathsController(IPathService pathService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var paths = await pathService.GetAllAsync(cancellationToken);
        return Ok(paths);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var path = await pathService.GetByIdAsync(id, cancellationToken);
        return path is null ? NotFound() : Ok(path);
    }
}

