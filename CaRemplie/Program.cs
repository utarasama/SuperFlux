using System;
using System.IO;
using System.Collections.Generic;

using Stokomani.Frmwk.Web;
using SerreVeure.Model;
using System.Globalization;

namespace CaRemplie
{
    public static class Program
    {
        private static readonly int NombreColAttendu = 4;

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
                cnx.Post("http://localhost:59166/api/flux/RecieveCommand", new Commande() {
                    ReferenceProduit = commande.Reference,
                    LibelleProduit = commande.Libelle,
                    Quantite = commande.Quantite,
                    DateLivraison = commande.DateLivraison })
                .Wait();
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

                                if (elements.Length == NombreColAttendu)
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
                                        Console.WriteLine($"Erreur dans la lecture de la quantité pour la référence '{cmde.Reference}'");

                                    cmde.DateLivraison = elements[3];
                                }
                            }
                        }
                        else
                            Console.WriteLine($"Fichier '{file}' vide");
                        string todoPath = Path.GetFullPath("Work\\Todo");
                        string donePath = Path.GetFullPath("Work\\Done");
                        string fileName = Path.GetFileName(file);
                        string destinationFile = Path.Combine(donePath, fileName);
                        string sourceFile = Path.Combine(todoPath, fileName);
                        File.Move(sourceFile, destinationFile, overwrite: true);
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
        public string DateLivraison { get; set; }
    }
}
