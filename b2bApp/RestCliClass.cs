using System;
using System.Collections.Generic;

using System.Net;
using System.Json;

namespace b2bApp
{
    class clsRestCli
    {
        String urlBase = "";
        String url = "";
        WebClient client;
        String cacheDir = "";

        public clsRestCli(String cacheDir)
        {
            urlBase = "http://www.umbriaeqb2b.com/";
            url = urlBase + "rest/b2brest.php";
            client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            this.cacheDir = cacheDir;
        }

        public String Login(String user, String password)
        {
            try
            {
                JsonObject jobj = new JsonObject(new KeyValuePair<string, JsonValue>("username", user), new KeyValuePair<string, JsonValue>("password", password));
                string data = jobj.ToString();
                string reply = client.UploadString(url + "?op=login", data);
                JsonValue jobjRes = JsonObject.Parse(reply);
                string cod = jobjRes["codice"];
                string descr = jobjRes["descrizione"];
                if (cod == "0")
                {
                    string id_sess = jobjRes["user_id"];
                    return id_sess;
                }
            }
            catch
            {
                ;
            }
            return "";
        }

        public JsonArray CatAll(String id_sess)
        {
            JsonArray arrCat = new JsonArray();

            try
            {
                string reply_cat = client.DownloadString(url + "?op=catall&session_id=" + id_sess);
                JsonValue jobjRes = JsonObject.Parse(reply_cat);
                string cod2 = jobjRes["codice"];
                if (cod2 == "0")
                {
                    arrCat = (JsonArray) jobjRes["categorie"];
                }
            }
            catch
            {
                ;
            }

            return arrCat;
        }
/*
        public bool ArtAll(String id_sess)
        {
            try
            {
                client.DownloadStringAsync(new Uri(url + "?op=articoli/all&session_id=" + id_sess));
                client.DownloadStringCompleted += (object sender3, DownloadStringCompletedEventArgs e3) =>
                {
                    String reply_cat = e3.Result;

                    JSONObject jsobj2 = new JSONObject(reply_cat);
                    string cod2 = jsobj2.GetString("codice");
                    if (cod2 == "0")
                    {
                        JSONArray artArray = jsobj2.GetJSONArray("articoli");
                        ArticoliClass objArt = new ArticoliClass(cacheDir);
                        objArt.ScriviArticoli(artArray);
                    }
                };
            }
            catch
            {
                return false;
            }

            return true;
        }
*/
        public JsonArray ArtCat(String id_sess, String cat_padre)
        {
            JsonArray artArray = new JsonArray();
            try
            {
                string reply_art = client.DownloadString(url + "?op=articoli/cat&session_id=" + id_sess + "&cat_id=" + cat_padre);
                JsonValue jobjRes = JsonObject.Parse(reply_art);
                string cod_art = jobjRes["codice"];
                if (cod_art == "0")
                {
                    artArray = (JsonArray)jobjRes["articoli"];
                }
            } catch
            {
                ;
            }

            return artArray;
        }

        public JsonArray ArtKeyword(String id_sess, String keyword)
        {
            JsonArray artArray = new JsonArray();

            try
            {
                String reply_art = client.DownloadString(url + "?op=articoli/cat&session_id=" + id_sess + "&cat_id=0&keyword=" + keyword);
                JsonValue jsobj_art = JsonValue.Parse(reply_art);

                string cod_art = jsobj_art["codice"];
                string descr_art = jsobj_art["descrizione"];
                if (cod_art == "0")
                {
                    artArray = (JsonArray) jsobj_art["articoli"];
                }

            } catch
            {
                ;
            }

            return artArray;
        }

        public JsonValue ArtIdp(String id_sess, String idp, String sizeImg)
        {
            JsonValue dati= null;

            try
            {
                String reply_art = client.DownloadString(url + "?op=schedart&session_id=" + id_sess + "&idp=" + idp + "&size=" + sizeImg);
                JsonValue jsobj = JsonValue.Parse(reply_art);
                string cod = jsobj["codice"];
                string descr = jsobj["descrizione"];
                if (cod == "0")
                {
                    dati = jsobj["articolo"];
                }
            } catch
            {
                ;
            }

            return dati;
        }

        public byte[] ArtFoto(String foto)
        {
            var imageBytes = client.DownloadData(new Uri(urlBase + foto));

            return imageBytes;
        }

        public JsonArray ElencoFoto()
        {
            JsonArray arrFoto = new JsonArray();
            string reply_foto = client.DownloadString(urlBase + "rest/b2bsync.php?op=elencofoto&cartella=../tmp");
            JsonValue jsobj = JsonValue.Parse(reply_foto);
            string cod_art = jsobj["codice"];
            string descr_art = jsobj["descrizione"];
            if (cod_art == "0")
            {
                arrFoto = (JsonArray)jsobj["elenco_foto"];
            }

            return arrFoto;
        }

        public JsonArray SitOrdini(String id_sess)
        {
            JsonArray ordiniArray = new JsonArray();

            try
            {
                String reply_art = client.DownloadString(url + "?op=sitord&session_id=" + id_sess);
                JsonValue jsobj_art = JsonValue.Parse(reply_art);

                string cod_art = jsobj_art["codice"];
                string descr_art = jsobj_art["descrizione"];
                if (cod_art == "0")
                {
                    ordiniArray = (JsonArray)jsobj_art["ordini"];
                }

            }
            catch
            {
                ;
            }

            return ordiniArray;
        }

        public JsonArray DettOrdini(String id_sess, String datadoc, String numdoc)
        {
            JsonArray ordiniArray = new JsonArray();

            try
            {
                String reply_art = client.DownloadString(url + "?op=dettord&session_id=" + id_sess + "&datadoc=" + datadoc + "&numdoc=" + numdoc);
                JsonValue jsobj_art = JsonValue.Parse(reply_art);

                string cod_art = jsobj_art["codice"];
                string descr_art = jsobj_art["descrizione"];
                if (cod_art == "0")
                {
                    ordiniArray = (JsonArray)jsobj_art["righe_ordine"];
                }

            }
            catch
            {
                ;
            }

            return ordiniArray;
        }

        public String InfoCli(String id_sess)
        {
            JsonValue dati = "";

            try
            {
                String reply_art = client.DownloadString(url + "?op=infocli&session_id=" + id_sess);
                JsonValue jsobj = JsonValue.Parse(reply_art);
                string cod = jsobj["codice"];
                string descr = jsobj["descrizione"];
                if (cod == "0")
                {
                    dati = jsobj["ragsoc"];
                }
            }
            catch
            {
                ;
            }

            return dati;
        }

        public int InviaOrd(String id_sess, JsonArray carrello, String Note)
        {
            try
            {
                string data = carrello.ToString();
                string reply = client.UploadString(url + "?op=inviaord&session_id=" + id_sess + "&note=" + Note, data);
                JsonValue jobjRes = JsonObject.Parse(reply);
                string cod = jobjRes["codice"];
                string descr = jobjRes["descrizione"];
                if (cod == "0")
                {
                    int id_ord = jobjRes["ord_id"];

                    return id_ord;
                }
            }
            catch
            {
                ;
            }
            return 0;
        }
    }
}