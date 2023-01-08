using System;
using AutoMapper;
using FreeCourse.Shared.Dtos;
using FreeCourseServices.Catalog.Dtos;
using FreeCourseServices.Catalog.Models;
using FreeCourseServices.Catalog.Settings;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourseServices.Catalog.Services
{
    public class CourseServices: ICourseServices
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public CourseServices(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;

        }
        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var coursess = await _courseCollection.Find(course => true).ToListAsync();
           
            if (coursess.Any())
            {
                foreach (var course in coursess)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }

            }
            else
            {
                coursess = new List<Course>();
            }
            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(coursess), 200);
        }
      
        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {

            var course = await _courseCollection.Find(course => course.Id == id).FirstOrDefaultAsync();
            if (course == null)
            {
                return Response<CourseDto>.Fail("Category Not Found.", 404);
            }

            course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
        }



        public async Task<Response<List<CourseDto>>> GetAllByUserId(string userId)
        {
            var coursess = await _courseCollection.Find(course => course.UserId==userId).ToListAsync();

            if (coursess.Any())
            {
                foreach (var course in coursess)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }

            }
            else
            {
                coursess = new List<Course>();
            }
            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(coursess), 200);
        }
        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var newCourse = _mapper.Map<Course>(courseCreateDto);
            newCourse.CreatedTime = DateTime.Now;
            await _courseCollection.InsertOneAsync(newCourse);
            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);
        }
        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);

            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);
            if (result == null)
            {
                return Response<NoContent>.Fail("Category Not Found.", 404);
            }
            return Response<NoContent>.Success(204);
        }
        public async Task<Response<NoContent>> DeleteAsync(string Id)
        {

            var result = await _courseCollection.DeleteOneAsync(course => course.Id == Id);
            if (result.DeletedCount >0)
            {
                return Response<NoContent>.Success(204);
            }
            else
            {
                return Response<NoContent>.Fail("Category Not Found.", 404);
            }

        }

    }
}
