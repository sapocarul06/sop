using System;

namespace PlanAlimentarApp
{
    /// <summary>
    /// Studii de caz privind arhitectura aplicației
    /// </summary>
    public class StudiiCaz
    {
        /// <summary>
        /// Studiu de caz: Modalitatea de stocare a imaginilor (server vs baza de date)
        /// </summary>
        public void PreZintaStudiuStocareImagini()
        {
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════════╗
║  STUDIU DE CAZ: STOCAREA IMAGINILOR - SERVER FILE SYSTEM vs BAZA DE DATE     ║
╠══════════════════════════════════════════════════════════════════════════════╣

1. STOCARE ÎN BAZA DE DATE (BLOB - Binary Large Object)
   ──────────────────────────────────────────────────────
   
   Avantaje:
   • Tranșacționalitate garantată (ACID) - imaginea și metadata sunt salvate atomic
   • Backup simplificat - o singură operație pentru toate datele
   • Securitate centralizată - permisiunile bazei de date se aplică și imaginilor
   • Consistență ridicată - nu există imagini 'orfane' fără înregistrare
   
   Dezavantaje:
   • Performanță scăzută la citire/scriere pentru fișiere mari
   • Creștere rapidă a dimensiunii bazei de date
   • Backup-urile devin foarte mari și lente
   • Consum mare de memorie RAM la interogări
   • Costuri mai mari pentru storage premium în cloud
   
   Implementare SQL Server:
   ```sql
   CREATE TABLE Imagini (
       Id INT PRIMARY KEY,
       Nume NVARCHAR(200),
       DateImagine VARBINARY(MAX), -- Stocare directă în DB
       DataUpload DATETIME
   );
   ```

2. STOCARE PE SERVER FILE SYSTEM
   ───────────────────────────────
   
   Avantaje:
   • Performanță superioară la citire (direct file I/O)
   • Baza de date rămâne mică și rapidă
   • Backup diferențiat posibil (DB separat de fișiere)
   • Costuri de storage mai mici
   • Possibilitatea de a folosi CDN pentru livrare rapidă
   
   Dezavantaje:
   • Gestionarea consistenței (fișiere orfane)
   • Backup necesită sincronizare între DB și file system
   • Securitate separată de gestionat
   • Dificultăți în medii distributed/cloud
   
   Implementare recomandată:
   ```csharp
   // În baza de date se stochează doar calea
   CREATE TABLE Alimente (
       Id INT PRIMARY KEY,
       Nume NVARCHAR(200),
       CaleImagine NVARCHAR(500), -- Doar path-ul
       ImagineHash NVARCHAR(64)   -- Hash pentru duplicate
   );
   
   // Fișierele stocate pe disk: /images/alimente/mere.jpg
   ```

3. RECOMANDARE PENTRU APLICAȚIA PLAN ALIMENTAR:
   ────────────────────────────────────────────
   
   ✓ SOLUȚIE HIDRIDĂ OPTIMĂ:
   
   a) Imagini mici (< 100KB) → Hash-uri în DB, fișiere pe server
   b) Folosire hash (MD5/SHA256) pentru detectarea duplicatelor
   c) CDN sau Azure Blob Storage pentru scalabilitate
   d) Cache Redis pentru imagini frecvent accesate
   
   Structura aleasă în acest proiect:
   • Tabelul Alimente conține: ImagineHash + CaleImagine
   • Fișierele fizice: ~/images/{nume_aliment}.jpg
   • Detectare duplicate prin comparare hash-uri

4. COMPARAȚIE PERFORMANȚĂ (estimativ):
   ────────────────────────────────────
   
   | Operațiune          | DB Storage | File System | Hybrid |
   |---------------------|------------|-------------|--------|
   | Citire imagine      | 150ms      | 20ms        | 25ms   |
   | Scriere imagine     | 200ms      | 30ms        | 35ms   |
   | Backup zilnic       | 5GB/5min   | 100MB/30s   | Mixt   |
   | Detectare duplicate | Lent       | Rapid       | Optim  |

╚══════════════════════════════════════════════════════════════════════════════╝
");
        }

        /// <summary>
        /// Studiu de caz: Găzduirea aplicației (Server tradițional vs Serverless)
        /// </summary>
        public void PreZintaStudiuServerVsServerless()
        {
            Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════════════════════╗
║  STUDIU DE CAZ: ARHITECTURĂ - SERVER TRADIȚIONAL vs SERVERLESS               ║
╠══════════════════════════════════════════════════════════════════════════════╣

1. ARHITECTURĂ CU SERVER TRADIȚIONAL (VM/VPS)
   ───────────────────────────────────────────
   
   Exemplu: Azure VM, AWS EC2, VPS local
   
   Avantaje:
   • Control complet asupra mediului și configurației
   • Cost predictibil (abonament lunar fix)
   • Compatibilitate maximă (.NET Framework 4.7.2 rulează nativ)
   • Latență mică pentru operațiuni locale
   • Debugging și monitoring ușor de implementat
   
   Dezavantaje:
   • Scalabilitate limitată (necesită load balancer manual)
   • Overhead de mentenanță (patching, updates, security)
   • Plătești și când nu ai trafic (resurse idle)
   • Disaster Recovery complex de configurat
   • Timp de provisioning lent (minute/ore)
   
   Cost estimat (Azure):
   • B1S VM (1 vCPU, 1GB RAM): ~$13/lună
   • SQL Database: ~$5/lună (Basic tier)
   • Storage: ~$2/lună
   • TOTAL: ~$20/lună (indiferent de utilizare)

2. ARHITECTURĂ SERVERLESS (Azure Functions/AWS Lambda)
   ────────────────────────────────────────────────────
   
   Avantaje:
   • Scalabilitate automată și instantanee
   • Pay-per-execution (plătești doar când rulează)
   • Zero mentenanță infrastructură
   • High availability built-in
   • Integrare nativă cu alte servicii cloud
   
   Dezavantaje:
   • Cold starts (latență inițială 2-5 secunde)
   • .NET Framework 4.7.2 NU este suportat (doar .NET Core/.NET 5+)
   • Limitări de execuție (max 10 minute per function)
   • Debugging mai complex
   • Vendor lock-in accentuat
   • Cost imprevizibil la trafic variabil
   
   Cost estimat (Azure Functions):
   • 1M executions/lună: ~$0.20
   • SQL Database: ~$5/lună
   • Storage: ~$2/lună
   • TOTAL: ~$7-50/lună (în funcție de trafic)

3. COMPARAȚIE DIRECTĂ:
   ────────────────────
   
   | Criteriu              | Server Tradițional | Serverless      |
   |-----------------------|--------------------|------------------|
   | Cost (trafic mic)     | $20/lună (fix)     | $7-15/lună      |
   | Cost (trafic mare)    | $20/lună (fix)     | $50-200+/lună   |
   | Scalabilitate         | Manuală            | Automatizată    |
   | Mentenanță            | Ridicată           | Minimă          |
   | Cold start            | Nu                 | Da (2-5s)       |
   | .NET Framework 4.7.2  | ✓ DA               | ✗ NU            |
   | Time-to-market        | Mediu              | Rapid           |

4. RECOMANDARE PENTRU APLICAȚIA PLAN ALIMENTAR:
   ────────────────────────────────────────────
   
   ✓ ALEGERE OPTIMĂ: SERVER TRADIȚIONAL (din motive tehnice)
   
   Motivare:
   • Aplicația folosește .NET Framework 4.7.2 (cerință specificată)
   • Serverless necesită migrare la .NET 6/8 (efort semnificativ)
   • Trafic estimat: moderat și predictibil
   • Importurile zilnice necesită execuții lungi (>10 min posibil)
   • Selenium WebDriver nu rulează bine în serverless
   
   Arhitectură recomandată:
   ```
   ┌─────────────────────────────────────────┐
   │  Azure VM (B2S - 2 vCPU, 4GB RAM)       │
   │  - Aplicație Console .NET 4.7.2         │
   │  - Task Scheduler pentru import zilnic  │
   │  - IIS (opțional pentru web API)        │
   └─────────────────────────────────────────┘
           │
           ▼
   ┌─────────────────────────────────────────┐
   │  Azure SQL Database (Standard S0)       │
   │  - PlanAlimentarDB                      │
   │  - Backup automat zilnic                │
   └─────────────────────────────────────────┘
           │
           ▼
   ┌─────────────────────────────────────────┐
   │  Azure Blob Storage                     │
   │  - Imagini alimente                     │
   │  - Rapoarte PDF/Excel generate          │
   └─────────────────────────────────────────┘
   ```

5. CONCLUZIE FINALĂ:
   ──────────────────
   
   Pentru acest proiect specific:
   → Server Tradițional (VM) este alegerea corectă
   
   Motive principale:
   1. Compatibilitate .NET Framework 4.7.2
   2. Execuții lungi pentru importuri și scraping
   3. Selenium WebDriver necesită mediu complet
   4. Cost predictibil și control total
   
   Serverless ar fi opțiunea ideală DOAR DACĂ:
   - S-ar migra la .NET 8
   - S-ar refactoriza în functions mici
   - Traficul ar fi foarte variabil/imprevizibil

╚══════════════════════════════════════════════════════════════════════════════╝
");
        }
    }
}
