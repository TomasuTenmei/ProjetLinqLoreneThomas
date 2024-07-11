using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public static void Main()
    {
        // Charger les données depuis people.json
        string json;
        using (StreamReader reader = new StreamReader("people.json"))
        {
            json = reader.ReadToEnd();
        }

        // Désérialiser le JSON en objets C#
        var jsonData = JsonSerializer.Deserialize<PeopleData>(json);

        // Maintenant, vous pouvez interroger les différentes parties de jsonData
        // par exemple, les personnages (characters), les maisons (houses), etc.

        // Exemple de recherche avec LINQ sur les personnages
        var gryffindorCharacters = jsonData.Characters
            .Where(c => c.House == "Gryffindor");

        Console.WriteLine("Gryffindor Characters:");
        foreach (var character in gryffindorCharacters)
        {
            Console.WriteLine($"Name: {character.Name}, Birthdate: {character.Birthdate}, Patronus: {character.Patronus}");
        }

        // Exemple de recherche avec LINQ sur les maisons
        var slytherinTraits = jsonData.Houses
            .Where(h => h.Name == "Slytherin")
            .Select(h => h.Traits);

        Console.WriteLine("\nSlytherin Traits:");
        foreach (var traits in slytherinTraits)
        {
            foreach (var trait in traits)
            {
                Console.WriteLine(trait);
            }
        }

        // Exemple de recherche avec LINQ sur les objets magiques
        var invisibilityCloakOwners = jsonData.MagicalObjects
            .FirstOrDefault(o => o.Name == "Invisibility Cloak")
            .Owner;

        Console.WriteLine("\nInvisibility Cloak Owners:");
        Console.WriteLine(invisibilityCloakOwners);

        // Exemple de recherche avec LINQ sur les sorts
        var charmSpells = jsonData.Spells
            .Where(s => s.Type == "Charm");

        Console.WriteLine("\nCharm Spells:");
        foreach (var spell in charmSpells)
        {
            Console.WriteLine($"Spell: {spell.Name}, Effect: {spell.Effect}");
        }
    }
}

// Définition des classes correspondant à la structure JSON

public class PeopleData
{
    public List<Character> Characters { get; set; }
    public List<House> Houses { get; set; }
    public List<Spell> Spells { get; set; }
    public List<MagicalObject> MagicalObjects { get; set; }
}

public class Character
{
    public string Name { get; set; }
    public string House { get; set; }
    public DateTime Birthdate { get; set; }
    public string BloodStatus { get; set; }
    public string Patronus { get; set; }
    public Wand Wand { get; set; }
}

public class House
{
    public string Name { get; set; }
    public string Founder { get; set; }
    public string Ghost { get; set; }
    public List<string> Traits { get; set; }
    public List<string> Colors { get; set; }
}

public class Spell
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Effect { get; set; }
}

public class Wand
{
    public string Wood { get; set; }
    public string Core { get; set; }
    public string Length { get; set; }
}

public class MagicalObject
{
    public string Name { get; set; }
    public string Owner { get; set; }
    public List<string> Owners { get; set; } // Pour le cas où il y a plusieurs propriétaires
    public string Description { get; set; }
}
