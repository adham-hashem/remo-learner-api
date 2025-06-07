using RLA.Application.DTOs.CoursesDTOs;
using RLA.Application.DTOs.LevelsDtos;
using RLA.Application.DTOs.UserDTOs;
using RLA.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace RLA.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    // [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IProfessorService _professorService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly ILevelService _levelService;
        private readonly ITermService _termService;

        public AdminController(
            IProfessorService professorService,
            IStudentService studentService,
            ICourseService courseService,
            ILevelService levelService,
            ITermService termService)
        {
            _professorService = professorService;
            _studentService = studentService;
            _courseService = courseService;
            _levelService = levelService;
            _termService = termService;
        }

        [HttpPost("professors")]
        public async Task<IActionResult> CreateProfessor([FromBody] AddProfessorDto professorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var professor = await _professorService.CreateProfessorAsync(professorDto);
                return CreatedAtAction(nameof(GetProfessor), new { id = professor.UserId }, new { UserId = professor.UserId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("students")]
        public async Task<IActionResult> CreateStudent([FromBody] AddStudentDto studentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var student = await _studentService.CreateStudentAsync(studentDto);
                return CreatedAtAction(nameof(GetStudent), new { id = student.UserId }, new { UserId = student.UserId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto courseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var course = await _courseService.CreateCourseAsync(courseDto);
                return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, new { CourseId = course.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("students/{id:guid}")]
        public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _studentService.UpdateStudentAsync(id, userDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("professors/{id:guid}")]
        public async Task<IActionResult> UpdateProfessor(Guid id, [FromBody] UpdateUserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _professorService.UpdateProfessorAsync(id, userDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("students/{id:guid}")]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("professors/{id:guid}")]
        public async Task<IActionResult> DeleteProfessor(Guid id)
        {
            try
            {
                await _professorService.DeleteProfessorAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("professors")]
        public async Task<IActionResult> GetAllProfessors()
        {
            var professors = await _professorService.GetAllProfessorsAsync();
            return Ok(professors.Select(p => new
            {
                ProfessorId = p.Id,
                UserId = p.UserId,
                UserName = p.User?.UserName,
                UniversityId = p.User?.UniversityId,
                Email = p.User?.Email,
                PhoneNumber = p.User?.PhoneNumber,
                Role = "Professor"
            }));
        }

        [HttpGet("professors/{id:guid}")]
        public async Task<IActionResult> GetProfessor(Guid id)
        {
            var professor = await _professorService.GetProfessorByUserIdAsync(id);
            if (professor == null)
                return NotFound();

            return Ok(new
            {
                ProfessorId = professor.Id,
                UserId = professor.UserId,
                UserName = professor.User?.UserName,
                UniversityId = professor.User?.UniversityId,
                Email = professor.User?.Email,
                PhoneNumber = professor.User?.PhoneNumber,
                Role = "Professor"
            });
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students.Select(s => new
            {
                StudentId = s.Id,
                UserId = s.UserId,
                UserName = s.User?.UserName,
                Email = s.User?.Email,
                FullName = s.User?.FullName,
                UniversityId = s.User?.UniversityId,
                PhoneNumber = s.User?.PhoneNumber
            }));
        }

        [HttpGet("students/{id:guid}")]
        public async Task<IActionResult> GetStudent(Guid id)
        {
            var student = await _studentService.GetStudentByUserIdAsync(id);
            if (student == null)
                return NotFound();

            return Ok(new
            {
                StudentId = student.Id,
                UserId = student.UserId,
                UserName = student.User?.UserName,
                Email = student.User?.Email,
                FullName = student.User?.FullName,
                UniversityId = student.User?.UniversityId,
                PhoneNumber = student.User?.PhoneNumber
            });
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses.Select(c => new
            {
                CourseId = c.Id,
                c.Name,
                c.Code,
                c.Overview,
                DayOfWeek = c.DayOfWeek.ToString(),
                Time = c.Time.ToString("h:mm tt"),
                c.Location,
                c.CreditHours,
                c.LevelId,
                LevelNumber = c.Level?.Number,
                c.ProfessorId,
                ProfessorName = c.Professor?.User?.UserName,
                c.TermId,
                TermStartDate = c.Term?.StartDate,
                TermEndDate = c.Term?.EndDate
            }));
        }

        [HttpGet("courses/{id:guid}")]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            return Ok(new
            {
                CourseId = course.Id,
                course.Name,
                course.Code,
                course.Overview,
                DayOfWeek = course.DayOfWeek.ToString(),
                Time = course.Time.ToString("h:mm tt"),
                course.Location,
                course.CreditHours,
                course.LevelId,
                LevelNumber = course.Level?.Number,
                course.ProfessorId,
                ProfessorName = course.Professor?.User?.UserName,
                course.TermId,
                TermStartDate = course.Term?.StartDate,
                TermEndDate = course.Term?.EndDate
            });
        }
    }
}