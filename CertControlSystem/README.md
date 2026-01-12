# CertControlSystem - System Zarządzania Certyfikatami i Uprawnieniami

**CertControlSystem** to nowoczesna aplikacja webowa typu MVC służąca do ewidencji klientów oraz monitorowania ważności ich certyfikatów i uprawnień (np. BHP, SEP). System automatycznie dba o to, by żaden termin nie został przeoczony, wysyłając powiadomienia e-mail oraz SMS.

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
* Automatyczna wysyłka powiadomień **E-mail (SMTP)** na 30 i 90 dni przed upływem ważności.

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

## ⚙️ Instrukcja Uruchomienia
# Wymagania wstępne
Visual Studio 2022 lub VS Code

.NET SDK 8.0

SQL Server (LocalDB lub pełna instancja)

# Krok 1: Klonowanie repozytorium
Bash

git clone [https://github.com/TwojLogin/CertControlSystem.git](https://github.com/TwojLogin/CertControlSystem.git)
cd CertControlSystem

# Krok 2: Konfiguracja Bazy Danych
W pliku appsettings.json upewnij się, że ConnectionStrings:CertDbContext wskazuje na Twój serwer SQL.

Metoda A: Entity Framework (Zalecana) Otwórz konsolę Package Manager Console i wpisz:

**Update-Database**

Metoda B: Skrypt SQL Jeśli wolisz utworzyć bazę ręcznie, w katalogu /Database znajduje się plik script.sql. Uruchom go w SQL Server Management Studio (SSMS).

# Krok 3: Konfiguracja SMTP (E-mail)
Aby system wysyłał prawdziwe maile, w plikach NotificationWorker.cs oraz CertificatesController.cs (metoda SendNotification) uzupełnij:

Email nadawcy

Hasło aplikacji (App Password) - dla Gmaila wymagane włączenie 2FA.

# Krok 4: Uruchomienie

**dotnet run**

Aplikacja dostępna będzie pod adresem: https://localhost:7083 (lub podobnym).

## 📧 API Endpointy
Pobranie wszystkich certyfikatów (JSON): GET /api/certificatesapi

## 👤 Autor
Bartosz Koperczak Projekt zaliczeniowy: Programowanie w ASP.NET / Programowanie Sieciowe