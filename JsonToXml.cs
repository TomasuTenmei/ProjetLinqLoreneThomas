using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProjetLinqLoreneThomas
{
    class ConvJsonToXml
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
                string json = System.IO.File.ReadAllText(filePath);

                // Convert JSON to JObject
                JObject jsonObject = JObject.Parse(json);

                // Convert JObject to XML
                XDocument xmlDocument = JsonConvert.DeserializeXNode(jsonObject.ToString(), "Root");

                // Save the JSON to a file
                string savePath = Path.Combine(directoryPath, fileName + ".xml");
                xmlDocument.Save(savePath);
                Console.WriteLine("\nFin de la convertion\n");
            }
            else
            {
                Console.WriteLine("\nLe chemin du fichier n'est pas valide ou le fichier n'existe pas.\n");
            }
        }
    }
}
