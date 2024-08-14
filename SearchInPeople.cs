using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ProjetLinqLoreneThomas
{
    class SearchInPeople
    {
        public class Person
        {
            public string Name { get; set; }
            public DateTime Birthdate { get; set; }
        }

        public List<Person> ReadJson(string filePath)
        {
            string json = File.ReadAllText(filePath);
            List<Person> people = JsonConvert.DeserializeObject<List<Person>>(json);
            return people;
        }

        public static void Execute()
        {
            Console.WriteLine("Veuillez entrer le chemin complet du fichier :");
        }
    }
}
