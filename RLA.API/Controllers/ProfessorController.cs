using RLA.Application.DTOs.CourseDiscussionsDTOs;
using RLA.Application.DTOs.MaterialsDTOs;
using RLA.Application.DTOs.QuizzesDTOs;
using RLA.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RLA.API.Controllers
{
    [Route("api/professor")]
    [ApiController]
    // [Authorize(Roles = "Professor")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorService _professorService;
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;
        private readonly IMaterialService _materialService;
        private readonly IQuizService _quizService;
        private readonly ICourseDiscussionService _discussionService;

        public ProfessorController(
            IProfessorService professorService,
            ICourseService courseService,
            IStudentService studentService,
            IMaterialService materialService,
            IQuizService quizService,
            ICourseDiscussionService discussionService)
        {
            _professorService = professorService;
            _courseService = courseService;
            _studentService = studentService;
            _materialService = materialService;
            _quizService = quizService;
            _discussionService = discussionService;
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetProfessorCourses()
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
                return Unauthorized("User ID not found in token!");
            var userId = Guid.Parse(subClaim.Value);

            var professor = await _professorService.GetProfessorByUserIdAsync(userId);
            if (professor == null)
                return NotFound("Professor not found for this user.");

            var courses = await _courseService.GetCoursesByProfessorIdAsync(userId);
            return Ok(courses.Select(c => new
            {
                c.Id,
                c.Name,
                DayOfWeek = c.DayOfWeek.ToString(),
                Time = c.Time.ToString("h:mm tt"),
                c.Location,
                TotalMaterials = c.Materials?.Count ?? 0,
                TotalQuizzes = c.Quizzes?.Count ?? 0,
                TotalDiscussions = c.Discussions?.Count ?? 0
            }));
        }

        [HttpGet("courses/{courseId:guid}/students")]
        public async Task<IActionResult> GetCourseStudents(Guid courseId)
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
                return Unauthorized("User ID not found in token!");
            var userId = Guid.Parse(subClaim.Value);

            var professor = await _professorService.GetProfessorByUserIdAsync(userId);
            if (professor == null)
                return NotFound("Professor not found for this user.");

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null || course.ProfessorId != userId)
                return NotFound("Course not found or you are not authorized to access it.");

            var students = await _studentService.GetStudentsByCourseIdAsync(courseId);
            return Ok(students.Select(sc => new
            {
                StudentId = sc.Id,
                FullName = sc.User?.FullName,
                FinalGrade = sc.StudentCourses?.FirstOrDefault(sc => sc.CourseId == courseId)?.FinalGrade
            }));
        }

        [HttpPost("courses/{courseId:guid}/materials")]
        public async Task<IActionResult> AddMaterial(Guid courseId, [FromBody] AddMaterialDto materialDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var material = await _materialService.CreateMaterialAsync(courseId, materialDto);
                return CreatedAtAction(nameof(GetMaterial), new { courseId, materialId = material.Id }, new { MaterialId = material.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("courses/{courseId:guid}/materials/{materialId:guid}")]
        public async Task<IActionResult> GetMaterial(Guid courseId, Guid materialId)
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
                return Unauthorized("User ID not found in token!");
            var userId = Guid.Parse(subClaim.Value);

            var professor = await _professorService.GetProfessorByUserIdAsync(userId);
            if (professor == null)
                return NotFound("Professor not found for this user.");

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null || course.ProfessorId != userId)
                return NotFound("Course not found or you are not authorized to access it.");

            var material = await _materialService.GetMaterialByIdAsync(materialId);
            if (material == null || material.CourseId != courseId)
                return NotFound("Material not found or does not belong to this course.");

            return Ok(new
            {
                material.Id,
                material.WeekNumber,
                material.LectureTitle,
                material.LectureDescription,
                material.FilePath
            });
        }

        [HttpGet("courses/{courseId:guid}/materials")]
        public async Task<IActionResult> GetAllMaterialsForCourse(Guid courseId)
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
                return Unauthorized("User ID not found in token!");
            var userId = Guid.Parse(subClaim.Value);

            var professor = await _professorService.GetProfessorByUserIdAsync(userId);
            if (professor == null)
                return NotFound("Professor not found for this user.");

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null || course.ProfessorId != userId)
                return NotFound("Course not found or you are not authorized to access it.");

            var materials = await _materialService.GetMaterialsByCourseIdAsync(courseId);
            return Ok(materials.Select(m => new
            {
                m.Id,
                m.WeekNumber,
                m.LectureTitle,
                m.LectureDescription,
                m.FilePath
            }));
        }

        [HttpPost("courses/{courseId:guid}/quizzes")]
        public async Task<IActionResult> CreateQuiz(Guid courseId, [FromBody] CreateQuizDto quizDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var quiz = await _quizService.CreateQuizAsync(courseId, quizDto);
                return CreatedAtAction(nameof(GetQuiz), new { courseId, quizId = quiz.Id }, new { QuizId = quiz.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("courses/{courseId:guid}/quizzes/{quizId:guid}")]
        public async Task<IActionResult> GetQuiz(Guid courseId, Guid quizId)
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
                return Unauthorized("User ID not found in token!");
            var userId = Guid.Parse(subClaim.Value);

            var professor = await _professorService.GetProfessorByUserIdAsync(userId);
            if (professor == null)
                return NotFound("Professor not found for this user.");

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null || course.ProfessorId != userId)
                return NotFound("Course not found or you are not authorized to access it.");

            var quiz = await _quizService.GetQuizByIdAsync(quizId);
            if (quiz == null || quiz.CourseId != courseId)
                return NotFound("Quiz not found or does not belong to this course.");

            return Ok(new
            {
                quiz.Id,
                quiz.Title,
                quiz.MaxScore,
                quiz.TermId,
                QuestionCount = quiz.Questions?.Count ?? 0
            });
        }

        [HttpGet("courses/{courseId:guid}/quizzes")]
        public async Task<IActionResult> GetAllQuizzesForCourse(Guid courseId)
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
                return Unauthorized("User ID not found in token!");
            var userId = Guid.Parse(subClaim.Value);

            var professor = await _professorService.GetProfessorByUserIdAsync(userId);
            if (professor == null)
                return NotFound("Professor not found for this user.");

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null || course.ProfessorId != userId)
                return NotFound("Course not found or you are not authorized to access it.");

            var quizzes = await _quizService.GetQuizzesByCourseIdAsync(courseId);
            return Ok(quizzes.Select(q => new
            {
                q.Id,
                q.Title,
                q.MaxScore,
                q.TermId,
                QuestionCount = q.Questions?.Count ?? 0
            }));
        }

        [HttpPost("courses/{courseId:guid}/discussions")]
        public async Task<IActionResult> AddDiscussion(Guid courseId, [FromBody] AddDiscussionDto discussionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
                return Unauthorized("User ID not found in token!");
            var userId = Guid.Parse(subClaim.Value);

            try
            {
                var discussion = await _discussionService.CreateDiscussionAsync(courseId, discussionDto, userId);
                return CreatedAtAction(nameof(GetDiscussion), new { courseId, discussionId = discussion.Id }, new { DiscussionId = discussion.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("courses/{courseId:guid}/discussions/{discussionId:guid}")]
        public async Task<IActionResult> GetDiscussion(Guid courseId, Guid discussionId)
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
                return Unauthorized("User ID not found in token!");
            var userId = Guid.Parse(subClaim.Value);

            var professor = await _professorService.GetProfessorByUserIdAsync(userId);
            if (professor == null)
                return NotFound("Professor not found for this user.");

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null || course.ProfessorId != userId)
                return NotFound("Course not found or you are not authorized to access it.");

            var discussion = await _discussionService.GetDiscussionByIdAsync(discussionId);
            if (discussion == null || discussion.CourseId != courseId)
                return NotFound("Discussion not found or does not belong to this course.");

            return Ok(new
            {
                discussion.Id,
                discussion.Message,
                PostedAt = discussion.PostedAt,
                ProfessorName = discussion.Professor?.User?.FullName,
                IsProfessor = discussion.ProfessorId != null
            });
        }

        [HttpGet("courses/{courseId:guid}/discussions")]
        public async Task<IActionResult> GetAllDiscussionsForCourse(Guid courseId)
        {
            var subClaim = User.FindFirst("sub");
            if (subClaim == null)
                return Unauthorized("User ID not found in token!");
            var userId = Guid.Parse(subClaim.Value);

            var professor = await _professorService.GetProfessorByUserIdAsync(userId);
            if (professor == null)
                return NotFound("Professor not found for this user.");

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null || course.ProfessorId != userId)
                return NotFound("Course not found or you are not authorized to access it.");

            var discussions = await _discussionService.GetDiscussionsByCourseIdAsync(courseId);
            return Ok(discussions.Select(d => new
            {
                d.Id,
                d.Message,
                PostedAt = d.PostedAt,
                ProfessorName = d.Professor?.User?.FullName,
                IsProfessor = d.ProfessorId != null
            }));
        }
    }
}