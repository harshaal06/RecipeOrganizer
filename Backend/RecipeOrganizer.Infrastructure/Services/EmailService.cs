using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using RecipeOrganizer.Domain.Entity;
using RecipeOrganizer.Domain.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendOTPAsync(string email, string userName, string otp)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("RecipeOrganizer", "hemantagl206@gmail.com")); 

        message.To.Add(MailboxAddress.Parse(email));

        message.Subject = "Email Verification OTP";

        message.Body = new TextPart("html")
        {
            Text = $@"
<h2>Hello {userName},</h2>

<p>Welcome to RecipeOrganizer.</p>

<p>Your verification OTP is:</p>

<h1 style='letter-spacing:5px;color:#ff5722;'>{otp}</h1>

<p>This OTP will expire in <b>10 minutes</b>.</p>

<p>Please do not share this OTP with anyone.</p>"
        };

        // Send using MailKit
        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }
}