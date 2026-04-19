using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quick_Gen.Contracts.Courses;
using Quick_Gen.Services.IServices;

namespace Quick_Gen.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CoursesController(ICourseService courseService) : ControllerBase
{
    // GET api/courses
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() =>
        Ok(await courseService.GetAllAsync());

    // GET api/courses/5
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await courseService.GetByIdAsync(id);
        return course is null ? NotFound() : Ok(course);
    }

    // POST api/courses
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        var created = await courseService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/courses/5
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request)
    {
        var updated = await courseService.UpdateAsync(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE api/courses/5
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await courseService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}