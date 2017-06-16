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
            filename = System.IO.Path.Combine(cacheDir, "cart_" + id_sess + ".txt");
        }

        public List<Tuple<string, string, string, string, string>> RigheCarrello()
        {
            List<Tuple<string, string, string, string, string>> carts = new List<Tuple<string, string, string, string, string>>();

            if (File.Exists(filename))
            {
                var streamReader = new StreamReader(filename);

                while (!streamReader.EndOfStream)
                {
                    String riga = streamReader.ReadLine();
                    String[] col = riga.Split('#');
                    carts.Add(new Tuple<string, string, string, string, string>(col[0], col[1], col[2], col[3], col[4]));
                }
                streamReader.Close();
            }
            return carts;
        }

        public Tuple<string, string, string, string, string> RigaCarrello(int riga)
        {
            List<Tuple<string, string, string, string, string>> carts = RigheCarrello();

            return carts[riga-1];
        }

        public bool AggiungiCarrello(String idp, String nome, String Qta, String Prezzo, String Note)
        {
            var streamWriter = new StreamWriter(filename, true);
            streamWriter.WriteLine(idp + "#" + nome + "#" + Qta + "#" + Prezzo + "#" + Note.Replace("#", ""));
            streamWriter.Close();
            return true;
        }

        public bool AggiornaCarrello(int riga, String Qta, String Note)
            
        {

            List<Tuple<string,string, string, string, string>> carts = RigheCarrello();
            String idp = carts[riga-1].Item1;
            String nome = carts[riga-1].Item2;
            String prezzo= carts[riga - 1].Item4;
            carts[riga-1]= new Tuple<string,string, string, string, string>(idp,nome, Qta, prezzo, Note);

            return ScriviCarrello(carts);
        }

        public bool EliminaCarrello(int riga)
        {

            List<Tuple<string, string, string, string, string>> carts = RigheCarrello();
            carts.RemoveAt(riga-1);

            return ScriviCarrello(carts);
        }

        private bool ScriviCarrello(List<Tuple<string, string, string,string, string>> carts)
        {
            var streamWriter = new StreamWriter(filename, false);
            for (int i = 0; i < carts.Count; i++)
            {
                streamWriter.WriteLine(carts[i].Item1 + "#" + carts[i].Item2 + "#" + carts[i].Item3 + "#" + carts[i].Item4 + "#" + carts[i].Item5 + "#");
            }
            streamWriter.Close();

            return true;
        }

        public int InviaOrdine()
        {
            ArticoliClass objArt = new ArticoliClass(cacheDir);
            List<Tuple<string, string, string, string, string>> carts = RigheCarrello();
            JsonArray arrOrdini = new JsonArray();

            for (int i = 0; i < carts.Count; i++)
            {
                JsonValue datiArt= objArt.DatiArticolo(carts[i].Item1);
                JsonObject riga_ord = new JsonObject();
                riga_ord.Add("codart", datiArt["codice"]);
                riga_ord.Add("qta", carts[i].Item3);
                riga_ord.Add("note", carts[i].Item5.Replace("<br>","\n"));
                riga_ord.Add("prezzo", datiArt["prezzo_lordo"]);
                riga_ord.Add("sconto1", datiArt["sconto"]);

                arrOrdini.Add(riga_ord);
            }
            clsRestCli objRestCli = new clsRestCli(cacheDir);
            int id_ord = objRestCli.InviaOrd(id_sess, arrOrdini);

            // Elimino righe carrello
            File.Delete(filename);

            return id_ord;

        }

    }
}