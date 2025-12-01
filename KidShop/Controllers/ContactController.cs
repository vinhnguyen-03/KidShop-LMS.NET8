using KidShop.Models;
using KidShop.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration; // Add this at the top

namespace KidShop.Controllers
{
    public class ContactController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration; // Add this field

        public ContactController(DataContext context, IConfiguration configuration) // Inject IConfiguration
        {
            _context = context;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        // POST: Contact/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(ContactVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Use _configuration instead of ConfigurationManager.AppSettings
                    string emailUsername = _configuration["EmailConfig:EmailUsername"];
                    string emailPassword = _configuration["EmailConfig:EmailPassword"];
                    string adminEmail = _configuration["EmailConfig:AdminEmail"];

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress(emailUsername); // Gửi từ email hệ thống
                        mail.ReplyToList.Add(new MailAddress(model.Email)); // Người nhận có thể reply lại người dùng
                        mail.To.Add(adminEmail);                           // Admin nhận thư
                        mail.Subject = "Liên hệ từ người dùng";
                        mail.IsBodyHtml = true;
                        mail.Body = $@"
                    <p><strong>Họ tên:</strong> {model.YourName}</p>
                    <p><strong>Email:</strong> {model.Email}</p>     
                    <p><strong>Nội dung:</strong></p>
                    <p>{model.Message}</p>
                ";
                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new NetworkCredential(emailUsername, emailPassword);
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }

                    TempData["Success"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Gửi liên hệ thất bại. Vui lòng thử lại sau.";
                    // Log lỗi nếu cần
                    Console.WriteLine(ex);
                }

                return RedirectToAction("Index");
            }

            return View("Index", model);
        }
    }
}
