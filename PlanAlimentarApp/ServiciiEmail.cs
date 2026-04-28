using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Net.Mail;

namespace PlanAlimentarApp
{
    /// <summary>
    /// Servicii de email asincrone pentru trimiterea de materiale promoționale
    /// </summary>
    public class ServiciiEmail
    {
        private string connectionString;
        
        // Configurație SMTP (de actualizat cu serverul real)
        private string smtpServer = "smtp.gmail.com";
        private int smtpPort = 587;
        private string emailExpeditor = "aplicatie@planalimentar.ro";
        private string parolaEmail = "parola_app_generata";

        public ServiciiEmail(string connString)
        {
            connectionString = connString;
        }

        /// <summary>
        /// Trimite emailuri promoționale către toți utilizatorii (asincron)
        /// </summary>
        public async Task TrimiteEmailuriPromotionaleAsync()
        {
            Console.WriteLine("Se pregătește trimiterea emailurilor promoționale...");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Obține toți utilizatorii
                var cmd = new SqlCommand("SELECT Id, Nume FROM Utilizatori", conn);
                
                using (var reader = cmd.ExecuteReader())
                {
                    var taskuri = new System.Collections.Generic.List<Task>();

                    while (reader.Read())
                    {
                        int userId = (int)reader["Id"];
                        string nume = reader["Nume"].ToString();

                        // Creează task asincron pentru fiecare utilizator
                        taskuri.Add(TrimiteEmailAsync(userId, nume));
                    }

                    // Așteaptă finalizarea tuturor emailurilor
                    await Task.WhenAll(taskuri);

                    Console.WriteLine($"✓ Toate emailurile au fost procesate ({taskuri.Count} trimiteri).");
                }
            }
        }

        /// <summary>
        /// Trimite un email单个 către un utilizator
        /// </summary>
        private async Task TrimiteEmailAsync(int userId, string nume)
        {
            try
            {
                // Simulare trimitere email (nu trimite real fără configurație SMTP validă)
                Console.WriteLine($"  → Se pregătește email pentru: {nume} (ID: {userId})");

                // În producție, codul real ar fi:
                /*
                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential(emailExpeditor, parolaEmail);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(emailExpeditor),
                        Subject = "Ofertă specială: Plan alimentar personalizat!",
                        Body = $@"
                            Salut {nume},
                            
                            Avem o ofertă specială pentru tine!
                            Descoperă planul alimentar perfect pentru obiectivele tale.
                            
                            Echipa PlanAlimentar
                        ",
                        IsBodyHtml = false
                    };
                    mailMessage.To.Add($"{nume.ToLower().Replace(' ', '.')}@email.com");

                    await client.SendMailAsync(mailMessage);
                }
                */

                // Simulare delay rețea
                await Task.Delay(100);

                // Logare în baza de date
                LogareTrimitereEmail(userId, "Email Promotional", "Ofertă specială", "Sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Eroare la trimiterea către {nume}: {ex.Message}");
                LogareTrimitereEmail(userId, "Email Promotional", "Ofertă specială", "Failed");
            }
        }

        /// <summary>
        /// Loghează trimiterea emailului în baza de date
        /// </summary>
        private void LogareTrimitereEmail(int userId, string tip, string subiect, string status)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    INSERT INTO LogNotificari (UtilizatorId, Tip, Subiect, Continut, DataTrimitere, Status)
                    VALUES (@UserId, @Tip, @Subiect, @Continut, @Data, @Status)", conn);

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Tip", tip);
                cmd.Parameters.AddWithValue("@Subiect", subiect);
                cmd.Parameters.AddWithValue("@Continut", "Material promoțional plan alimentar");
                cmd.Parameters.AddWithValue("@Data", DateTime.Now);
                cmd.Parameters.AddWithValue("@Status", status);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Trimite notificare pentru un anunț nou care corespunde preferințelor utilizatorului
        /// </summary>
        public async Task TrimiteNotificareAnuntAsync(int userId, string titluAnunt)
        {
            try
            {
                Console.WriteLine($"  → Notificare pentru utilizator {userId}: {titluAnunt}");
                
                // Simulare
                await Task.Delay(50);
                
                LogareTrimitereEmail(userId, "Notificare", $"Anunț nou: {titluAnunt}", "Sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Eroare notificare: {ex.Message}");
            }
        }
    }
}
