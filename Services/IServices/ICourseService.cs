using Quick_Gen.Contracts.Courses;

namespace Quick_Gen.Services.IServices
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseResponse>> GetAllAsync();
        Task<CourseResponse?> GetByIdAsync(int id);
        Task<CourseResponse> CreateAsync(CreateCourseRequest request);
        Task<CourseResponse?> UpdateAsync(int id, UpdateCourseRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
