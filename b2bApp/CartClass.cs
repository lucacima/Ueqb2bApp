using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System.IO;
using System.Json;

namespace b2bApp
{
    class clsCart
    {
        String cacheDir = "";
        String id_sess = "";
        string filename = "";

        public clsCart(String cacheDir, String id_sess)
        {
            this.cacheDir = cacheDir;
            this.id_sess = id_sess;
            filename = System.IO.Path.Combine(cacheDir, "cart_" + id_sess + ".json");
        }

        public JsonArray RigheCarrello()
        {
            JsonArray arrRighe = new JsonArray();
            if (File.Exists(filename))
            {
                String righe = File.ReadAllText(filename);
                arrRighe = (JsonArray)JsonArray.Parse(righe);
            }

            return arrRighe;
        }

        public List<Tuple<string, string, string, string, string, string>> ListaCarrello()
        {
            List<Tuple<string, string, string, string, string, string>> righe = new List<Tuple<string, string, string, string, string, string>>();

            JsonArray arrRighe = RigheCarrello();
            for (int i = 0; i < arrRighe.Count; i++)
            {
                righe.Add(new Tuple<string, string, string, string, string, string>(arrRighe[i]["idp"], arrRighe[i]["codice"], arrRighe[i]["nome"], arrRighe[i]["Qta"], arrRighe[i]["Prezzo"], arrRighe[i]["Note"]));
            }

            return righe;
        }

        public JsonValue RigaCarrello(int riga)
        {
            JsonArray arrRighe = RigheCarrello();

            return arrRighe[riga-1];
        }

        public bool AggiungiCarrello(String idp, String codice, String nome, String Qta, String Prezzo, String Note)
        {
            JsonObject riga = new JsonObject(new KeyValuePair<string, JsonValue>("idp", idp), new KeyValuePair<string, JsonValue>("codice", codice), new KeyValuePair<string, JsonValue>("nome", nome), new KeyValuePair<string, JsonValue>("Qta", Qta), new KeyValuePair<string, JsonValue>("Prezzo", Prezzo), new KeyValuePair<string, JsonValue>("Note", Note));
            JsonArray arrRighe = RigheCarrello();
            arrRighe.Add(riga);

            return ScriviCarrello(arrRighe);
        }

        public bool AggiornaCarrello(int riga, String Qta, String Note)
            
        {
            JsonArray arrRighe = RigheCarrello();
            arrRighe[riga - 1]["Qta"] = Qta;
            arrRighe[riga - 1]["Note"] = Note;

            return ScriviCarrello(arrRighe);
        }

        public bool EliminaCarrello(int riga)
        {
            JsonArray arrRighe = RigheCarrello();
            arrRighe.RemoveAt(riga - 1);

            return ScriviCarrello(arrRighe);
        }

        private bool ScriviCarrello(JsonArray carts)
        {
            File.WriteAllText(filename, carts.ToString());

            return true;
        }

        public int InviaOrdine(String Note)
        {
            ArticoliClass objArt = new ArticoliClass(cacheDir);
            JsonArray carts = RigheCarrello();
            JsonArray arrOrdini = new JsonArray();

            for (int i = 0; i < carts.Count; i++)
            {
                JsonValue datiArt= objArt.DatiArticolo(carts[i]["idp"]);
                JsonObject riga_ord = new JsonObject();
                riga_ord.Add("codart", carts[i]["codice"]);
                riga_ord.Add("qta", carts[i]["Qta"]);
                riga_ord.Add("note", carts[i]["Note"]);
                riga_ord.Add("prezzo", datiArt["prezzo_lordo"]);
                riga_ord.Add("sconto1", datiArt["sconto"]);

                arrOrdini.Add(riga_ord);
            }
            clsRestCli objRestCli = new clsRestCli(cacheDir);
            int id_ord = objRestCli.InviaOrd(id_sess, arrOrdini, Note);

            // Elimino righe carrello
            File.Delete(filename);

            return id_ord;

        }

    }
}