using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace PlanAlimentarApp
{
    /// <summary>
    /// Detectare anunțuri duplicate pe baza hash-ului imaginilor
    /// </summary>
    public class DetectareDuplicate
    {
        private string connectionString;

        public DetectareDuplicate(string connString)
        {
            connectionString = connString;
        }

        /// <summary>
        /// Detectează anunțurile cu imagini identice folosind hash-uri
        /// </summary>
        public void DetecteazaAnunturiDuplicate()
        {
            Console.WriteLine("Se caută anunțuri cu imagini identice...");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Găsește hash-uri duplicate în tabelul Anunturi
                var cmd = new SqlCommand(@"
                    SELECT ImagineHash, COUNT(*) as NrDuplicate
                    FROM Anunturi
                    WHERE ImagineHash IS NOT NULL
                    GROUP BY ImagineHash
                    HAVING COUNT(*) > 1", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    bool gasitDuplicate = false;

                    while (reader.Read())
                    {
                        gasitDuplicate = true;
                        string hash = reader["ImagineHash"].ToString();
                        int nrDuplicate = (int)reader["NrDuplicate"];

                        Console.WriteLine($"\n⚠ Hash duplicat găsit: {hash}");
                        Console.WriteLine($"   Număr anunțuri cu acest hash: {nrDuplicate}");

                        // Afișează detaliile anunțurilor duplicate
                        var detaliiCmd = new SqlCommand(@"
                            SELECT Id, Titlu, DataPublicare 
                            FROM Anunturi 
                            WHERE ImagineHash = @Hash", conn);
                        detaliiCmd.Parameters.AddWithValue("@Hash", hash);

                        using (var detaliiReader = detaliiCmd.ExecuteReader())
                        {
                            while (detaliiReader.Read())
                            {
                                Console.WriteLine($"   - ID: {detaliiReader["Id"]}, Titlu: {detaliiReader["Titlu"]}, Data: {detaliiReader["DataPublicare"]}");
                            }
                        }
                    }

                    if (!gasitDuplicate)
                    {
                        Console.WriteLine("✓ Nu s-au găsit anunțuri duplicate.");
                    }
                }

                // Verifică și în tabelul Alimente
                Console.WriteLine("\nSe verifică și alimentele pentru duplicate...");
                
                var cmdAlimente = new SqlCommand(@"
                    SELECT ImagineHash, Nume, COUNT(*) as NrDuplicate
                    FROM Alimente
                    WHERE ImagineHash IS NOT NULL
                    GROUP BY ImagineHash, Nume
                    HAVING COUNT(*) > 1", conn);

                using (var reader = cmdAlimente.ExecuteReader())
                {
                    bool gasitDuplicateAlimente = false;

                    while (reader.Read())
                    {
                        gasitDuplicateAlimente = true;
                        Console.WriteLine($"⚠ Aliment duplicat: {reader["Nume"]} (Hash: {reader["ImagineHash"]})");
                    }

                    if (!gasitDuplicateAlimente)
                    {
                        Console.WriteLine("✓ Nu s-au găsit alimente duplicate.");
                    }
                }
            }
        }

        /// <summary>
        /// Calculează hash-ul MD5 pentru o imagine (simulat)
        /// În producție, ar citi fișierul imagine real
        /// </summary>
        public string CalculeazaHashImagine(byte[] dateImagine)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(dateImagine);
                StringBuilder sb = new StringBuilder();
                
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                
                return sb.ToString();
            }
        }

        /// <summary>
        /// Adaugă un anunț nou cu verificare de duplicate
        /// </summary>
        public bool AdaugaAnuntFaraDuplicate(string titlu, string descriere, string hashImagine)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Verifică dacă există deja un anunț cu același hash
                var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Anunturi WHERE ImagineHash = @Hash AND Activ = 1", conn);
                checkCmd.Parameters.AddWithValue("@Hash", hashImagine);

                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    Console.WriteLine($"⚠ Anunț duplicat detectat! Hash: {hashImagine}");
                    return false;
                }

                // Adaugă anunțul nou
                var insertCmd = new SqlCommand(@"
                    INSERT INTO Anunturi (Titlu, Descriere, ImagineHash, DataPublicare, Activ)
                    VALUES (@Titlu, @Descriere, @Hash, @Data, 1)", conn);

                insertCmd.Parameters.AddWithValue("@Titlu", titlu);
                insertCmd.Parameters.AddWithValue("@Descriere", descriere);
                insertCmd.Parameters.AddWithValue("@Hash", hashImagine);
                insertCmd.Parameters.AddWithValue("@Data", DateTime.Now);

                insertCmd.ExecuteNonQuery();
                Console.WriteLine("✓ Anunț adăugat cu succes!");
                return true;
            }
        }
    }
}
