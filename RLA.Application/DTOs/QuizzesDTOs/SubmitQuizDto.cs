using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RLA.Application.DTOs.QuizzesDTOs
{
    public class SubmitQuizDto
    {
        public Guid QuizId { get; set; }
        public List<QuizAnswerRequest> Answers { get; set; } = new List<QuizAnswerRequest>();
    }
}
