namespace ProjetLinqLoreneThomas
{    
    class Program
    {
        static void Main(string[] args)
        {
            bool continuer = true;

            while (continuer)
            {

                int choix;

                // Affichage des options et demande de choix
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\nVeuillez choisir parmi les options suivantes :");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("1. Convertir un fichier Xml en Json");
                Console.WriteLine("2. Convertir un fichier Json en Xml");
                Console.WriteLine("3. Recherche un terme dans toutes les données");
                Console.WriteLine("4. Recherche dans l'arbre des données");
                Console.WriteLine("0. Quitter le programme\n");

                // Lecture de l'entrée de l'utilisateur
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Entrez votre choix : ");
                Console.ResetColor();
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
                        case 4:
                            SearchInTree.Execute();
                            break;
                        case 0:
                        Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Fin du programme\n");
                            continuer = false;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Choix invalide\n");
                            break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Choix invalide\n");
                }
            }
            Console.ResetColor();
        }
    }
}