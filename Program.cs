namespace ProjetLinqLoreneThomas
{    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start\n");

            bool continuer = true;

            while (continuer)
            {

                int choix;

                // Affichage des options et demande de choix
                Console.WriteLine("Veuillez choisir parmi les options suivantes :");
                Console.WriteLine("1. Convertir un fichier Xml en Json");
                Console.WriteLine("2. Convertir un fichier Json en Xml");
                Console.WriteLine("3. Recherche dans toutes les données");
                Console.WriteLine("0. Quitter le programme\n");

                // Lecture de l'entrée de l'utilisateur
                Console.Write("Entrez votre choix : ");
                string input = Console.ReadLine();
                Console.Write("\n");

                if (int.TryParse(input, out choix))
                {

                    switch (choix)
                    {
                        case 1:
                            ConvXmlToJson.Execute();
                            break;
                        case 2:
                            ConvJsonToXml.Execute();
                            break;
                        case 3:
                            SearchInAllData.Execute();
                            break;
                        case 0:
                            Console.WriteLine("Fin du programme\n");
                            continuer = false;
                            break;
                        default:
                            Console.WriteLine("Choix invalide\n");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Choix invalide\n");
                }
            }
        }
    }
}