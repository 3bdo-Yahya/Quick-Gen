using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quick_Gen.Contracts.Paths;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Controllers;

[ApiController]
[Route("api/admin/paths")]
[Authorize(Roles = "Admin")]
public sealed class AdminPathsController(IAdminPathService adminPathService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePathRequest request, CancellationToken cancellationToken)
    {
        var created = await adminPathService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(PathsController.GetById), "Paths", new { id = created.Id }, created);
    }

    [HttpPut("{pathId:int}")]
    public async Task<IActionResult> Update(int pathId, [FromBody] UpdatePathRequest request, CancellationToken cancellationToken)
    {
        var updated = await adminPathService.UpdateAsync(pathId, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{pathId:int}")]
    public async Task<IActionResult> Delete(int pathId, CancellationToken cancellationToken)
    {
        var deleted = await adminPathService.DeleteAsync(pathId, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("{pathId:int}/courses")]
    public async Task<IActionResult> ReplaceCourses(
        int pathId,
        [FromBody] ReplacePathCoursesRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await adminPathService.ReplaceCoursesAsync(pathId, request, cancellationToken).ConfigureAwait(false);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

