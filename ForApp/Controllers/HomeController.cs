﻿using ForApp.Areas.Identity.Data;
using ForApp.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using System.Diagnostics;

namespace ForApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
        }



        public IActionResult Index()
        {
            return View();
        }

        /*public IActionResult SendMail()
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("from_address@example.com"));
            email.To.Add(MailboxAddress.Parse("chungntbgcd201567@fpt.edu.vn"));
            email.Subject = "Test Email Subject"; //tieu de email
            email.Body = new TextPart(TextFormat.Plain) { Text = "Example Plain Text Message Body" };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("alexander.kshlerin14@ethereal.email", "tw5sPv9h7eTCeR64hn");
            smtp.Send(email);
            smtp.Disconnect(true);
            return NoContent();
        }*/
        public async Task<IActionResult> Privacy()
        {
            await _emailSender.SendEmailAsync("chungntbgcd201567@fpt.edu.vn", "test send mail", "just test");
            return View();
        }
        [Authorize(Roles = "Customer")]
        public IActionResult ForCustomerOnly()
        {
            ViewBag.message = "This is for Customer only! Hi " + _userManager.GetUserName(HttpContext.User);
            return View("Views/Home/Index.cshtml");
        }

        [Authorize(Roles = "Seller")]
        public IActionResult ForSellerOnly()
        {
            ViewBag.message = "This is for Store Owner only!";
            return View("Views/Home/Index.cshtml");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}