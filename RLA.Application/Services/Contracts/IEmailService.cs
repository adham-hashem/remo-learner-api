using System.Threading.Tasks;

namespace RLA.Application.Services.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}