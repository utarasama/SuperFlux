using System;
using System.IO;
using System.Collections.Generic;

using Stokomani.Frmwk.Web;
using SerreVeure.Model;

namespace CaRemplie
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chargement des commandes...");

            List<ReceiveCommand> commandes = LoadCommands();

            Console.WriteLine("Création des commandes...");
            if (commandes != null)
                foreach (var cmde in commandes)
                    CreateCommand(cmde);
        }

        private static void CreateCommand(ReceiveCommand commande)
        {
            Console.WriteLine($"Création des commande {commande.Reference}...");
            using (HttpConnector cnx = new HttpConnector())
                cnx.Post("http://localhost:59166/api/flux/RecieveCommand", new Commande() { ReferenceProduit = commande.Reference, LibelleProduit = commande.Libelle, Quantite = commande.Quantite }).Wait();
        }

        private static List<ReceiveCommand> LoadCommands()
        {
            List<ReceiveCommand> commandes = new List<ReceiveCommand>();

            if (Directory.Exists(@"Work\Todo"))
            {
                string[] files = Directory.GetFiles(@"Work\Todo");

                if (files != null)
                {
                    foreach (string file in files)
                    {
                        string[] lines = File.ReadAllLines(file);

                        if (lines != null)
                        {
                            foreach (string line in lines)
                            {
                                string[] elements = line.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                                if (elements.Length == 3)
                                {
                                    ReceiveCommand cmde = new ReceiveCommand();
                                    cmde.Reference = elements[0];
                                    cmde.Libelle = elements[1];

                                    if (int.TryParse(elements[2], out int qte))
                                    {
                                        cmde.Quantite = qte;
                                        commandes.Add(cmde);
                                    }
                                    else
                                        Console.WriteLine($"Erreur dans la lecture de la quantité pour la refence '{cmde.Reference}'");
                                }
                            }
                        }
                        else
                            Console.WriteLine($"Fichier '{file}' vide");
                        File.Move(@"Work\Todo\" + file, @"Work\Done\" + file);
                    }
                }
                else
                    Console.WriteLine("Dossier 'todo' vide");
            }
            else
                Console.WriteLine("Pas de dossier 'todo'");

            return commandes;
        }
    }

    internal class ReceiveCommand
    {
        public string Reference { get; set; }
        public string Libelle { get; set; }
        public int Quantite { get; set; }
    }
}
