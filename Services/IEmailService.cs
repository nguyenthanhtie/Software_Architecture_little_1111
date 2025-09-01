namespace Final_VS1.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task<bool> SendConfirmationEmailAsync(string toEmail, string confirmationLink);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink);
        Task<bool> SendOrderConfirmationEmailAsync(string toEmail, int orderId, decimal totalAmount);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
        Task<bool> SendOrderStatusUpdateEmailAsync(string toEmail, int orderId, string newStatus);
    }
}
