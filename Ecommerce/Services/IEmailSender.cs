using System.Threading.Tasks;

namespace Ecommerce.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}