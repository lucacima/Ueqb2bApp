using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;

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

        public List<Tuple<string, string, string, string>> RigheCarrello()
        {
            List<Tuple<string, string, string, string>> carts = new List<Tuple<string, string, string, string>>();

            if (File.Exists(filename))
            {
                var streamReader = new StreamReader(filename);

                while (!streamReader.EndOfStream)
                {
                    String riga = streamReader.ReadLine();
                    String[] col = riga.Split(';');
                    carts.Add(new Tuple<string, string, string, string>(col[0], col[1], col[2], col[3]));
                }
                streamReader.Close();
            }
            return carts;
        }

        public Tuple<string, string, string, string> RigaCarrello(int riga)
        {
            List<Tuple<string, string, string, string>> carts = RigheCarrello();

            return carts[riga-1];
        }

        public bool AggiungiCarrello(String idp, String nome, String Qta, String Note)
        {
            var streamWriter = new StreamWriter(filename, true);
            streamWriter.WriteLine(idp + ";" + nome + ";" + Qta + ";" + Note.Replace(";", ""));
            streamWriter.Close();
            return true;
        }

        public bool AggiornaCarrello(int riga, String Qta, String Note)
            
        {

            List<Tuple<string,string, string, string>> carts = RigheCarrello();
            String idp = carts[riga].Item1;
            String nome = carts[riga].Item2;
            carts[riga-1]= new Tuple<string,string, string, string>(idp,nome, Qta, Note);

            return ScriviCarrello(carts);
        }

        public bool EliminaCarrello(int riga)
        {

            List<Tuple<string, string, string, string>> carts = RigheCarrello();
            carts.RemoveAt(riga-1);

            return ScriviCarrello(carts);
        }

        private bool ScriviCarrello(List<Tuple<string, string, string,string>> carts)
        {
            var streamWriter = new StreamWriter(filename, false);
            for (int i = 0; i < carts.Count; i++)
            {
                streamWriter.WriteLine(carts[i].Item1 + ";" + carts[i].Item2 + ";" + carts[i].Item3 + ";" + carts[i].Item4);
            }
            streamWriter.Close();

            return true;
        }

    }
}