using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PlanAlimentarApp
{
    /// <summary>
    /// Notificări paralele pentru anunțuri care corespund cerințelor utilizatorului
    /// Folosește fire de execuție paralele (Task Parallel Library)
    /// </summary>
    public class NotificariParalele
    {
        private string connectionString;

        public NotificariParalele(string connString)
        {
            connectionString = connString;
        }

        /// <summary>
        /// Verifică anunțurile noi și notifică utilizatorii interesați folosind task-uri paralele
        /// </summary>
        public void VerificaSiNotificaAnunturiNoi()
        {
            Console.WriteLine("Se verifică anunțurile noi și se trimit notificări paralele...");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Obține anunțurile noi din ultimele 24 de ore
                var cmdAnunturi = new SqlCommand(@"
                    SELECT Id, Titlu, Descriere 
                    FROM Anunturi 
                    WHERE DataPublicare >= @DataLimit AND Activ = 1", conn);
                
                cmdAnunturi.Parameters.AddWithValue("@DataLimit", DateTime.Now.AddHours(-24));

                var anunturiNoi = new List<(int Id, string Titlu, string Descriere)>();

                using (var reader = cmdAnunturi.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        anunturiNoi.Add((
                            (int)reader["Id"],
                            reader["Titlu"].ToString(),
                            reader["Descriere"].ToString()
                        ));
                    }
                }

                if (anunturiNoi.Count == 0)
                {
                    Console.WriteLine("Nu sunt anunțuri noi în ultimele 24 de ore.");
                    return;
                }

                Console.WriteLine($"S-au găsit {anunturiNoi.Count} anunțuri noi.");

                // Obține toți utilizatorii
                var cmdUtilizatori = new SqlCommand("SELECT Id, Nume, GreutateTinta FROM Utilizatori", conn);
                var utilizatori = new List<(int Id, string Nume, decimal GreutateTinta)>();

                using (var reader = cmdUtilizatori.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        utilizatori.Add((
                            (int)reader["Id"],
                            reader["Nume"].ToString(),
                            (decimal)reader["GreutateTinta"]
                        ));
                    }
                }

                if (utilizatori.Count == 0)
                {
                    Console.WriteLine("Nu există utilizatori înregistrați.");
                    return;
                }

                // Procesare paralelă: Pentru fiecare anunț, verifică care utilizatori sunt interesați
                var optiuniParalele = new ParallelOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount // Folosește toate core-urile disponibile
                };

                int notificariTrimise = 0;

                Parallel.ForEach(anunturiNoi, optiuniParalele, anunt =>
                {
                    Console.WriteLine($"\n[Thread {System.Threading.Thread.CurrentThread.ManagedThreadId}] Procesare anunț: {anunt.Titlu}");

                    foreach (var utilizator in utilizatori)
                    {
                        // Simulare verificare dacă anunțul corespunde preferințelor utilizatorului
                        bool corespunde = VerificaPreferinte(anunt.Descriere, utilizator.GreutateTinta);

                        if (corespunde)
                        {
                            // Trimite notificare asincron
                            TrimiteNotificare(utilizator.Id, utilizator.Nume, anunt.Titlu);
                            System.Threading.Interlocked.Increment(ref notificariTrimise);
                        }
                    }
                });

                Console.WriteLine($"\n✓ Procesare completă. Total notificări trimise: {notificariTrimise}");
            }
        }

        /// <summary>
        /// Verifică dacă un anunț corespunde preferințelor utilizatorului (simulat)
        /// </summary>
        private bool VerificaPreferinte(string descriere, decimal greutateTinta)
        {
            // Logica simplificată: verifică cuvinte cheie în descriere
            string[] cuvinteCheie = { "slăbit", "dietă", "calorii", "nutriție", "sănătos" };
            
            foreach (var cuvant in cuvinteCheie)
            {
                if (descriere.ToLower().Contains(cuvant))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Trimite o notificare către un utilizator
        /// </summary>
        private void TrimiteNotificare(int userId, string nume, string titluAnunt)
        {
            // Simulare trimitere notificare
            Console.WriteLine($"  → Notificare către {nume} (ID: {userId}) despre: {titluAnunt}");

            // Logare în baza de date
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    INSERT INTO LogNotificari (UtilizatorId, Tip, Subiect, Continut, DataTrimitere, Status)
                    VALUES (@UserId, @Tip, @Subiect, @Continut, @Data, @Status)", conn);

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Tip", "Notificare Paralela");
                cmd.Parameters.AddWithValue("@Subiect", titluAnunt);
                cmd.Parameters.AddWithValue("@Continut", "Anunț nou care corespunde preferințelor tale");
                cmd.Parameters.AddWithValue("@Data", DateTime.Now);
                cmd.Parameters.AddWithValue("@Status", "Sent");

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Demo pentru rularea notificărilor paralele
        /// </summary>
        public void DemoNotificariParalele()
        {
            Console.WriteLine("\n=== DEMO NOTIFICĂRI PARALELE ===");
            Console.WriteLine("Această funcție demonstrează utilizarea Task Parallel Library");
            Console.WriteLine("pentru procesarea simultană a mai multor notificări.\n");

            // Adaugă câteva anunțuri demo dacă nu există
            AdaugaAnunturiDemo();

            // Rulează verificarea
            VerificaSiNotificaAnunturiNoi();
        }

        /// <summary>
        /// Adaugă anunțuri demo pentru testare
        /// </summary>
        private void AdaugaAnunturiDemo()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var anunturiDemo = new[]
                {
                    (Titlu: "Ofertă dietă slăbit", Desc: "Program special pentru slăbit sănătos cu monitorizare calorii"),
                    (Titlu: "Nutriție personalizată", Desc: "Planuri de nutriție adaptate obiectivelor tale"),
                    (Titlu: "Alimente sănătoase", Desc: "Descoperă alimentele perfecte pentru dieta ta")
                };

                foreach (var anunt in anunturiDemo)
                {
                    var checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Anunturi WHERE Titlu = @Titlu", conn);
                    checkCmd.Parameters.AddWithValue("@Titlu", anunt.Titlu);

                    if ((int)checkCmd.ExecuteScalar() == 0)
                    {
                        var insertCmd = new SqlCommand(@"
                            INSERT INTO Anunturi (Titlu, Descriere, DataPublicare, Activ)
                            VALUES (@Titlu, @Desc, @Data, 1)", conn);

                        insertCmd.Parameters.AddWithValue("@Titlu", anunt.Titlu);
                        insertCmd.Parameters.AddWithValue("@Desc", anunt.Desc);
                        insertCmd.Parameters.AddWithValue("@Data", DateTime.Now);

                        insertCmd.ExecuteNonQuery();
                        Console.WriteLine($"Adăugat anunț demo: {anunt.Titlu}");
                    }
                }
            }
        }
    }
}
