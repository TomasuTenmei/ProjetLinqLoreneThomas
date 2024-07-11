using System.Xml.Linq;
using Newtonsoft.Json;

namespace ProjetLinqLoreneThomas
{
    class ConvXmlToJson
    {
        public static void Execute()
        {
            Console.WriteLine("Veuillez entrer le chemin complet du fichier :");

            string filePath = Console.ReadLine();
            string directoryPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            // Vérifier si le chemin de fichier est valide
            if (System.IO.File.Exists(filePath))
            {
                // Load the XML file
                XDocument xmlDocument = XDocument.Load(filePath);

                // Convert XML to JSON
                string json = JsonConvert.SerializeXNode(xmlDocument);

                // Save the JSON to a file
                string savePath = Path.Combine(directoryPath, fileName + ".json");
                System.IO.File.WriteAllText(savePath, json);
                Console.WriteLine("\nFin de la convertion\n");
            }
            else
            {
                Console.WriteLine("\nLe chemin du fichier n'est pas valide ou le fichier n'existe pas.\n");
            }
        }
    }
}
