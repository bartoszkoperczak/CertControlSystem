# CertControlSystem - System Zarządzania Certyfikatami i Uprawnieniami

**CertControlSystem** to nowoczesna aplikacja webowa typu MVC służąca do ewidencji klientów oraz monitorowania ważności ich certyfikatów i uprawnień (np. BHP, SEP). System automatycznie dba o to, by żaden termin nie został przeoczony, wysyłając powiadomienia e-mail i sms.

## 🚀 Główne Funkcjonalności

### 1. Zarządzanie Klientami i Certyfikatami (CRUD)
* Pełna obsługa bazy klientów (Dodawanie, Edycja, Usuwanie, Szczegóły).
* Przypisywanie certyfikatów do klientów z wyborem typu szkolenia.
* **Inteligentne formularze:** Automatyczne wyliczanie daty ważności na podstawie wybranego typu szkolenia.

### 2. Monitoring i Statusy
* Wizualna prezentacja statusu certyfikatu (Aktywny/Wygasł/Wygasa wkrótce).
* Kolorystyczne oznaczanie rekordów wymagających uwagi.
* Pasek postępu (Progress Bar) w szczegółach certyfikatu.

### 3. Automatyzacja (Worker Service)
* Działająca w tle usługa **Windows Service / Linux Daemon**.
* Codzienne sprawdzanie bazy danych pod kątem wygasających uprawnień.
* Automatyczna wysyłka powiadomień **E-mail (SMTP)** i **SMS (smsapi.pl) na 30 i 90 dni przed upływem ważności.

### 4. Bezpieczeństwo i Dostęp
* System logowania i rejestracji oparty o **ASP.NET Core Identity**.
* Podział uprawnień:
    * **Gość:** Tylko podgląd danych.
    * **Zalogowany Użytkownik:** Pełna edycja, dodawanie i usuwanie danych oraz ręczne wysyłanie powiadomień.

### 5. API
* Wystawiony endpoint REST API (`/api/certificatesapi`) zwracający listę certyfikatów w formacie JSON (dla zewnętrznych integracji).

---

## 🛠️ Stack Technologiczny

* **Framework:** .NET 8 (ASP.NET Core MVC)
* **Baza danych:** Microsoft SQL Server (LocalDB / Production)
* **ORM:** Entity Framework Core
* **Frontend:** Razor Views, Bootstrap 5, JavaScript
* **Komunikacja E-mail:** MailKit + MimeKit (Obsługa SMTP/Gmail)
* **Komunikacja SMS:** SMSAPI.pl (REST API)
* **Zadania w tle:** Hosted Services (`IHostedService`)

---

## 📂 Struktura Projektu

```text
CertControlSystem/
├── BackgroundServices/  # Logika automatycznych powiadomień (Worker)
├── Controllers/         # Kontrolery MVC oraz API
├── Models/              # Encje bazy danych i DTO
├── Views/               # Widoki Razor (UI aplikacji)
│   ├── Certificates/    # Widoki certyfikatów
│   ├── Clients/         # Widoki klientów
│   └── Shared/          # Layout i partiale (menu, logowanie)
├── wwwroot/             # Pliki statyczne (CSS, JS, Obrazki)
├── Program.cs           # Konfiguracja Dependency Injection i Pipeline'u
└── appsettings.json     # Konfiguracja bazy danych i logowania
```

---

## 📖 Dokumentacja

### 1. Wymagania

- Visual Studio 2022 lub VS Code
- .NET SDK 8.0
- SQL Server (LocalDB lub pełna instancja)
- Konto e-mail (np. Gmail) do wysyłki powiadomień
- Konto na smsapi.pl do wysyłki SMSów (opcjonalnie)

### 2. Instalacja

#### Krok 1: Klonowanie repozytorium

git clone [https://github.com/bartoszkoperczak/CertControlSystem.git]
cd CertControlSystem

#### Krok 2: Konfiguracja bazy danych

W pliku `appsettings.json` ustaw odpowiedni connection string w sekcji `ConnectionStrings:CertDbContext`, np.: "ConnectionStrings": { "CertDbContext": "Server=(localdb)\MSSQLLocalDB;Database=CertDB;Trusted_Connection=True;" }

##### Metoda A: Entity Framework (zalecana)

W konsoli Package Manager Console: Update-Database

##### Metoda B: Skrypt SQL

W katalogu `/Database` znajduje się plik `script.sql`. Uruchom go w SQL Server Management Studio (SSMS).

#### Krok 3: Konfiguracja SMTP (E-mail) && Konfiguracja SMS Api

Aby system wysyłał prawdziwe maile, w plikach `NotificationWorker.cs` oraz `CertificatesController.cs` (metoda `SendNotification`) uzupełnij:

- Adres e-mail nadawcy (np. `test@gmail.com`)
- Hasło aplikacji (App Password) – dla Gmaila wymagane włączenie 2FA i wygenerowanie hasła aplikacji

Analogicznie, jeśli chcesz wysyłać SMSy, uzupełnij token z smsapi.pl w plikach `NotificationWorker.cs` oraz `CertificatesController.cs` w sekcji SMS Api.

#### Krok 4: Uruchomienie aplikacji

dotnet run

Aplikacja będzie dostępna pod adresem: `https://localhost:7083` (lub innym wskazanym w konsoli).

---

### 3. Testowi użytkownicy i hasła

Po pierwszym uruchomieniu możesz zarejestrować użytkownika przez formularz rejestracji.  
Aby dodać domyślnego użytkownika (np. `Admin`), możesz dodać kod seedujący w pliku `Program.cs` lub ręcznie utworzyć konto przez UI.

**Przykładowy użytkownik:**
- Login: `admin@certcontrol.local`
- Hasło: `Test123!`  
(Uwaga: Jeśli nie istnieje, zarejestruj ręcznie.)

---

### 4. Konfiguracja

- **Łańcuch połączenia z bazą:**  
  Plik: `appsettings.json`, klucz: `ConnectionStrings:CertDbContext`
- **SMTP:**  
  Pliki: `NotificationWorker.cs`, `CertificatesController.cs` – uzupełnij dane logowania do SMTP
- **Port aplikacji:**  
  Domyślnie 7083 (możesz zmienić w `launchSettings.json`)

---

### 5. Opis działania aplikacji z punktu widzenia użytkownika

- **Gość** (niezalogowany):  
  Może przeglądać listę certyfikatów i klientów, ale nie może edytować ani dodawać danych.
- **Zalogowany użytkownik:**  
  Ma pełny dostęp do funkcji CRUD (dodawanie, edycja, usuwanie klientów i certyfikatów), może ręcznie wysyłać powiadomienia e-mail.
- **Automatyczne powiadomienia:**  
  System codziennie sprawdza certyfikaty wygasające za 30 i 90 dni i wysyła powiadomienia e-mail/SMS do klientów.
- **API:**  
  Dostępny jest endpoint `/api/certificatesapi`, który zwraca listę certyfikatów w formacie JSON – można go użyć do integracji z zewnętrznymi systemami lub serwisami powiadomień.

---

## 📧 API Endpointy

- Pełna obsługa API CRUD (JSON):  
  `GET /api/certificatesapi`
- `POST /api/certificatesapi`
  `DELETE /api/certificatesapi`
- `POST /api/certificatesapi`
---

## 👤 Autor

Bartosz Koperczak, Mateusz Porzycki, Krystian Pyś
```
