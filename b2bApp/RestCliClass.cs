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
using System.Net;
using Org.Json;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;

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
            urlBase = "http://2.115.37.22/umbriaeq/";
            url = urlBase + "rest/b2brest.php";
            client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            this.cacheDir = cacheDir;
        }

        public String Login(String user, String password)
        {
            try
            {
                JSONObject jsobjIn = new JSONObject();
                jsobjIn.Put("username", user);
                jsobjIn.Put("password", password);

                string data = jsobjIn.ToString();
                string reply = client.UploadString(url + "?op=login", data);

                JSONObject jsobj = new JSONObject(reply);
                string cod = jsobj.GetString("codice");
                string descr = jsobj.GetString("descrizione");

                if (cod == "0")
                {
                    string id_sess = jsobj.GetString("user_id");
                    return id_sess;
                }
            }
            catch
            {
                ;
            }
            return "";
        }

        public bool CatAll(String id_sess)
        {
            try
            {
                string reply_cat = client.DownloadString(url + "?op=catall&session_id=" + id_sess);
                JSONObject jsobj2 = new JSONObject(reply_cat);
                string cod2 = jsobj2.GetString("codice");
                if (cod2 == "0")
                {
                    ArticoliClass objArt = new ArticoliClass(cacheDir);
                    objArt.ScriviCategorie(jsobj2.GetJSONArray("categorie"));

                    return true;
                }
            }
            catch
            {
                ;
            }

            return false;
        }

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

        public bool ArtCat(String id_sess, String cat_padre)
        {
            string reply_art = client.DownloadString(url + "?op=articoli/cat&session_id=" + id_sess + "&cat_id=" + cat_padre);
            JSONObject jsobj_art = new JSONObject(reply_art);
            string cod_art = jsobj_art.GetString("codice");
            string descr_art = jsobj_art.GetString("descrizione");
            if (cod_art == "0")
            {
                JSONArray artArray = jsobj_art.GetJSONArray("articoli");
                ArticoliClass objArt = new ArticoliClass(cacheDir);
                objArt.ScriviArticoli(artArray);
            }

            return true;
        }

        public JSONArray ArtKeyword(String id_sess, String keyword)
        {
            JSONArray artArray = new JSONArray();

            String reply_art = client.DownloadString(url + "?op=articoli/cat&session_id=" + id_sess + "&cat_id=0&keyword=" + keyword);
            JSONObject jsobj_art = new JSONObject(reply_art);
            string cod_art = jsobj_art.GetString("codice");
            string descr_art = jsobj_art.GetString("descrizione");
            if (cod_art == "0")
            {
                artArray = jsobj_art.GetJSONArray("articoli");
            }

            return artArray;
        }

        public JSONObject ArtIdp(String id_sess, String idp, String sizeImg)
        {
            JSONObject dati = new JSONObject();

            String reply_art = client.DownloadString(url + "?op=schedart&session_id=" + id_sess + "&idp=" + idp + "&size=" + sizeImg);
            JSONObject jsobj = new JSONObject(reply_art);
            string cod = jsobj.GetString("codice");
            string descr = jsobj.GetString("descrizione");
            if (cod == "0")
            {
                dati = jsobj.GetJSONObject("articolo");
            }

            return dati;
        }

        public byte[] ArtFoto(String foto)
        {
            var imageBytes = client.DownloadData(new Uri(urlBase + foto));

            return imageBytes;


        }

    }
    }