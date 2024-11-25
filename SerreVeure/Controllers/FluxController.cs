using Microsoft.AspNetCore.Mvc;
using SerreVeure.Data;
using SerreVeure.Model;

namespace SerreVeure.Controllers
{

    namespace SerreVeure.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class FluxController : ControllerBase
        {
            [HttpGet("GetAllCmds")]
            public ActionResult<List<Commande>> RecupererCommandes()
            {
                using (CommandeData data = new CommandeData())
                    return data.GetAll();
            }

            [HttpGet("GetAllprdts")]
            public ActionResult<List<Produit>> RecupererProduits()
            {
                using (ProduitData data = new ProduitData())
                    return data.GetAll();
            }

            [HttpGet("GetprdtByRef/{reference}")]
            public ActionResult<ProduitComplet> RecupererProduitCompletParRef(string reference)
            {
                using (CommandeData cmdeData = new CommandeData())
                using (ProduitData prdtData = new ProduitData())
                {
                    Produit prdt = prdtData.GetProduct(reference);

                    if (prdt == null)
                        return null;
                    else
                    {
                        ProduitComplet prdtC = new ProduitComplet() { Libelle = prdt.Libelle, Reference = prdt.Reference };
                        prdtC.Commandes = cmdeData.GetProductCommand(reference);

                        return prdtC;
                    }
                }
            }

            [HttpPost("RecieveCommand")]
            public void RecieveCommand([FromBody] Commande command)
            {
                try
                {
                    using (CommandeData cmdeData = new CommandeData())
                    using (ProduitData prdtData = new ProduitData())
                    {
                        prdtData.CreateProduct(command.ReferenceProduit, command.LibelleProduit);
                        cmdeData.CreateCommand(command.ReferenceProduit, command.LibelleProduit, command.Quantite);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

    }

}
