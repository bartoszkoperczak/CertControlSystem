using CertControlSystem.Models;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;

namespace CertControlSystem.BackgroundServices
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationWorker> _logger;

        //do testów TimeSpan.FromMinutes(1);
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); 

        public NotificationWorker(IServiceProvider serviceProvider, ILogger<NotificationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SYSTEM POWIADOMIEŃ: Uruchomiono usługę w tle.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<CertDbContext>();
                        
                        //certyfikaty wygasające za 30 dni
                        await CheckCertificatesAndNotify(context, 30);

                        //certyfikaty wygasające za 90 dni
                        await CheckCertificatesAndNotify(context, 90);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Błąd krytyczny w usłudze powiadomień.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckCertificatesAndNotify(CertDbContext context, int daysThreshold)
        {
            var targetDate = DateOnly.FromDateTime(DateTime.Now.AddDays(daysThreshold));

            var certificates = await context.Certificates
                .Include(c => c.Client)
                .Include(c => c.Type)
                .Where(c => c.IsActive && c.ExpirationDate == targetDate)
                .ToListAsync();

            if (certificates.Any())
            {
                _logger.LogInformation($"Znaleziono {certificates.Count} certyfikatów wygasających za {daysThreshold} dni.");
            }

            foreach (var cert in certificates)
            {
                bool alreadySent = await context.NotificationLogs.AnyAsync(l => 
                    l.CertificateId == cert.Id && 
                    l.MessageContent.Contains($"{daysThreshold} dni"));

                if (!alreadySent)
                {
                    string message = $"Dzien dobry {cert.Client.FirstName}! Przypominamy, że Twoj certyfikat '{cert.Type.Name}' traci waznosc dnia {cert.ExpirationDate}. Prosimy o kontakt w celu odnowienia.";
                    
                    if (!string.IsNullOrEmpty(cert.Client.Email))
                    {
                        await SendRealEmailAsync(cert.Client.Email, "Ważne: Wygasający certyfikat", message);
                    }
                    
                    if (!string.IsNullOrEmpty(cert.Client.PhoneNumber))
                    {
                        await SendSmsApiAsync(cert.Client.PhoneNumber, message);
                    }

                    context.NotificationLogs.Add(new NotificationLog
                    {
                        CertificateId = cert.Id,
                        Channel = "Email/SMS",
                        SentDate = DateTime.Now,
                        MessageContent = $"Przypomnienie {daysThreshold} dni",
                        Status = "Wysłano"
                    });
                }
            }
            
            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
            }
        }

        private async Task SendRealEmailAsync(string emailTo, string subject, string messageBody)
        {
            try 
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("CertControlSystem", "koperczakbartosz@gmail.com")); 
                message.To.Add(new MailboxAddress("", emailTo));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = messageBody };

                using (var client = new SmtpClient())
                {
                    //dla Gmaila : (Security -> 2FA -> App passwords)
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync("koperczakbartosz@gmail.com", "xxxx");
                    
                    //dla innego hosta
                    // await client.ConnectAsync("smtp.mail.pl", 587, false);
                    // await client.AuthenticateAsync("mail", "haslo");

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                _logger.LogInformation($"[EMAIL] Wysłano do: {emailTo}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd wysyłki email do {emailTo}: {ex.Message}");
            }
        }

        private async Task SendSmsApiAsync(string phoneNumber, string message)
        {
            try
            {
                string token = "";
                string url = "https://api.smsapi.pl/sms.do";

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "access_token", token },
                        { "to", phoneNumber },
                        { "message", message },
                        { "format", "json" }
                    };

                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"Błąd SMSAPI: {error}");
                    }
                    else
                    {
                        _logger.LogInformation($"[SMSAPI] Wysłano SMS do: {phoneNumber}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd SMS: {ex.Message}");
            }
            
            await Task.CompletedTask;
        }
    }
}