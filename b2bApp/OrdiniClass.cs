using System;
using System.Collections.Generic;
using System.Linq;
using System.Json;
using System.IO;

namespace b2bApp
{
    class OrdiniClass
    {
        String cacheDir = "";
        String filename = "";
        String fileragsoc = "";

        public OrdiniClass(String cacheDir)
        {
            this.cacheDir = cacheDir;
            filename = System.IO.Path.Combine(cacheDir, "b2bAppCacheO.json");
            fileragsoc = System.IO.Path.Combine(cacheDir, "b2bAppCacheR.json");
        }

        public bool EliminaCache()
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            if (File.Exists(fileragsoc))
            {
                File.Delete(fileragsoc);
            }

            return true;

        }

        public List<Tuple<string, string, string, string>> ElencoOrdini(String id_sess)
        {
            List<Tuple<string, string, string, string>> items = new List<Tuple<string, string, string, string>>();

            try
            {
                JsonArray ordArray = new JsonArray();
                if (File.Exists(filename))
                {
                    String ordini = File.ReadAllText(filename);
                    ordArray = (JsonArray)JsonArray.Parse(ordini);
                }
                else
                {
                    clsRestCli RestScli = new clsRestCli(cacheDir);
                    ordArray = RestScli.SitOrdini(id_sess);
                    File.WriteAllText(filename, ordArray.ToString());
                }

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

        public String RagSocCli(String id_sess)
        {
            String RagSoc = "";

            try
            {
                if (File.Exists(fileragsoc))
                {
                    RagSoc = File.ReadAllText(fileragsoc);
                }
                else
                {
                    clsRestCli RestScli = new clsRestCli(cacheDir);
                    RagSoc = RestScli.InfoCli(id_sess);
                    File.WriteAllText(fileragsoc, RagSoc);
                }
            }
            catch
            {
                ;
            }

            return RagSoc;
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