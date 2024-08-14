using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace ProjetLinqLoreneThomas
{
    class SearchInAllData
    {
        public static void Execute()
        {
            string jsonFilePath = Path.Combine("..", "..", "..", "data", "people.json");
            string xmlFilePath = Path.Combine("..", "..", "..", "data", "world.xml");
            Console.WriteLine("Veuillez entrer ce que vous cherchez :");
            string keyword = Console.ReadLine();

            // Demander les options de l'utilisateur
            Console.WriteLine("Souhaitez-vous une recherche sensible à la casse (Match Case) ? (y/n) :");
            bool matchCase = Console.ReadLine().ToLower() == "y";

            Console.WriteLine("Souhaitez-vous rechercher le mot entier (Match Whole Word) ? (y/n) :");
            bool matchWholeWord = Console.ReadLine().ToLower() == "y";

            // Rechercher dans le fichier JSON
            var jsonResults = RechercherDansJson(jsonFilePath, keyword, matchCase, matchWholeWord);
            Console.WriteLine("Résultats dans le fichier JSON :");
            foreach (var result in jsonResults)
            {
                Console.WriteLine(result);
            }

            // Rechercher dans le fichier XML
            var xmlResults = RechercherDansXml(xmlFilePath, keyword, matchCase, matchWholeWord);
            Console.WriteLine("\nRésultats dans le fichier XML :");
            foreach (var result in xmlResults)
            {
                Console.WriteLine(result);
            }

            static List<string> RechercherDansJson(string filePath, string keyword, bool matchCase, bool matchWholeWord)
            {
                var results = new List<string>();

                // Charger le fichier JSON
                string jsonContent = File.ReadAllText(filePath);
                JObject json = JObject.Parse(jsonContent);

                // Préparer le mot-clé selon les options
                var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                // Utiliser LINQ pour rechercher le mot-clé dans le JSON
                var query = json.DescendantsAndSelf()
                                .OfType<JValue>()
                                .Where(jv => jv.Type == JTokenType.String &&
                                    ContientMot(jv.Value<string>(), keyword, comparison, matchWholeWord))
                                .Select(jv => jv.Path + ": " + jv.Value<string>());

                results.AddRange(query);

                return results;
            }

            static List<string> RechercherDansXml(string filePath, string keyword, bool matchCase, bool matchWholeWord)
            {
                var results = new List<string>();

                // Charger le fichier XML
                XDocument xmlDoc = XDocument.Load(filePath);

                // Préparer le mot-clé selon les options
                var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                // Utiliser LINQ pour rechercher le mot-clé dans le XML
                var query = xmlDoc.Descendants()
                                .Where(x => ContientMot(x.Value, keyword, comparison, matchWholeWord))
                                .Select(x => x.Name + ": " + x.Value);

                results.AddRange(query);

                return results;
            }

            static bool ContientMot(string source, string keyword, StringComparison comparison, bool matchWholeWord)
            {
                if (matchWholeWord)
                {
                    // Séparer les mots et vérifier s'il y a une correspondance exacte
                    var words = source.Split(new[] { ' ', '\t', '\r', '\n', ',', '.', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                    return words.Any(word => string.Equals(word, keyword, comparison));
                }
                else
                {
                    // Rechercher le mot-clé dans la chaîne
                    return source.IndexOf(keyword, comparison) >= 0;
                }
            }
        }
    }
}

