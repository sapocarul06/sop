using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlanAlimentarApp
{
    /// <summary>
    /// Script de import inițial - Rulează o singură dată la început
    /// Importă date de la mai mulți provideri de alimente
    /// </summary>
    public class ImportInitial
    {
        private string connectionString;
        private HttpClient httpClient;

        public ImportInitial(string connString)
        {
            connectionString = connString;
            httpClient = new HttpClient();
        }

        /// <summary>
        /// Importă date de la mai mulți provideri (simulat)
        /// În producție, acestea ar fi API-uri reale sau scraping
        /// </summary>
        public void ImportaDateDeLaProvideri()
        {
            Console.WriteLine("Se începe importul inițial de la provideri...");

            // Provider 1: API Nutriție (simulat)
            Console.WriteLine("\n[Provider 1] Import date nutriționale...");
            ImportaDeLaProvider1();

            // Provider 2: Bază de date alimente locale (simulat)
            Console.WriteLine("\n[Provider 2] Import alimente tradiționale...");
            ImportaDeLaProvider2();

            // Provider 3: Scraping site-uri specializate (simulat, similar cu exemplul SuperBet)
            Console.WriteLine("\n[Provider 3] Scraping date alimente...");
            ImportaDeLaProvider3();

            Console.WriteLine("\n✓ Import inițial completat cu succes!");
            Console.WriteLine($"Total alimente importate: {NumaraAlimenteImportate()}");
        }

        /// <summary>
        /// Simulare import de la un API de nutriție
        /// </summary>
        private void ImportaDeLaProvider1()
        {
            // Date simulate - în realitate ar fi un apel HTTP către un API
            var alimenteProvider1 = new List<Aliment>
            {
                new Aliment { Nume = "Mere", CaloriiPer100g = 52, ProteinePer100g = 0.3m, CarbohidratiPer100g = 14, GrasimiPer100g = 0.2m, Provider = "API-Nutritie", ImagineHash = "a1b2c3d4e5f6" },
                new Aliment { Nume = "Banane", CaloriiPer100g = 89, ProteinePer100g = 1.1m, CarbohidratiPer100g = 23, GrasimiPer100g = 0.3m, Provider = "API-Nutritie", ImagineHash = "b2c3d4e5f6g7" },
                new Aliment { Nume = "Portocale", CaloriiPer100g = 47, ProteinePer100g = 0.9m, CarbohidratiPer100g = 12, GrasimiPer100g = 0.1m, Provider = "API-Nutritie", ImagineHash = "c3d4e5f6g7h8" },
                new Aliment { Nume = "Struguri", CaloriiPer100g = 67, ProteinePer100g = 0.6m, CarbohidratiPer100g = 17, GrasimiPer100g = 0.4m, Provider = "API-Nutritie", ImagineHash = "d4e5f6g7h8i9" },
                new Aliment { Nume = "Pui piept", CaloriiPer100g = 165, ProteinePer100g = 31, CarbohidratiPer100g = 0, GrasimiPer100g = 3.6m, Provider = "API-Nutritie", ImagineHash = "e5f6g7h8i9j0" },
                new Aliment { Nume = "Orez brun", CaloriiPer100g = 111, ProteinePer100g = 2.6m, CarbohidratiPer100g = 23, GrasimiPer100g = 0.9m, Provider = "API-Nutritie", ImagineHash = "f6g7h8i9j0k1" },
                new Aliment { Nume = "Broccoli", CaloriiPer100g = 34, ProteinePer100g = 2.8m, CarbohidratiPer100g = 7, GrasimiPer100g = 0.4m, Provider = "API-Nutritie", ImagineHash = "g7h8i9j0k1l2" },
                new Aliment { Nume = "Spanac", CaloriiPer100g = 23, ProteinePer100g = 2.9m, CarbohidratiPer100g = 3.6m, GrasimiPer100g = 0.4m, Provider = "API-Nutritie", ImagineHash = "h8i9j0k1l2m3" },
                new Aliment { Nume = "Somn", CaloriiPer100g = 208, ProteinePer100g = 20, CarbohidratiPer100g = 0, GrasimiPer100g = 13, Provider = "API-Nutritie", ImagineHash = "i9j0k1l2m3n4" },
                new Aliment { Nume = "Ouă", CaloriiPer100g = 155, ProteinePer100g = 13, CarbohidratiPer100g = 1.1m, GrasimiPer100g = 11, Provider = "API-Nutritie", ImagineHash = "j0k1l2m3n4o5" }
            };

            SalveazaAlimenteInBaza(alimenteProvider1);
            Console.WriteLine($"  → {alimenteProvider1.Count} alimente importate de la Provider 1");
        }

        /// <summary>
        /// Simulare import din bază de date locală cu alimente tradiționale
        /// </summary>
        private void ImportaDeLaProvider2()
        {
            var alimenteProvider2 = new List<Aliment>
            {
                new Aliment { Nume = "Mămăligă", CaloriiPer100g = 95, ProteinePer100g = 2.1m, CarbohidratiPer100g = 20, GrasimiPer100g = 0.5m, Provider = "Alimente-Traditionale", ImagineHash = "k1l2m3n4o5p6" },
                new Aliment { Nume = "Sarmale", CaloriiPer100g = 185, ProteinePer100g = 8, CarbohidratiPer100g = 12, GrasimiPer100g = 11, Provider = "Alimente-Traditionale", ImagineHash = "l2m3n4o5p6q7" },
                new Aliment { Nume = "Ciorbă de perișoare", CaloriiPer100g = 75, ProteinePer100g = 5, CarbohidratiPer100g = 6, GrasimiPer100g = 3.5m, Provider = "Alimente-Traditionale", ImagineHash = "m3n4o5p6q7r8" },
                new Aliment { Nume = "Cozonac", CaloriiPer100g = 320, ProteinePer100g = 7, CarbohidratiPer100g = 50, GrasimiPer100g = 10, Provider = "Alimente-Traditionale", ImagineHash = "n4o5p6q7r8s9" },
                new Aliment { Nume = "Brânză de burduf", CaloriiPer100g = 350, ProteinePer100g = 25, CarbohidratiPer100g = 2, GrasimiPer100g = 28, Provider = "Alimente-Traditionale", ImagineHash = "o5p6q7r8s9t0" },
                new Aliment { Nume = "Slănina afumată", CaloriiPer100g = 450, ProteinePer100g = 15, CarbohidratiPer100g = 1, GrasimiPer100g = 43, Provider = "Alimente-Traditionale", ImagineHash = "p6q7r8s9t0u1" },
                new Aliment { Nume = "Zeamă de pui", CaloriiPer100g = 45, ProteinePer100g = 3, CarbohidratiPer100g = 5, GrasimiPer100g = 1.5m, Provider = "Alimente-Traditionale", ImagineHash = "q7r8s9t0u1v2" },
                new Aliment { Nume = "Plăcintă cu brânză", CaloriiPer100g = 280, ProteinePer100g = 9, CarbohidratiPer100g = 30, GrasimiPer100g = 14, Provider = "Alimente-Traditionale", ImagineHash = "r8s9t0u1v2w3" }
            };

            SalveazaAlimenteInBaza(alimenteProvider2);
            Console.WriteLine($"  → {alimenteProvider2.Count} alimente importate de la Provider 2");
        }

        /// <summary>
        /// Simulare scraping (similar cu exemplul SuperBet din cerință)
        /// În loc de Selenium, folosim date simulate
        /// </summary>
        private void ImportaDeLaProvider3()
        {
            // Aceasta ar folosi Selenium WebDriver ca în exemplul dat
            // Pentru demo, simulăm rezultatul scraping-ului
            
            var alimenteProvider3 = new List<Aliment>
            {
                new Aliment { Nume = "Iaurt grecesc", CaloriiPer100g = 97, ProteinePer100g = 9, CarbohidratiPer100g = 4, GrasimiPer100g = 5, Provider = "Scraping-Site", ImagineHash = "s9t0u1v2w3x4" },
                new Aliment { Nume = "Granola", CaloriiPer100g = 470, ProteinePer100g = 10, CarbohidratiPer100g = 60, GrasimiPer100g = 20, Provider = "Scraping-Site", ImagineHash = "t0u1v2w3x4y5" },
                new Aliment { Nume = "Avocado", CaloriiPer100g = 160, ProteinePer100g = 2, CarbohidratiPer100g = 9, GrasimiPer100g = 15, Provider = "Scraping-Site", ImagineHash = "u1v2w3x4y5z6" },
                new Aliment { Nume = "Quinoa", CaloriiPer100g = 120, ProteinePer100g = 4.4m, CarbohidratiPer100g = 21, GrasimiPer100g = 1.9m, Provider = "Scraping-Site", ImagineHash = "v2w3x4y5z6a7" },
                new Aliment { Nume = "Somon afumat", CaloriiPer100g = 117, ProteinePer100g = 18, CarbohidratiPer100g = 0, GrasimiPer100g = 4.3m, Provider = "Scraping-Site", ImagineHash = "w3x4y5z6a7b8" },
                new Aliment { Nume = "Hummus", CaloriiPer100g = 166, ProteinePer100g = 8, CarbohidratiPer100g = 14, GrasimiPer100g = 10, Provider = "Scraping-Site", ImagineHash = "x4y5z6a7b8c9" }
            };

            SalveazaAlimenteInBaza(alimenteProvider3);
            Console.WriteLine($"  → {alimenteProvider3.Count} alimente importate de la Provider 3 (scraping)");
        }

        /// <summary>
        /// Salvează lista de alimente în baza de date SQL Server
        /// </summary>
        private void SalveazaAlimenteInBaza(List<Aliment> alimente)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                foreach (var aliment in alimente)
                {
                    // Verificăm dacă există deja (evităm duplicatele la importul inițial)
                    var checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Alimente WHERE Nume = @Nume AND Provider = @Provider", conn);
                    checkCmd.Parameters.AddWithValue("@Nume", aliment.Nume);
                    checkCmd.Parameters.AddWithValue("@Provider", aliment.Provider);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        var insertCmd = new SqlCommand(@"
                            INSERT INTO Alimente 
                            (Nume, CaloriiPer100g, ProteinePer100g, CarbohidratiPer100g, GrasimiPer100g, Provider, ImagineHash, CaleImagine)
                            VALUES 
                            (@Nume, @Calorii, @Proteine, @Carbo, @Grasimi, @Provider, @Hash, @Cale)", conn);

                        insertCmd.Parameters.AddWithValue("@Nume", aliment.Nume);
                        insertCmd.Parameters.AddWithValue("@Calorii", aliment.CaloriiPer100g);
                        insertCmd.Parameters.AddWithValue("@Proteine", aliment.ProteinePer100g);
                        insertCmd.Parameters.AddWithValue("@Carbo", aliment.CarbohidratiPer100g);
                        insertCmd.Parameters.AddWithValue("@Grasimi", aliment.GrasimiPer100g);
                        insertCmd.Parameters.AddWithValue("@Provider", aliment.Provider);
                        insertCmd.Parameters.AddWithValue("@Hash", aliment.ImagineHash ?? DBNull.Value);
                        insertCmd.Parameters.AddWithValue("@Cale", $"~/images/{aliment.Nume.ToLower().Replace(' ', '_')}.jpg");

                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Numără totalul de alimente importate
        /// </summary>
        private int NumaraAlimenteImportate()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Alimente", conn);
                return (int)cmd.ExecuteScalar();
            }
        }
    }

    /// <summary>
    /// Model pentru alimente
    /// </summary>
    public class Aliment
    {
        public string Nume { get; set; }
        public decimal CaloriiPer100g { get; set; }
        public decimal ProteinePer100g { get; set; }
        public decimal CarbohidratiPer100g { get; set; }
        public decimal GrasimiPer100g { get; set; }
        public string Provider { get; set; }
        public string ImagineHash { get; set; }
    }
}
