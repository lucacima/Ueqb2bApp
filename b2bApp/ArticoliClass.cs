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
using Org.Json;
using System.IO;
using Android.Graphics;

namespace b2bApp
{
    class ArticoliClass
    {
        String cacheDir = "";
        String prefixCat = "b2bAppCacheC";
        String prefixArt = "b2bAppCacheA";
        String prefixFoto = "b2bAppCacheF";

        public ArticoliClass(String cacheDir)
        {
            this.cacheDir = cacheDir;
        }
        
        public bool ScriviCategorie(JSONArray catArray)
        {
            try
            {
                //Elimino file esistenti
                string[] filesCat = Directory.GetFiles(cacheDir, prefixCat + "*.txt");
                foreach (string fileCat in filesCat)
                {
                    File.Delete(fileCat);
                }

                String filename = System.IO.Path.Combine(cacheDir, prefixCat + "0.txt");
                StreamWriter streamWriter = new StreamWriter(filename, false);
                String cat_padre = "0";

                for (int i = 0; i < catArray.Length(); i++)
                {
                    JSONObject item = (JSONObject)catArray.Get(i);
                    String catp = item.GetString("cat_padre");
                    if (catp != cat_padre)
                    {
                        streamWriter.Close();
                        filename = System.IO.Path.Combine(cacheDir, prefixCat + catp + ".txt");
                        streamWriter = new StreamWriter(filename, false);
                        cat_padre = catp;
                    }
                    String idc = item.GetString("category_id");
                    String categ = item.GetString("categoria");
                    streamWriter.WriteLine(idc + "#" + categ);
                }
                streamWriter.Close();

                return true;
            } catch
            {
                return false;
            }
        }

        public bool ScriviArticoli(JSONArray artArray)
        {
            try
            {
                //Elimino file esistenti
                string[] filesArt = Directory.GetFiles(cacheDir, prefixArt + "*.txt");
                foreach (string fileArt in filesArt)
                {
                    File.Delete(fileArt);
                }

                String cat_padre = "0";
                String filename = System.IO.Path.Combine(cacheDir, prefixArt + "0.txt");
                StreamWriter streamWriter = new StreamWriter(filename, false);


                for (int i = 0; i < artArray.Length(); i++)
                {
                    JSONObject item = (JSONObject)artArray.Get(i);
                    String catp = item.GetString("category_id");
                    if (catp != cat_padre)
                    {
                        streamWriter.Close();
                        filename = System.IO.Path.Combine(cacheDir, prefixArt + catp + ".txt");
                        streamWriter = new StreamWriter(filename, false);
                        cat_padre = catp;
                    }
                    String idp = item.GetString("product_id");
                    String nome = item.GetString("nome");
                    streamWriter.WriteLine(idp + "#" + nome);
                }
                streamWriter.Close();

                return true;
            } catch
            {
                return false;
            }
        }

        public List<Tuple<string, string, int>> ElencoCategorie(String cat_padre)
        {
            List<Tuple<string, string, int>> cats = new List<Tuple<string, string, int>>();

            try
            {
                string filename = System.IO.Path.Combine(cacheDir, prefixCat + cat_padre + ".txt");
                if (File.Exists(filename))
                {
                    var streamReader = new StreamReader(filename);

                    while (!streamReader.EndOfStream)
                    {
                        String riga = streamReader.ReadLine();
                        String[] col = riga.Split('#');
                        cats.Add(new Tuple<string, string, int>(col[0], col[1], Resource.Drawable.freccia));
                    }
                    streamReader.Close();
                }
            }
            catch
            {
                ;
            }
            return cats;
        }

        public List<Tuple<string, string, int>> ElencoArticoli(String cat_padre)
        {
            List<Tuple<string, string, int>> items = new List<Tuple<string,string, int>>();

            try
            {
                string filename = System.IO.Path.Combine(cacheDir, prefixArt + cat_padre + ".txt");
                if (File.Exists(filename))
                {
                    var streamReader = new StreamReader(filename);

                    while (!streamReader.EndOfStream)
                    {
                        String riga = streamReader.ReadLine();
                        String[] col = riga.Split('#');
                        items.Add(new Tuple<string, string, int>(col[0], col[1], Resource.Drawable.cart));
                    }
                    streamReader.Close();
                }
            }
            catch
            {
                ;
            }

            return items;
        }

        public List<Tuple<string, string, int>> RicercaArticoli(String id_sess, String keyword)
        {
            List<Tuple<string, string, int>> items = new List<Tuple<string, string, int>>();

            try
            {
                clsRestCli RestScli = new clsRestCli(cacheDir);
                JSONArray artArray = RestScli.ArtKeyword(id_sess, keyword);

                for (int i = 0; i < artArray.Length(); i++)
                {
                    JSONObject item = (JSONObject)artArray.Get(i);
                    String idp = item.GetString("idp");
                    String prodotto = item.GetString("nome");
                    items.Add(new Tuple<string, string, int>(idp, prodotto, Resource.Drawable.cart));
                }
            }
            catch
            {
                ;
            }

            return items;
        }

        public Tuple<JSONObject, Bitmap> Articolo(String id_sess, String idp, String sizeImg)
        {
            Tuple<JSONObject, Bitmap> res = null;

            try
            {
                clsRestCli RestScli = new clsRestCli(cacheDir);
                JSONObject dati = RestScli.ArtIdp(id_sess, idp, sizeImg);
                String foto = dati.GetString("foto");

                if (foto != "")
                {
                    foto = foto.Replace("../", "");

                    bool scarica = false;
                    Bitmap imageBitmap = null;
                    byte[] imageBytes = null;

                    string filename = System.IO.Path.Combine(cacheDir, prefixFoto + idp + "_" + sizeImg + ".txt");
                    if (File.Exists(filename))
                    {
                        DateTime oggi = DateTime.Today;

                        if (File.GetCreationTime(filename).Date != oggi.Date)
                        {
                            scarica = true;
                        }
                        else
                        {
                            imageBytes = File.ReadAllBytes(filename);
                        }

                    }
                    else scarica = true;

                    if (scarica)
                    {
                        imageBytes = RestScli.ArtFoto(foto);
                        File.WriteAllBytes(filename, imageBytes);
                    }

                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);

                    }

                    res = new Tuple<JSONObject, Bitmap>(dati, imageBitmap);
                }
            }
            catch
            {
                ;
            }

            return res;
        }

    }
    }