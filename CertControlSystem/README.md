# CertControlSystem

**System Zarządzania Certyfikatami** – aplikacja internetowa do obsługi pełnego cyklu życia certyfikatów pracowników i klientów.

---

## Opis projektu

**CertControlSystem** to aplikacja webowa umożliwiająca centralne zarządzanie certyfikatami w organizacji. System pozwala na:

* rejestrowanie klientów (pracowników, kontrahentów),
* przypisywanie certyfikatów do klientów,
* monitorowanie dat ważności certyfikatów (np. BHP, SEP),
* automatyczne powiadomienia Email/SMS o zbliżającym się wygaśnięciu,
* prowadzenie dziennika wysłanych powiadomień.

Projekt ma charakter **edukacyjny** i został stworzony w celu nauki ASP.NET Core, EF Core oraz architektury aplikacji webowych.

---

## Technologie

* **Backend:** ASP.NET Core Razor Pages (.NET 10)
* **Baza danych:** SQL Server
* **ORM:** Entity Framework Core (Database-First)
* **Procesy w tle:** `BackgroundService` (`IHostedService`)
* **Powiadomienia:**

  * Email (SMTP)
  * SMS (integracja z zewnętrzną usługą)
* **Frontend:** Razor Pages + Bootstrap 5
* **Autoryzacja:** ASP.NET Core Identity

---

## Architektura katalogów

```
CertControlSystem/
├── Pages/                 # Razor Pages (UI)
├── Models/                # Klasy encji (scaffolded z EF Core)
├── Services/              # Logika biznesowa, serwisy, BackgroundService
├── wwwroot/               # Zasoby statyczne (CSS, JS, Bootstrap)
├── SQL/                   # Skrypty SQL (tworzenie bazy)
├── Program.cs             # Konfiguracja aplikacji
├── appsettings.json       # Konfiguracja środowiska
├── .gitignore
└── README.md
```

---

## Wymagania

* Visual Studio 2022+ **lub** Visual Studio Code
* .NET 10 SDK
* SQL Server (LocalDB lub pełna instancja)

---

## Szybki start

### 1️. Klonowanie repozytorium

```bash
git clone https://github.com/bartoszkoperczak/CertControlSystem.git
cd CertControlSystem
```

### 2️. Konfiguracja bazy danych

1. Otwórz **SQL Server Management Studio**
2. Uruchom skrypt:

```
SQL/CreateDatabase.sql
```

Skrypt utworzy bazę danych **CertDB** wraz z przykładowymi danymi.

### 3️. Przywrócenie pakietów NuGet

```bash
dotnet restore
```

### 4️. Uruchomienie aplikacji

```bash
dotnet run
```

Aplikacja będzie dostępna pod adresem:
👉 `https://localhost:5001`

---

##  Struktura bazy danych

| Tabela             | Opis                                                                |
| ------------------ | ------------------------------------------------------------------- |
| `Clients`          | Klienci / pracownicy (Id, FirstName, LastName, Email, Phone)        |
| `CertificateTypes` | Typy certyfikatów (Id, Name, DefaultValidityMonths)                 |
| `Certificates`     | Certyfikaty (Id, ClientId, TypeId, IssueDate, Expiration, IsActive) |
| `NotificationLogs` | Historia powiadomień (Id, CertificateId, Channel, SentDate, Status) |

---

##  Główne funkcjonalności

* Zarządzanie klientami (CRUD)
* Zarządzanie certyfikatami (CRUD)
* Powiązania certyfikatów z klientami i typami
* Automatyczne powiadomienia o wygasających certyfikatach
* Historia wysłanych powiadomień
* REST API:

  ```
  GET /api/certificates
  ```
* Autoryzacja użytkowników (logowanie, role, ograniczenia dostępu)

---

##  Konfiguracja powiadomień Email

W pliku `appsettings.json` dodaj:

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "twoj-email@gmail.com",
  "SenderPassword": "twoje-haslo-aplikacji",
  "EnableSSL": true
}
```

>  **Uwaga:**
> Dla Gmaila wymagane jest hasło aplikacji (App Password).

---

##  Instalacja pakietów NuGet (przykład)

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package MailKit
```

---

##  Kontakt

* GitHub: [https://github.com/bartoszkoperczak/CertControlSystem](https://github.com/bartoszkoperczak/CertControlSystem)