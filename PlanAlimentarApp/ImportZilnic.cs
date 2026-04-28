using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PlanAlimentarApp
{
    /// <summary>
    /// Script de import zilnic - Rulează în fiecare zi (configurat ca cron/Task Scheduler)
    /// Actualizează datele existente și adaugă alimente noi de la provideri
    /// </summary>
    public class ImportZilnic
    {
        private string connectionString;

        public ImportZilnic(string connString)
        {
            connectionString = connString;
        }

        /// <summary>
        /// Actualizează datele zilnic - rulează automat prin Task Scheduler
        /// </summary>
        public void ActualizeazaDateZilnice()
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Se începe actualizarea zilnică...");

            // 1. Actualizare prețuri/disponibilitate de la provideri
            ActualizeazaDateDeLaProvideri();

            // 2. Adăugare alimente sezoniere noi
            AdaugaAlimenteSezoniere();

            // 3. Curățare date vechi (opțional, după 90 de zile)
            CurataDateVechi();

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ✓ Actualizare zilnică completată!");
        }

        /// <summary>
        /// Actualizează informațiile despre alimente de la provideri
        /// </summary>
        private void ActualizeazaDateDeLaProvideri()
        {
            Console.WriteLine("  → Se actualizează datele de la provideri...");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Simulare: Actualizare data import pentru toate alimentele
                var updateCmd = new SqlCommand(@"
                    UPDATE Alimente 
                    SET DataImport = @DataNow
                    WHERE Provider IN ('API-Nutritie', 'Scraping-Site')", conn);
                
                updateCmd.Parameters.AddWithValue("@DataNow", DateTime.Now);
                int rowsAffected = updateCmd.ExecuteNonQuery();

                Console.WriteLine($"     {rowsAffected} înregistrări actualizate.");
            }
        }

        /// <summary>
        /// Adaugă alimente sezoniere în funcție de perioadă
        /// </summary>
        private void AdaugaAlimenteSezoniere()
        {
            Console.WriteLine("  → Se verifică alimentele sezoniere...");

            int lunaCurenta = DateTime.Now.Month;
            var alimenteSezoniere = new List<Aliment>();

            // Primăvară (Martie, Aprilie, Mai)
            if (lunaCurenta >= 3 && lunaCurenta <= 5)
            {
                alimenteSezoniere.AddRange(new[]
                {
                    new Aliment { Nume = "Ridichi de primăvară", CaloriiPer100g = 16, ProteinePer100g = 0.7m, CarbohidratiPer100g = 3.4m, GrasimiPer100g = 0.1m, Provider = "Sezonier-Primavara", ImagineHash = "sez1prim" },
                    new Aliment { Nume = "Ștevie", CaloriiPer100g = 20, ProteinePer100g = 2.5m, CarbohidratiPer100g = 3, GrasimiPer100g = 0.3m, Provider = "Sezonier-Primavara", ImagineHash = "sez2prim" },
                    new Aliment { Nume = "Urzici", CaloriiPer100g = 42, ProteinePer100g = 5.5m, CarbohidratiPer100g = 6, GrasimiPer100g = 0.5m, Provider = "Sezonier-Primavara", ImagineHash = "sez3prim" }
                });
            }
            // Vară (Iunie, Iulie, August)
            else if (lunaCurenta >= 6 && lunaCurenta <= 8)
            {
                alimenteSezoniere.AddRange(new[]
                {
                    new Aliment { Nume = "Roșii de grădină", CaloriiPer100g = 18, ProteinePer100g = 0.9m, CarbohidratiPer100g = 3.9m, GrasimiPer100g = 0.2m, Provider = "Sezonier-Vara", ImagineHash = "sez1vara" },
                    new Aliment { Nume = "Castraveți", CaloriiPer100g = 15, ProteinePer100g = 0.7m, CarbohidratiPer100g = 3.6m, GrasimiPer100g = 0.1m, Provider = "Sezonier-Vara", ImagineHash = "sez2vara" },
                    new Aliment { Nume = "Pepene galben", CaloriiPer100g = 34, ProteinePer100g = 0.8m, CarbohidratiPer100g = 8, GrasimiPer100g = 0.2m, Provider = "Sezonier-Vara", ImagineHash = "sez3vara" },
                    new Aliment { Nume = "Cireșe", CaloriiPer100g = 50, ProteinePer100g = 1, CarbohidratiPer100g = 12, GrasimiPer100g = 0.3m, Provider = "Sezonier-Vara", ImagineHash = "sez4vara" }
                });
            }
            // Toamnă (Septembrie, Octombrie, Noiembrie)
            else if (lunaCurenta >= 9 && lunaCurenta <= 11)
            {
                alimenteSezoniere.AddRange(new[]
                {
                    new Aliment { Nume = "Dovleac", CaloriiPer100g = 26, ProteinePer100g = 1, CarbohidratiPer100g = 6.5m, GrasimiPer100g = 0.1m, Provider = "Sezonier-Toamna", ImagineHash = "sez1toamna" },
                    new Aliment { Nume = "Struguri proaspeți", CaloriiPer100g = 67, ProteinePer100g = 0.6m, CarbohidratiPer100g = 17, GrasimiPer100g = 0.4m, Provider = "Sezonier-Toamna", ImagineHash = "sez2toamna" },
                    new Aliment { Nume = "Nuci", CaloriiPer100g = 654, ProteinePer100g = 15, CarbohidratiPer100g = 14, GrasimiPer100g = 65, Provider = "Sezonier-Toamna", ImagineHash = "sez3toamna" }
                });
            }
            // Iarnă (Decembrie, Ianuarie, Februarie)
            else
            {
                alimenteSezoniere.AddRange(new[]
                {
                    new Aliment { Nume = "Varză murată", CaloriiPer100g = 19, ProteinePer100g = 0.9m, CarbohidratiPer100g = 4.3m, GrasimiPer100g = 0.1m, Provider = "Sezonier-Iarna", ImagineHash = "sez1iarna" },
                    new Aliment { Nume = "Gutui", CaloriiPer100g = 57, ProteinePer100g = 0.4m, CarbohidratiPer100g = 15, GrasimiPer100g = 0.1m, Provider = "Sezonier-Iarna", ImagineHash = "sez2iarna" },
                    new Aliment { Nume = "Portocale de iarnă", CaloriiPer100g = 47, ProteinePer100g = 0.9m, CarbohidratiPer100g = 12, GrasimiPer100g = 0.1m, Provider = "Sezonier-Iarna", ImagineHash = "sez3iarna" }
                });
            }

            // Inserare alimente sezoniere dacă nu există deja
            int adaugate = 0;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (var aliment in alimenteSezoniere)
                {
                    var checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Alimente WHERE Nume = @Nume", conn);
                    checkCmd.Parameters.AddWithValue("@Nume", aliment.Nume);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        var insertCmd = new SqlCommand(@"
                            INSERT INTO Alimente 
                            (Nume, CaloriiPer100g, ProteinePer100g, CarbohidratiPer100g, GrasimiPer100g, Provider, ImagineHash, CaleImagine, DataImport)
                            VALUES 
                            (@Nume, @Calorii, @Proteine, @Carbo, @Grasimi, @Provider, @Hash, @Cale, @Data)", conn);

                        insertCmd.Parameters.AddWithValue("@Nume", aliment.Nume);
                        insertCmd.Parameters.AddWithValue("@Calorii", aliment.CaloriiPer100g);
                        insertCmd.Parameters.AddWithValue("@Proteine", aliment.ProteinePer100g);
                        insertCmd.Parameters.AddWithValue("@Carbo", aliment.CarbohidratiPer100g);
                        insertCmd.Parameters.AddWithValue("@Grasimi", aliment.GrasimiPer100g);
                        insertCmd.Parameters.AddWithValue("@Provider", aliment.Provider);
                        insertCmd.Parameters.AddWithValue("@Hash", aliment.ImagineHash);
                        insertCmd.Parameters.AddWithValue("@Cale", $"~/images/{aliment.Nume.ToLower().Replace(' ', '_')}.jpg");
                        insertCmd.Parameters.AddWithValue("@Data", DateTime.Now);

                        insertCmd.ExecuteNonQuery();
                        adaugate++;
                    }
                }
            }

            Console.WriteLine($"     {adaugate} alimente sezoniere adăugate.");
        }

        /// <summary>
        /// Curăță datele mai vechi de 90 de zile (opțional)
        /// </summary>
        private void CurataDateVechi()
        {
            Console.WriteLine("  → Se curăță datele vechi (opțional)...");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Șterge alimentele cu data import mai veche de 90 de zile
                // DOAR dacă au fost marcate ca expirate
                var deleteCmd = new SqlCommand(@"
                    DELETE FROM Alimente 
                    WHERE DataImport < @DataLimit 
                    AND Provider LIKE '%Temp%'", conn);
                
                deleteCmd.Parameters.AddWithValue("@DataLimit", DateTime.Now.AddDays(-90));
                int deleted = deleteCmd.ExecuteNonQuery();

                if (deleted > 0)
                {
                    Console.WriteLine($"     {deleted} înregistrări vechi șterse.");
                }
                else
                {
                    Console.WriteLine("     Nu sunt înregistrări vechi de șters.");
                }
            }
        }
    }
}
