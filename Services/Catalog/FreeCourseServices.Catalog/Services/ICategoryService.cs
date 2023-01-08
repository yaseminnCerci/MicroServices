using FreeCourse.Shared.Dtos;
using FreeCourseServices.Catalog.Dtos;
using FreeCourseServices.Catalog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourseServices.Catalog.Services
{
  public  interface ICategoryService
    {

        Task<Response<List<CategoryDto>>> GetAllAsync();
        Task<Response<CategoryDto>> CreateAsync(CategoryDto categoryDto);
        Task<Response<CategoryDto>> GetByIdAsync(string Id);
    }
}
