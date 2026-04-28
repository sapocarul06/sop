using System;
using System.Data.SqlClient;
using System.IO;

namespace PlanAlimentarApp
{
    /// <summary>
    /// Generare rapoarte PDF și Excel
    /// </summary>
    public class GenerareRapoarte
    {
        private string connectionString;

        public GenerareRaport(string connString)
        {
            connectionString = connString;
        }

        /// <summary>
        /// Generează raport PDF cu planul alimentar
        /// Folosește o abordare simplificată fără librării externe
        /// </summary>
        public void GenerareRaportPDF()
        {
            Console.WriteLine("Se generează raport PDF...");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Obține planurile alimentare pentru toți utilizatorii
                var cmd = new SqlCommand(@"
                    SELECT u.Nume, p.ZiuaSaptamanii, p.MicDejun, p.Pranz, p.Cina, p.Gustare, p.TotalCalorii
                    FROM PlanuriAlimentare p
                    JOIN Utilizatori u ON p.UtilizatorId = u.Id
                    ORDER BY u.Nume, p.ZiuaSaptamanii", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    string continutText = "PLAN ALIMENTAR SĂPTĂMÂNAL\n";
                    continutText += "============================\n\n";

                    string numeCurent = "";
                    
                    while (reader.Read())
                    {
                        string nume = reader["Nume"].ToString();
                        
                        if (nume != numeCurent)
                        {
                            if (numeCurent != "")
                            {
                                continutText += "\n-----------------------------------\n\n";
                            }
                            
                            numeCurent = nume;
                            continutText += $"Utilizator: {nume}\n";
                            continutText += "-----------------------------------\n";
                        }

                        int ziua = (int)reader["ZiuaSaptamanii"];
                        string ziuaNume = GetZiuaNume(ziua);
                        
                        continutText += $"\n{ziuaNume}:\n";
                        continutText += $"  Mic dejun: {reader["MicDejun"]}\n";
                        continutText += $"  Prânz: {reader["Pranz"]}\n";
                        continutText += $"  Cină: {reader["Cina"]}\n";
                        continutText += $"  Gustare: {reader["Gustare"]}\n";
                        continutText += $"  Total calorii: {reader["TotalCalorii"]} kcal\n";
                    }

                    // Salvează ca fișier text (simulare PDF - în producție s-ar folosi iTextSharp)
                    string caleFisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlanAlimentar.txt");
                    File.WriteAllText(caleFisier, continutText);

                    Console.WriteLine($"✓ Raport generat la: {caleFisier}");
                    Console.WriteLine("NOTĂ: Pentru PDF real, instalați NuGet: iText7 sau iTextSharp");
                }
            }
        }

        /// <summary>
        /// Generează raport Excel (CSV) cu date nutriționale
        /// </summary>
        public void GenerareRaportExcel()
        {
            Console.WriteLine("\nSe generează raport Excel (format CSV)...");

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var cmd = new SqlCommand(@"
                    SELECT Nume, CaloriiPer100g, ProteinePer100g, CarbohidratiPer100g, GrasimiPer100g, Provider, DataImport
                    FROM Alimente
                    ORDER BY Nume", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    string csvContent = "Nume,Calorii/100g,Proteine/100g,Carbohidrati/100g,Grasimi/100g,Provider,DataImport\n";

                    while (reader.Read())
                    {
                        string linie = $"{EscapeCsv(reader["Nume"].ToString())}," +
                                      $"{reader["CaloriiPer100g"]}," +
                                      $"{reader["ProteinePer100g"]}," +
                                      $"{reader["CarbohidratiPer100g"]}," +
                                      $"{reader["GrasimiPer100g"]}," +
                                      $"{EscapeCsv(reader["Provider"].ToString())}," +
                                      $"{((DateTime)reader["DataImport"]):yyyy-MM-dd}\n";
                        
                        csvContent += linie;
                    }

                    // Salvează ca CSV
                    string caleFisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DateNutritionale.csv");
                    File.WriteAllText(caleFisier, csvContent, System.Text.Encoding.UTF8);

                    Console.WriteLine($"✓ Raport Excel (CSV) generat la: {caleFisier}");
                    Console.WriteLine("Acest fișier poate fi deschis în Microsoft Excel.");
                }
            }
        }

        /// <summary>
        /// Escape pentru caractere speciale în CSV
        /// </summary>
        private string EscapeCsv(string valoare)
        {
            if (valoare.Contains(",") || valoare.Contains("\"") || valoare.Contains("\n"))
            {
                return "\"" + valoare.Replace("\"", "\"\"") + "\"";
            }
            return valoare;
        }

        /// <summary>
        /// Returnează numele zilei din număr
        /// </summary>
        private string GetZiuaNume(int zi)
        {
            string[] zile = { "Luni", "Marți", "Miercuri", "Joi", "Vineri", "Sâmbătă", "Duminică" };
            return zile[zi - 1];
        }
    }
}
