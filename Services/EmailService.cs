using Final_VS1.Helper;
using Final_VS1.Services;

namespace Final_VS1.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;

        public EmailService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(subject))
                return false;

            try
            {
                await _emailSender.SenderEmailAsync(toEmail, subject, htmlBody);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendConfirmationEmailAsync(string toEmail, string confirmationLink)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(confirmationLink))
                return false;

            var subject = "Xác nhận đăng ký tài khoản - Little Fish Beauty";
            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #333; text-align: center;'>Chào mừng bạn đến với Little Fish Beauty!</h2>
                    
                    <p>Cảm ơn bạn đã đăng ký tài khoản tại Little Fish Beauty.</p>
                    
                    <p>Để hoàn tất quá trình đăng ký, vui lòng nhấn vào nút xác nhận bên dưới:</p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{confirmationLink}' 
                           style='background-color: #007bff; color: white; padding: 12px 30px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Xác nhận tài khoản
                        </a>
                    </div>
                    
                    <p style='color: #666; font-size: 14px;'>
                        Nếu nút không hoạt động, bạn có thể copy và paste đường link sau vào trình duyệt:
                        <br>
                        <a href='{confirmationLink}'>{confirmationLink}</a>
                    </p>
                    
                    <p style='color: #666; font-size: 14px;'>
                        Link này sẽ hết hạn sau 24 giờ.
                    </p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px; text-align: center;'>
                        Email này được gửi từ hệ thống Little Fish Beauty.
                    </p>
                </div>";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(resetLink))
                return false;

            var subject = "Yêu cầu đặt lại mật khẩu - Little Fish Beauty";
            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #333; text-align: center;'>Đặt lại mật khẩu</h2>
                    
                    <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                    
                    <p>Để đặt lại mật khẩu, vui lòng nhấn vào nút bên dưới:</p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' 
                           style='background-color: #dc3545; color: white; padding: 12px 30px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Đặt lại mật khẩu
                        </a>
                    </div>
                    
                    <p style='color: #666; font-size: 14px;'>
                        Nếu nút không hoạt động, bạn có thể copy và paste đường link sau vào trình duyệt:
                        <br>
                        <a href='{resetLink}'>{resetLink}</a>
                    </p>
                    
                    <p style='color: #666; font-size: 14px;'>
                        Link này sẽ hết hạn sau 1 giờ.
                    </p>
                    
                    <p style='color: #666; font-size: 14px;'>
                        Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.
                    </p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px; text-align: center;'>
                        Email này được gửi từ hệ thống Little Fish Beauty.
                    </p>
                </div>";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task<bool> SendOrderConfirmationEmailAsync(string toEmail, int orderId, decimal totalAmount)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return false;

            var subject = $"Xác nhận đơn hàng #{orderId} - Little Fish Beauty";
            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #333; text-align: center;'>Xác nhận đơn hàng</h2>
                    
                    <p>Cảm ơn bạn đã đặt hàng tại Little Fish Beauty!</p>
                    
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                        <h3 style='margin-top: 0;'>Thông tin đơn hàng</h3>
                        <p><strong>Mã đơn hàng:</strong> #{orderId}</p>
                        <p><strong>Tổng tiền:</strong> {totalAmount:N0} ₫</p>
                        <p><strong>Thời gian đặt:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                        <p><strong>Trạng thái:</strong> Chờ xác nhận</p>
                    </div>
                    
                    <p>Đơn hàng của bạn đang được xử lý. Chúng tôi sẽ liên hệ với bạn sớm nhất để xác nhận thông tin giao hàng.</p>
                    
                    <p>Bạn có thể theo dõi trạng thái đơn hàng bằng cách đăng nhập vào tài khoản của mình trên website.</p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px; text-align: center;'>
                        Cảm ơn bạn đã tin tưởng Little Fish Beauty!
                    </p>
                </div>";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(userName))
                return false;

            var subject = "Chào mừng bạn đến với Little Fish Beauty!";
            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #333; text-align: center;'>Chào mừng {userName}!</h2>
                    
                    <p>Tài khoản của bạn đã được kích hoạt thành công!</p>
                    
                    <p>Bạn có thể bắt đầu khám phá các sản phẩm làm đẹp tuyệt vời của chúng tôi và tận hưởng trải nghiệm mua sắm tốt nhất.</p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='#' 
                           style='background-color: #28a745; color: white; padding: 12px 30px; 
                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Bắt đầu mua sắm
                        </a>
                    </div>
                    
                    <p>Nếu bạn có bất kỳ câu hỏi nào, đừng ngần ngại liên hệ với chúng tôi.</p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px; text-align: center;'>
                        Cảm ơn bạn đã gia nhập Little Fish Beauty!
                    </p>
                </div>";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task<bool> SendOrderStatusUpdateEmailAsync(string toEmail, int orderId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(newStatus))
                return false;

            var subject = $"Cập nhật trạng thái đơn hàng #{orderId} - Little Fish Beauty";
            var statusMessage = GetStatusMessage(newStatus);
            
            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #333; text-align: center;'>Cập nhật đơn hàng</h2>
                    
                    <p>Đơn hàng #{orderId} của bạn đã được cập nhật trạng thái.</p>
                    
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                        <p><strong>Trạng thái hiện tại:</strong> <span style='color: #007bff;'>{newStatus}</span></p>
                        <p><strong>Thời gian cập nhật:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                    </div>
                    
                    <p>{statusMessage}</p>
                    
                    <p>Cảm ơn bạn đã tin tưởng Little Fish Beauty!</p>
                    
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='color: #999; font-size: 12px; text-align: center;'>
                        Email này được gửi tự động từ hệ thống Little Fish Beauty.
                    </p>
                </div>";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }

        private string GetStatusMessage(string status)
        {
            return status switch
            {
                "Đã xác nhận" => "Đơn hàng của bạn đã được xác nhận và đang được chuẩn bị.",
                "Đang xử lý" => "Đơn hàng của bạn đang được đóng gói và chuẩn bị giao hàng.",
                "Đang giao" => "Đơn hàng của bạn đang trên đường giao đến địa chỉ của bạn.",
                "Hoàn thành" => "Đơn hàng của bạn đã được giao thành công. Cảm ơn bạn đã mua hàng!",
                "Đã hủy" => "Đơn hàng của bạn đã bị hủy. Nếu có thắc mắc, vui lòng liên hệ với chúng tôi.",
                _ => "Trạng thái đơn hàng của bạn đã được cập nhật."
            };
        }
    }
}
