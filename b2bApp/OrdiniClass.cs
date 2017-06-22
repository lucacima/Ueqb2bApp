using System;
using System.Collections.Generic;
using System.Linq;
using System.Json;

using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace b2bApp
{
    class OrdiniClass
    {
        String cacheDir = "";


        public OrdiniClass(String cacheDir)
        {
            this.cacheDir = cacheDir;
        }

        public List<Tuple<string, string, string, string>> ElencoOrdini(String id_sess)
        {
            List<Tuple<string, string, string, string>> items = new List<Tuple<string, string, string, string>>();

            try
            {
                clsRestCli RestScli = new clsRestCli(cacheDir);
                JsonArray ordArray = RestScli.SitOrdini(id_sess);

                for (int i = 0; i < ordArray.Count(); i++)
                {
                    JsonValue item = ordArray[i];
                    items.Add(new Tuple<string, string, string, string>(item["DataDoc"], item["NumDoc"], string.Format("{0:N0}", (float) item["QtaT"]), item["TotO"]));                    
                }
            }
            catch
            {
                ;
            }

            return items;
        }

        public List<Tuple<string, string, string, string>> DettOrdine(String id_sess, String datadoc, String numdoc)
        {
            List<Tuple<string, string, string, string>> items = new List<Tuple<string, string, string, string>>();

            try
            {
                clsRestCli RestScli = new clsRestCli(cacheDir);
                JsonArray ordArray = RestScli.DettOrdini(id_sess,datadoc,numdoc);

                for (int i = 0; i < ordArray.Count(); i++)
                {
                    JsonValue item = ordArray[i];
                    String CodArt = item["CodArt"];
                    items.Add(new Tuple<string, string, string, string>(CodArt.Trim(), item["DesArt"], string.Format("{0:N0}", (float)item["Qta"]), item["ImportoIva"]));
                }
            }
            catch
            {
                ;
            }

            return items;
        }


    }
}