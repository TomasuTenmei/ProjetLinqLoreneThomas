using System;
using System.Collections.Generic;
using System.Globalization;
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

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nVeuillez entrer ce que vous cherchez : ");
            Console.ResetColor();
            string keyword = Console.ReadLine();

            // Demander les options de l'utilisateur
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nSouhaitez-vous une recherche sensible à la casse (Match Case) ? (y/n) : ");
            Console.ResetColor();
            bool matchCase = Console.ReadLine().ToLower() == "y";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nSouhaitez-vous rechercher le mot entier (Match Whole Word) ? (y/n) : ");
            Console.ResetColor();
            bool matchWholeWord = Console.ReadLine().ToLower() == "y";

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nSouhaitez-vous effectuer une recherche distincte dans une liste spécifique ? (y/n) : ");
            Console.ResetColor();
            bool rechercheDistincte = Console.ReadLine().ToLower() == "y";

            if (rechercheDistincte)
            {
                // Afficher les listes disponibles dans le fichier JSON
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Listes disponibles dans le fichier JSON :");
                var jsonLists = ObtenirListesJson(jsonFilePath);
                foreach (var list in jsonLists)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(list);
                }

                // Afficher les listes disponibles dans le fichier XML
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Listes disponibles dans le fichier XML :");
                var xmlLists = ObtenirListesXml(xmlFilePath);
                foreach (var list in xmlLists)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(list);
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nDans quelle liste souhaitez-vous chercher : ");
                Console.ResetColor();
                string listeChoisie = Console.ReadLine();

                if (jsonLists.Contains(listeChoisie))
                {
                    var jsonResults = RechercherDansJson(jsonFilePath, keyword, matchCase, matchWholeWord, listeChoisie);
                    AfficherResultats(jsonResults, "liste JSON sélectionnée");
                }
                else if (xmlLists.Contains(listeChoisie))
                {
                    var xmlResults = RechercherDansXml(xmlFilePath, keyword, matchCase, matchWholeWord, listeChoisie);
                    AfficherResultats(xmlResults, "liste XML sélectionnée");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Liste non trouvée.");
                    Console.ResetColor();
                }
            }
            else
            {
                // Rechercher dans le fichier JSON
                var jsonResults = RechercherDansJson(jsonFilePath, keyword, matchCase, matchWholeWord);
                AfficherResultats(jsonResults, "fichier JSON");

                // Rechercher dans le fichier XML
                var xmlResults = RechercherDansXml(xmlFilePath, keyword, matchCase, matchWholeWord);
                AfficherResultats(xmlResults, "fichier XML");
            }
        }

        static List<string> ObtenirListesJson(string filePath)
        {
            string jsonContent = File.ReadAllText(filePath);
            JObject json = JObject.Parse(jsonContent);
            return json.Properties().Select(p => p.Name).ToList();
        }

        static List<string> ObtenirListesXml(string filePath)
        {
            XDocument xmlDoc = XDocument.Load(filePath);
            return xmlDoc.Root.Elements().Select(e => e.Name.LocalName).Distinct().ToList();
        }

        static List<string> RechercherDansJson(string filePath, string keyword, bool matchCase, bool matchWholeWord, string listeChoisie = null)
        {
            var results = new List<string>();
            string jsonContent = File.ReadAllText(filePath);
            JObject json = JObject.Parse(jsonContent);

            var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            IEnumerable<JToken> query;
            if (string.IsNullOrEmpty(listeChoisie))
            {
                query = json.DescendantsAndSelf().OfType<JValue>();
            }
            else
            {
                var selectedToken = json[listeChoisie];
                if (selectedToken is JContainer container)
                {
                    query = container.DescendantsAndSelf().OfType<JValue>();
                }
                else
                {
                    query = Enumerable.Empty<JValue>(); // Si la liste choisie n'est pas un conteneur, la requête est vide
                }
            }

            if (query != null)
            {
                var filteredQuery = query.Where(jv => jv.Type == JTokenType.String &&
                                               ContientMot(jv.Value<string>(), keyword, comparison, matchWholeWord))
                                  .Select(jv => jv.Path + ": " + jv.Value<string>());
                results.AddRange(filteredQuery);
            }

            return results;
        }

        static List<string> RechercherDansXml(string filePath, string keyword, bool matchCase, bool matchWholeWord, string listeChoisie = null)
        {
            var results = new List<string>();
            XDocument xmlDoc = XDocument.Load(filePath);

            var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            IEnumerable<XElement> query;
            if (string.IsNullOrEmpty(listeChoisie))
            {
                query = xmlDoc.Descendants();
            }
            else
            {
                query = xmlDoc.Descendants(listeChoisie);
            }

            var filteredQuery = query.Where(x => ContientMot(x.Value, keyword, comparison, matchWholeWord))
                                     .Select(x => x.Name + ": " + x.Value);

            results.AddRange(filteredQuery);

            return results;
        }

        static bool ContientMot(string source, string keyword, StringComparison comparison, bool matchWholeWord)
        {
            if (matchWholeWord)
            {
                var words = source.Split(new[] { ' ', '\t', '\r', '\n', ',', '.', ';', ':', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                return words.Any(word => string.Equals(word, keyword, comparison));
            }
            else
            {
                return source.IndexOf(keyword, comparison) >= 0;
            }
        }

        static void AfficherResultats(List<string> results, string source)
        {
            if (results.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nRésultats dans {source} :");
                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAucun résultat trouvé dans {source}.");
            }
            Console.ResetColor(); // Réinitialise la couleur par défaut de la console
        }
    }

    class SearchInTree
    {
        public static void Execute()
        {
            string jsonFilePath = Path.Combine("..", "..", "..", "data", "people.json");
            string xmlFilePath = Path.Combine("..", "..", "..", "data", "world.xml");

            // Afficher les listes disponibles dans le fichier JSON
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Listes disponibles dans le fichier JSON :");
            var jsonLists = ObtenirListesJson(jsonFilePath);
            foreach (var list in jsonLists)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(list);
            }

            // Afficher les listes disponibles dans le fichier XML
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Listes disponibles dans le fichier XML :");
            var xmlLists = ObtenirListesXml(xmlFilePath);
            foreach (var list in xmlLists)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(list);
            }

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nDans quelle liste souhaitez-vous chercher : ");
            Console.ResetColor();
            string listeChoisie = Console.ReadLine();

            // Vérifier si la liste choisie est dans le fichier JSON
            if (jsonLists.Contains(listeChoisie))
            {
                var keys = ObtenirClesDeListeJson(jsonFilePath, listeChoisie);
                AfficherCles(keys, "liste JSON");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nQuelle clé souhaitez-vous utiliser : ");
                Console.ResetColor();
                string keyChoisie = Console.ReadLine();

                if (keyChoisie == "birthdate")
                {
                    AfficherPersonnesSelonAge(jsonFilePath, listeChoisie);
                }
                else
                {
                    var valeurs = ObtenirValeursPourCle(jsonFilePath, listeChoisie, keyChoisie);
                    AfficherValeurs(valeurs);
                }
            }
            else if (xmlLists.Contains(listeChoisie))
            {
                var keys = ObtenirClesDeListeXml(xmlFilePath, listeChoisie);
                AfficherCles(keys, "liste XML");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("\nQuelle clé souhaitez-vous utiliser : ");
                Console.ResetColor();
                string keyChoisie = Console.ReadLine();

                var valeurs = ObtenirValeursPourCleXml(xmlFilePath, listeChoisie, keyChoisie);
                AfficherValeurs(valeurs);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Liste non trouvée.");
                Console.ResetColor();
            }
        }

        static List<string> ObtenirListesJson(string filePath)
        {
            string jsonContent = File.ReadAllText(filePath);
            JObject json = JObject.Parse(jsonContent);
            return json.Properties().Select(p => p.Name).ToList();
        }

        static List<string> ObtenirListesXml(string filePath)
        {
            XDocument xmlDoc = XDocument.Load(filePath);
            return xmlDoc.Root.Elements().Select(e => e.Name.LocalName).Distinct().ToList();
        }

        static List<string> ObtenirClesDeListeJson(string filePath, string listeChoisie)
        {
            string jsonContent = File.ReadAllText(filePath);
            JObject json = JObject.Parse(jsonContent);
            var selectedToken = json[listeChoisie];

            if (selectedToken is JArray jsonArray)
            {
                return jsonArray.Children<JObject>()
                                .SelectMany(obj => obj.Properties())
                                .Select(p => p.Name)
                                .Distinct()
                                .ToList();
            }
            else if (selectedToken is JObject jsonObject)
            {
                return jsonObject.Properties().Select(p => p.Name).ToList();
            }
            else
            {
                return new List<string> { "Aucune clé disponible" };
            }
        }

        static void AfficherCles(List<string> keys, string source)
        {
            if (keys.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nClés dans {source} sélectionnée :");
                foreach (var key in keys)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(key);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nAucune clé trouvée dans {source}.");
            }
            Console.ResetColor();
        }

        static void AfficherPersonnesSelonAge(string filePath, string listeChoisie)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nSouhaitez-vous afficher la liste de tout le monde, des personnes mineures ou des personnes majeures ? (all/min/maj) : ");
            Console.ResetColor();
            string choix = Console.ReadLine();

            string jsonContent = File.ReadAllText(filePath);
            JObject json = JObject.Parse(jsonContent);
            var selectedToken = json[listeChoisie];

            if (selectedToken is JArray jsonArray)
            {
                var personnes = jsonArray.Children<JObject>()
                                         .Select(obj => new
                                         {
                                             Nom = obj["name"]?.ToString(),
                                             Birthdate = DateTime.TryParseExact(
                                                 obj["birthdate"]?.ToString(),
                                                 "yyyy-MM-dd",
                                                 CultureInfo.InvariantCulture,
                                                 DateTimeStyles.None,
                                                 out DateTime date) ? date : (DateTime?)null
                                         })
                                         .Where(p => p.Birthdate.HasValue)
                                         .ToList();

                //string lowercaseChoice = choix.ToLower();
                //List<string> noms;
                //switch (lowercaseChoice)
                //{
                //    case "all":
                //        noms = personnes.Select(p => p.Nom).ToList();
                //        break;
                //    case "min":
                //        noms = personnes.Where(p => CalculerAge(p.Birthdate.Value) < 18).Select(p => p.Nom).ToList();
                //        break;
                //    case "maj":
                //        noms = personnes.Where(p => CalculerAge(p.Birthdate.Value) >= 18).Select(p => p.Nom).ToList();
                //        break;
                //    default:
                //        noms = new List<string>();
                //        break;
                //}

                List<string> noms = choix.ToLower() switch
                {
                    "all" => personnes.Select(p => p.Nom).ToList(),
                    "min" => personnes.Where(p => CalculerAge(p.Birthdate.Value) < 18).Select(p => p.Nom).ToList(),
                    "maj" => personnes.Where(p => CalculerAge(p.Birthdate.Value) >= 18).Select(p => p.Nom).ToList(),
                    _ => new List<string>()
                };

                AfficherNoms(noms);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Aucune donnée disponible.");
                Console.ResetColor();
            }
        }

        static int CalculerAge(DateTime birthdate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthdate.Year;
            if (birthdate.Date > today.AddYears(-age)) age--;
            return age;
        }

        static void AfficherNoms(List<string> noms)
        {
            if (noms.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nListe des personnes :");
                foreach (var nom in noms)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(nom);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nAucune personne trouvée pour le critère sélectionné.");
            }
            Console.ResetColor();
        }

        static List<string> ObtenirValeursPourCle(string filePath, string listeChoisie, string keyChoisie)
        {
            var jsonData = File.ReadAllText(filePath);
            var jsonObject = JObject.Parse(jsonData);
            
            // Navigate to the chosen list
            var liste = jsonObject[listeChoisie] as JArray;
            var valeurs = new HashSet<string>(); // Use HashSet to automatically handle duplicates

            if (liste != null)
            {
                foreach (var item in liste)
                {
                    var valeur = item[keyChoisie]?.ToString();
                    if (valeur != null)
                    {
                        valeurs.Add(valeur); // Add value to the HashSet
                    }
                }
            }

            return valeurs.ToList(); // Convert HashSet back to List for returning
        }

        static void AfficherValeurs(List<string> valeurs)
        {
            if (valeurs.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nValeurs pour la clé choisie :");
                foreach (var valeur in valeurs)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(valeur);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nAucune valeur trouvée pour la clé sélectionnée.");
            }
            Console.ResetColor();
        }

        static List<string> ObtenirValeursPourCleXml(string filePath, string listeChoisie, string keyChoisie)
        {
            var xmlDocument = XDocument.Load(filePath);
            var valeurs = new HashSet<string>(); // Use HashSet to automatically handle duplicates

            var elements = xmlDocument.Descendants(listeChoisie);

            foreach (var element in elements)
            {
                var valeur = element.Element(keyChoisie)?.Value;
                if (valeur != null)
                {
                    valeurs.Add(valeur); // Add value to the HashSet
                }
            }

            return valeurs.ToList(); // Convert HashSet back to List for returning
        }

        static List<string> ObtenirClesDeListeXml(string filePath, string listeChoisie)
        {
            var xmlDocument = XDocument.Load(filePath);
            var firstElement = xmlDocument.Descendants(listeChoisie).FirstOrDefault();
            var keys = new List<string>();

            if (firstElement != null)
            {
                foreach (var element in firstElement.Elements())
                {
                    keys.Add(element.Name.LocalName);
                }
            }

            return keys;
        }

    }
}
