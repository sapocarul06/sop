using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PlanAlimentarApp
{
    /// <summary>
    /// Programul principal - Aplicație de gestionare a planurilor alimentare
    /// </summary>
    class Program
    {
        // String de conexiune la SQL Server (modificați conform configurației dumneavoastră)
        private static string connectionString = "Server=localhost;Database=PlanAlimentarDB;Integrated Security=true;";

        static void Main(string[] args)
        {
            Console.WriteLine("=== APLICAȚIE GESTIONARE PLAN ALIMENTAR ===");
            Console.WriteLine("==========================================\n");

            bool continuare = true;
            while (continuare)
            {
                AfiseazaMeniu();
                string optiune = Console.ReadLine();

                switch (optiune)
                {
                    case "1":
                        RuleazaImportInitial();
                        break;
                    case "2":
                        RuleazaImportZilnic();
                        break;
                    case "3":
                        GenereazaPlanAlimentar();
                        break;
                    case "4":
                        DetectareDuplicateDemo();
                        break;
                    case "5":
                        GenereazaRapoarteDemo();
                        break;
                    case "6":
                        TrimiteEmailuriDemo();
                        break;
                    case "7":
                        NotificariParaleleDemo();
                        break;
                    case "8":
                        StudiiCazDemo();
                        break;
                    case "9":
                        CalculeazaStatistici();
                        break;
                    case "0":
                        continuare = false;
                        Console.WriteLine("La revedere!");
                        break;
                    default:
                        Console.WriteLine("Opțiune invalidă. Încercați din nou.");
                        break;
                }

                if (continuare)
                {
                    Console.WriteLine("\nApăsați orice tastă pentru a continua...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void AfiseazaMeniu()
        {
            Console.WriteLine("MENIU PRINCIPAL:");
            Console.WriteLine("1. Import inițial date (rulează o singură dată)");
            Console.WriteLine("2. Import zilnic actualizare date (cron daily)");
            Console.WriteLine("3. Generează plan alimentar personalizat");
            Console.WriteLine("4. Detectare anunțuri duplicate (imagini)");
            Console.WriteLine("5. Generare rapoarte PDF/Excel");
            Console.WriteLine("6. Trimitere emailuri promoționale");
            Console.WriteLine("7. Notificări paralele pentru anunțuri");
            Console.WriteLine("8. Studii de caz (stocare imagini, server vs serverless)");
            Console.WriteLine("9. Statistici și consum timp");
            Console.WriteLine("0. Ieșire");
            Console.Write("\nAlegeți o opțiune: ");
        }

        static void RuleazaImportInitial()
        {
            Console.WriteLine("\n--- IMPORT INIȚIAL DATE ---");
            var import = new ImportInitial(connectionString);
            import.ImportaDateDeLaProvideri();
            Console.WriteLine("Import inițial completat cu succes!");
        }

        static void RuleazaImportZilnic()
        {
            Console.WriteLine("\n--- IMPORT ZILNIC ACTUALIZARE ---");
            var import = new ImportZilnic(connectionString);
            import.ActualizeazaDateZilnice();
            Console.WriteLine("Import zilnic completat cu succes!");
            Console.WriteLine("NOTĂ: Pentru automatizare, configurați Task Scheduler în Windows:");
            Console.WriteLine("  - Acțiune: Start a program");
            Console.WriteLine($"  - Program: {AppDomain.CurrentDomain.FriendlyName} argument: --daily-import");
        }

        static void GenereazaPlanAlimentar()
        {
            Console.WriteLine("\n--- GENERARE PLAN ALIMENTAR ---");
            
            Console.Write("Introduceți numele: ");
            string nume = Console.ReadLine();

            Console.Write("Greutate actuală (kg): ");
            decimal greutate = decimal.Parse(Console.ReadLine());

            Console.Write("Înălțime (cm): ");
            decimal inaltime = decimal.Parse(Console.ReadLine());

            Console.Write("Vârsta (ani): ");
            int varsta = int.Parse(Console.ReadLine());

            Console.Write("Sex (M/F): ");
            string sex = Console.ReadLine().ToUpper();

            Console.Write("Greutate țintă (kg): ");
            decimal greutateTinta = decimal.Parse(Console.ReadLine());

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("AdaugaUtilizator", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Nume", nume);
                    cmd.Parameters.AddWithValue("@GreutateActuala", greutate);
                    cmd.Parameters.AddWithValue("@Inaltime", inaltime);
                    cmd.Parameters.AddWithValue("@Varsta", varsta);
                    cmd.Parameters.AddWithValue("@Sex", sex);
                    cmd.Parameters.AddWithValue("@GreutateTinta", greutateTinta);

                    int userId = Convert.ToInt32(cmd.ExecuteScalar());
                    Console.WriteLine($"\nUtilizator înregistrat cu ID: {userId}");

                    // Calcul necesar caloric (Formula Mifflin-St Jeor)
                    decimal bmr = sex == "M" 
                        ? (10 * greutate) + (6.25f * inaltime) - (5 * varsta) + 5
                        : (10 * greutate) + (6.25f * inaltime) - (5 * varsta) - 161;

                    // Factor activitate moderată (1.55)
                    decimal caloriiNecesare = bmr * 1.55m;
                    
                    // Deficit caloric pentru slăbit (500 kcal/zi)
                    if (greutate > greutateTinta)
                    {
                        caloriiNecesare -= 500;
                    }

                    Console.WriteLine($"\nNecesar caloric zilnic: {caloriiNecesare:F0} kcal");
                    Console.WriteLine("Se generează plan alimentar pentru 7 zile...");

                    // Generare plan (simplificat)
                    var planGenerator = new PlanAlimentarGenerator(connectionString);
                    planGenerator.GenereazaPlanSaptamanal(userId, caloriiNecesare);
                }
            }
        }

        static void DetectareDuplicateDemo()
        {
            Console.WriteLine("\n--- DETECTARE DUPLICATE IMAGINI ---");
            var detector = new DetectareDuplicate(connectionString);
            detector.DetecteazaAnunturiDuplicate();
        }

        static void GenereazaRapoarteDemo()
        {
            Console.WriteLine("\n--- GENERARE RAPOARTE ---");
            var generator = new GenerareRapoarte(connectionString);
            generator.GenerareRaportPDF();
            generator.GenerareRaportExcel();
        }

        static void TrimiteEmailuriDemo()
        {
            Console.WriteLine("\n--- TRIMITERE EMAILURI PROMOȚIONALE ---");
            var serviciuEmail = new ServiciiEmail(connectionString);
            serviciuEmail.TrimiteEmailuriPromotionaleAsync().Wait();
        }

        static void NotificariParaleleDemo()
        {
            Console.WriteLine("\n--- NOTIFICĂRI PARALELE ---");
            var notificator = new NotificariParalele(connectionString);
            notificator.VerificaSiNotificaAnunturiNoi();
        }

        static void StudiiCazDemo()
        {
            Console.WriteLine("\n--- STUDII DE CAZ ---");
            var studii = new StudiiCaz();
            studii.PreZintaStudiuStocareImagini();
            studii.PreZintaStudiuServerVsServerless();
        }

        static void CalculeazaStatistici()
        {
            Console.WriteLine("\n--- STATISTICI ȘI CONSUM TIMP ---");
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                // Număr total utilizatori
                var cmd1 = new SqlCommand("SELECT COUNT(*) FROM Utilizatori", conn);
                int nrUtilizatori = (int)cmd1.ExecuteScalar();

                // Număr total alimente
                var cmd2 = new SqlCommand("SELECT COUNT(*) FROM Alimente", conn);
                int nrAlimente = (int)cmd2.ExecuteScalar();

                // Număr planuri generate
                var cmd3 = new SqlCommand("SELECT COUNT(*) FROM PlanuriAlimentare", conn);
                int nrPlanuri = (int)cmd3.ExecuteScalar();

                Console.WriteLine($"Total utilizatori: {nrUtilizatori}");
                Console.WriteLine($"Total alimente în baza de date: {nrAlimente}");
                Console.WriteLine($"Total planuri generate: {nrPlanuri}");

                // Simulare calcul statistic complex (consumator de timp)
                Console.WriteLine("\nCalcul statistic complex (simulare)...");
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                System.Threading.Thread.Sleep(2000); // Simulare procesare intensă
                stopwatch.Stop();
                Console.WriteLine($"Timp execuție: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }

    /// <summary>
    /// Generator de planuri alimentare
    /// </summary>
    public class PlanAlimentarGenerator
    {
        private string connectionString;

        public PlanAlimentarGenerator(string connString)
        {
            connectionString = connString;
        }

        public void GenereazaPlanSaptamanal(int userId, decimal caloriiNecesare)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                string[] zileleSaptamanii = { "Luni", "Marți", "Miercuri", "Joi", "Vineri", "Sâmbătă", "Duminică" };
                
                for (int zi = 1; zi <= 7; zi++)
                {
                    // Selectare aleatorie alimente pentru fiecare masă
                    var cmd = new SqlCommand(@"
                        SELECT TOP 4 Nume FROM Alimente 
                        WHERE CaloriiPer100g BETWEEN @minCal AND @maxCal
                        ORDER BY NEWID()", conn);
                    
                    cmd.Parameters.AddWithValue("@minCal", caloriiNecesare * 0.7m);
                    cmd.Parameters.AddWithValue("@maxCal", caloriiNecesare * 1.3m);

                    using (var reader = cmd.ExecuteReader())
                    {
                        var mese = new List<string>();
                        while (reader.Read())
                        {
                            mese.Add(reader["Nume"].ToString());
                        }

                        if (mese.Count >= 4)
                        {
                            var insertCmd = new SqlCommand(@"
                                INSERT INTO PlanuriAlimentare 
                                (UtilizatorId, ZiuaSaptamanii, MicDejun, Pranz, Cina, Gustare, TotalCalorii)
                                VALUES 
                                (@UserId, @Zi, @MicDejun, @Pranz, @Cina, @Gustare, @TotalCal)", conn);

                            insertCmd.Parameters.AddWithValue("@UserId", userId);
                            insertCmd.Parameters.AddWithValue("@Zi", zi);
                            insertCmd.Parameters.AddWithValue("@MicDejun", mese[0]);
                            insertCmd.Parameters.AddWithValue("@Pranz", mese[1]);
                            insertCmd.Parameters.AddWithValue("@Cina", mese[2]);
                            insertCmd.Parameters.AddWithValue("@Gustare", mese[3]);
                            insertCmd.Parameters.AddWithValue("@TotalCal", caloriiNecesare);

                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }

                Console.WriteLine("Plan alimentar generat cu succes pentru 7 zile!");
                Console.WriteLine("Puteți genera raportul PDF pentru a vizualiza planul detaliat.");
            }
        }
    }
}
