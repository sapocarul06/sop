# Proiect: Gestionare Plan Alimentar

## Descriere
Aplicație console C# .NET Framework 4.7.2 pentru gestionarea planurilor alimentare cu import de date de la mai mulți provideri, generare de rapoarte și notificări.

## Cerințe îndeplinite

### ✅ Scripturi Import
- **ImportInitial.cs** - Rulează o singură dată, importă date de la 3 provideri (API, DB locală, Scraping)
- **ImportZilnic.cs** - Rulează zilnic prin Task Scheduler (cron Windows), actualizează datele

### ✅ Baza de Date
- **ScriptCreareBazaDate.sql** - Script SQL Server (SSMS) pentru crearea tabelelor
- Tabele: Utilizatori, Alimente, PlanuriAlimentare, Anunturi, LogNotificari

### ✅ Funcționalități Principale
- Introducere date utilizator (greutate, înălțime, vârstă, sex, greutate țintă)
- Calcul necesar caloric (Formula Mifflin-St Jeor)
- Generare plan alimentar săptămânal personalizat
- Alimente diferite în fiecare zi

### ✅ Detectare Duplicate
- **DetectareDuplicate.cs** - Identifică anunțuri identice pe baza hash-ului imaginilor
- Algoritm MD5/SHA256 pentru comparare

### ✅ Studii de Caz
- **StudiiCaz.cs** - Analiză detaliată:
  - Stocare imagini: Server vs Bază de date (recomandare: hibrid)
  - Găzduire: Server tradițional vs Serverless (recomandare: VM pentru .NET 4.7.2)

### ✅ Generare Rapoarte
- **GenerareRapoarte.cs** - Export PDF (text) și Excel (CSV)

### ✅ Servicii Asincrone
- **ServiciiEmail.cs** - Trimitere emailuri promoționale async către toți utilizatorii

### ✅ Notificări Paralele
- **NotificariParalele.cs** - Fire de execuție paralele (Task Parallel Library) pentru verificarea anunțurilor noi

### ✅ Statistici
- Calcule consumatoare de timp cu măsurare performanță
- Rapoarte despre utilizatori, alimente și planuri generate

## Structură Proiect

```
PlanAlimentarApp/
├── Program.cs              # Aplicația principală cu meniu
├── ImportInitial.cs        # Import inițial (o singură dată)
├── ImportZilnic.cs         # Import zilnic (cron daily)
├── DetectareDuplicate.cs   # Detectare duplicate imagini
├── GenerareRapoarte.cs     # Export PDF/Excel
├── ServiciiEmail.cs        # Email asincron
├── NotificariParalele.cs   # Notificări parallele
├── StudiiCaz.cs            # Studii de caz arhitectură
└── PlanAlimentarApp.csproj # Fișier proiect .NET 4.7.2

ScriptCreareBazaDate.sql    # Script SQL Server
```

## Configurare

### 1. Creare Bază de Date
```sql
-- Deschideți SQL Server Management Studio (SSMS)
-- Conectați-vă la serverul local sau remote
-- Deschideți și rulați scriptul: ScriptCreareBazaDate.sql
```

### 2. Configurare Conexiune
În `Program.cs`, modificați string-ul de conexiune:
```csharp
private static string connectionString = "Server=localhost;Database=PlanAlimentarDB;Integrated Security=true;";
```

### 3. Compilare și Rulare
```bash
# Din Visual Studio Developer Command Prompt
cd PlanAlimentarApp
msbuild PlanAlimentarApp.csproj
bin\Debug\PlanAlimentarApp.exe
```

### 4. Configurare Task Scheduler (Import Zilnic)
1. Deschideți **Task Scheduler** în Windows
2. Create Basic Task → "Import Zilnic Plan Alimentar"
3. Trigger: Daily, ora 02:00
4. Action: Start a program
   - Program: `C:\Path\To\PlanAlimentarApp.exe`
   - Arguments: `--daily-import`
5. Finish

## Utilizare

### Meniu Principal
```
1. Import inițial date (rulează o singură dată)
2. Import zilnic actualizare date (cron daily)
3. Generează plan alimentar personalizat
4. Detectare anunțuri duplicate (imagini)
5. Generare rapoarte PDF/Excel
6. Trimitere emailuri promoționale
7. Notificări paralele pentru anunțuri
8. Studii de caz (stocare imagini, server vs serverless)
9. Statistici și consum timp
0. Ieșire
```

### Exemplu Generare Plan
```
Introduceți numele: Ion Popescu
Greutate actuală (kg): 85
Înălțime (cm): 175
Vârsta (ani): 35
Sex (M/F): M
Greutate țintă (kg): 75

→ Necesar caloric zilnic: 2400 kcal
→ Se generează plan alimentar pentru 7 zile...
→ Plan generat cu succes!
```

## Tehnologii Folosite
- **Limbaj**: C# .NET Framework 4.7.2
- **Bază de date**: Microsoft SQL Server
- **Scraping**: Selenium WebDriver (simulat în demo)
- **PDF**: iTextSharp/iText7 (opțional, demo folosește text)
- **Excel**: CSV format (compatibil Excel)
- **Parallelism**: Task Parallel Library (TPL)

## Arhitectură Recomandată
```
Azure VM (B2S) ──> Azure SQL Database ──> Azure Blob Storage
     │                    │                      │
  App .NET             Date                   Imagini
  Task Scheduler       Backup                 Rapoarte
```

## Note Importante
- Pentru producție, instalați pachetele NuGet: `iText7`, `EPPlus` (Excel real)
- Configurați SMTP valid pentru trimitere emailuri reale
- Actualizați calea imaginilor în funcție de mediu
- Pentru scraping real, adăugați Selenium.WebDriver prin NuGet
