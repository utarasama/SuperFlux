﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaLit.Data;
using SerreVeure.Model;
using Stokomani.Frmwk.Xaml;

namespace CaLit.Model
{
    public class AccueilViewModel : NotifyerObject
    {
        #region Private members

        private string erreur = null;
        private List<Produit> produits = null;
        private Produit selectedProduit = null;
        private ProduitComplet produitComplet = null;
        private bool isLoading = false;

        #endregion

        public AccueilViewModel()
        {
            ChargerCommand = new AsyncRelayCommand(OnChargerCommand);
            ChargerProduitCommand = new AsyncRelayCommand(OnChargerProduitCommand);
        }

        #region Properties

        public AsyncRelayCommand ChargerCommand { get; private set; }
        public AsyncRelayCommand ChargerProduitCommand { get; private set; }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    this.Notify(m => m.IsLoading);
                }
            }
        }

        public bool AfficherProduits { get { return Produits != null && Produits.Count > 0; } }

        public List<Produit> Produits
        {
            get { return produits; }
            set
            {
                if (produits != value)
                {
                    produits = value;
                    this.Notify(m => m.Produits);
                    this.Notify(m => m.AfficherProduits);
                }
            }
        }

        public Produit SelectedProduit
        {
            get { return selectedProduit; }
            set
            {
                if (selectedProduit != value)
                {
                    selectedProduit = value;
                    this.Notify(m => m.SelectedProduit);
                    this.Notify(m => m.ProduitEstSelectionne);
                }
            }
        }
        public bool ProduitEstSelectionne { get { return SelectedProduit != null; } }

        public ProduitComplet ProduitComplet
        {
            get { return produitComplet; }
            set
            {
                if (produitComplet != value)
                {
                    produitComplet = value;
                    this.Notify(m => m.ProduitComplet);
                    this.Notify(m => m.AfficherProduitComplet);
                }
            }
        }

        public bool AfficherProduitComplet { get { return ProduitComplet != null; } }

        public bool AfficherErreur { get { return !string.IsNullOrEmpty(Erreur); } }

        public string Erreur
        {
            get { return erreur; }
            set
            {
                if (erreur != value)
                {
                    erreur = value;
                    this.Notify(m => m.Erreur);
                    this.Notify(m => m.AfficherErreur);
                }
            }
        }

        #endregion

        #region Helpers

        private async Task OnChargerCommand(object o)
        {
            IsLoading = true;
            Erreur = null;
            try
            {
                ProduitComplet = null;
                Produits = await ProduitData.GetAll();
            }
            catch (Exception ex)
            {
                Erreur = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }

        }

        private async Task OnChargerProduitCommand(object o)
        {
            if (SelectedProduit != null)
            {
                ProduitComplet = null;
                IsLoading = true;
                Erreur = null;
                try
                {
                    ProduitComplet = await ProduitData.Get(SelectedProduit.Reference);
                }
                catch (Exception ex)
                {
                    Erreur = ex.Message;
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        #endregion
    }
}
