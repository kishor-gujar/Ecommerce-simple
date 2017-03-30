using System.Threading.Tasks;

namespace Ecommerce.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}