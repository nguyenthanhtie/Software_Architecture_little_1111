namespace Final_VS1.Helper
{
    public interface IEmailSender
    {
        Task SenderEmailAsync(string toEmail, string subject, string body);
    }
}
