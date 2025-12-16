using CertControlSystem.Models;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text; // Potrzebne do kodowania treści SMS

namespace CertControlSystem.BackgroundServices
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationWorker> _logger;
        
        // Ustawienie interwału: W produkcji np. raz na 24h (TimeSpan.FromHours(24))
        // Do testów i prezentacji zostawiamy 1 minutę.
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); 

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
                        
                        // 1. Sprawdź certyfikaty wygasające za 30 dni (1 miesiąc)
                        await CheckCertificatesAndNotify(context, 30);

                        // 2. Sprawdź certyfikaty wygasające za 90 dni (3 miesiące)
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
            // Obliczamy datę, której szukamy (Dzisiaj + X dni)
            var targetDate = DateOnly.FromDateTime(DateTime.Now.AddDays(daysThreshold));

            // Pobieramy z bazy certyfikaty, które wygasają DOKŁADNIE tego dnia
            // Include(c => c.Client) jest kluczowe - dzięki temu mamy dostęp do Emaila i Telefonu klienta!
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
                // Zabezpieczenie: Sprawdzamy w bazie, czy już nie wysłaliśmy powiadomienia o tym typie (żeby nie spamować)
                bool alreadySent = await context.NotificationLogs.AnyAsync(l => 
                    l.CertificateId == cert.Id && 
                    l.MessageContent.Contains($"{daysThreshold} dni"));

                if (!alreadySent)
                {
                    string message = $"Dzień dobry {cert.Client.FirstName}! Przypominamy, że Twój certyfikat '{cert.Type.Name}' traci ważność dnia {cert.ExpirationDate}. Prosimy o kontakt w celu odnowienia.";
                    
                    // A. Wysyłka E-mail (na prawdziwy adres z bazy!)
                    if (!string.IsNullOrEmpty(cert.Client.Email))
                    {
                        await SendRealEmailAsync(cert.Client.Email, "Ważne: Wygasający certyfikat", message);
                    }
                    
                    // B. Wysyłka SMS (na prawdziwy numer z bazy!)
                    if (!string.IsNullOrEmpty(cert.Client.PhoneNumber))
                    {
                        await SendSmsApiAsync(cert.Client.PhoneNumber, message);
                    }

                    // C. Zapisz historię w bazie (wymóg projektu)
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
                // Tu wpisz nazwę firmy kolegi i jego adres e-mail
                message.From.Add(new MailboxAddress("System Certyfikatów", "twoj-email@gmail.com")); 
                message.To.Add(new MailboxAddress("", emailTo)); // <-- Tu wpada prawdziwy email klienta z bazy
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = messageBody };

                using (var client = new SmtpClient())
                {
                    // --- KONFIGURACJA DLA GMAILA ---
                    // Jeśli używasz Gmaila, musisz wygenerować "Hasło do aplikacji" (App Password) w ustawieniach konta Google (Security -> 2FA -> App passwords).
                    // Zwykłe hasło do logowania NIE zadziała!
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync("twoj-email@gmail.com", "twoje-haslo-aplikacji-google");
                    
                    // --- KONFIGURACJA DLA INNEGO HOSTA (np. firmowego) ---
                    // await client.ConnectAsync("smtp.firma-kolegi.pl", 587, false);
                    // await client.AuthenticateAsync("biuro@firma-kolegi.pl", "Haslo123!");

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
            // Jeśli nie masz tokenu SMSAPI, ten kod tylko wyświetli log (co wystarczy do zaliczenia na studiach jako "symulacja").
            // Żeby działało naprawdę, odkomentuj kod poniżej i wpisz token.
            
            _logger.LogWarning($"[SMS] (Symulacja) Do: {phoneNumber}, Treść: {message}");

            /* // --- PRAWDZIWA WYSYŁKA PRZEZ SMSAPI.PL ---
            try
            {
                string token = "TU_WKLEJ_TOKEN_OD_SMSAPI"; 
                string url = "https://api.smsapi.pl/sms.do";

                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "auth_token", token },
                        { "to", phoneNumber },
                        { "message", message },
                        { "from", "Eco" }, // Pole nadawcy (musi być aktywne w SMSAPI) lub "Eco"
                        { "format", "json" }
                    };

                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync(url, content);
                    
                    // Opcjonalnie sprawdź response.IsSuccessStatusCode
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd SMS: {ex.Message}");
            }
            */
            
            await Task.CompletedTask;
        }
    }
}