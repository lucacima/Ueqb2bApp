using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Android.Graphics;
using System.Json;

namespace b2bApp
{
    class ArticoliClass
    {
        String cacheDir = "";
        String prefixCat = "b2bAppCacheC";
        String prefixArt = "b2bAppCacheA";
        String prefixSch = "b2bAppCacheS";
        String prefixFoto = "b2bAppCacheF";

        public ArticoliClass(String cacheDir)
        {
            this.cacheDir = cacheDir;
        }
        
        public bool PrendiCategorie(String id_sess)
        {
            try
            {
                //Elimino file esistenti
                string[] filesCat = Directory.GetFiles(cacheDir, prefixCat + "*.txt");
                foreach (string fileCat in filesCat)
                {
                    File.Delete(fileCat);
                }

                clsRestCli RestScli = new clsRestCli(cacheDir);
                JsonArray catArray = RestScli.CatAll(id_sess);

                String filename = System.IO.Path.Combine(cacheDir, prefixCat + "0.txt");
                StreamWriter streamWriter = new StreamWriter(filename, false);
                String cat_padre = "0";

                for (int i = 0; i < catArray.Count(); i++)
                {
                    //JSONObject item = (JSONObject)catArray.Get(i);
                    JsonValue item = catArray[i];
                    String catp = item["cat_padre"];
                    if (catp != cat_padre)
                    {
                        streamWriter.Close();
                        filename = System.IO.Path.Combine(cacheDir, prefixCat + catp + ".txt");
                        streamWriter = new StreamWriter(filename, false);
                        cat_padre = catp;
                    }
                    String idc = item["category_id"];
                    String categ = item["categoria"];
                    streamWriter.WriteLine(idc + "#" + categ);
                }
                streamWriter.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EliminaCacheArticoli()
        {
            //Elimino file esistenti
            string[] filesArt = Directory.GetFiles(cacheDir, prefixArt + "*.txt");
            foreach (string fileArt in filesArt)
            {
                File.Delete(fileArt);
            }

            return true;
        }

/*
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
*/
        public bool ScriviArticoliCat(JsonArray artArray, String cat_padre)
        {
            try
            {
                String filename = System.IO.Path.Combine(cacheDir, prefixArt + cat_padre + ".txt");
                StreamWriter streamWriter = new StreamWriter(filename, false);

                for (int i = 0; i < artArray.Count(); i++)
                {
                    JsonValue item = artArray[i];
                    String idp = item["idp"];
                    String nome = item["nome"];
                    streamWriter.WriteLine(idp + "#" + nome);

                    ScriviSchedaArt(item);
                }
                streamWriter.Close();

                return true;
            }
            catch
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

        public List<Tuple<string, string, int>> ElencoArticoli(String id_sess, String cat_padre)
        {
            List<Tuple<string, string, int>> items = new List<Tuple<string,string, int>>();

            try
            {
                string filename = System.IO.Path.Combine(cacheDir, prefixArt + cat_padre + ".txt");
                if (!File.Exists(filename))
                {
                    clsRestCli RestScli = new clsRestCli(cacheDir);
                    JsonArray articoli= RestScli.ArtCat(id_sess, cat_padre);
                    ScriviArticoliCat(articoli, cat_padre);
                }

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
                JsonArray artArray = RestScli.ArtKeyword(id_sess, keyword);

                for (int i = 0; i < artArray.Count(); i++)
                {
                    JsonValue item = artArray[i];
                    String idp = item["idp"];
                    String prodotto = item["nome"];
                    items.Add(new Tuple<string, string, int>(idp, prodotto, Resource.Drawable.cart));

                    ScriviSchedaArt(item);
                }
            }
            catch
            {
                ;
            }

            return items;
        }

        bool ScriviSchedaArt(JsonValue Articolo)
        {
            string filename = System.IO.Path.Combine(cacheDir, prefixSch + Articolo["idp"] + ".txt");
            File.WriteAllText(filename, Articolo.ToString());
            return true;
        }

        public JsonValue DatiArticolo(String idp)
        {
            JsonValue dati = null;

            string filename = System.IO.Path.Combine(cacheDir, prefixSch + idp + ".txt");
            if (File.Exists(filename))
            {
                String strDati = File.ReadAllText(filename);
                dati = JsonValue.Parse(strDati);
            }

            return dati;
        }

        public Bitmap Articolo(String id_sess, String idp, String sizeImg)
        {
            Bitmap res = null;

            try
            {
                clsRestCli RestScli = new clsRestCli(cacheDir);
                JsonValue dati = RestScli.ArtIdp(id_sess, idp, sizeImg);
                if (dati != null) {
                    //JsonValue dati = jsonArt["articolo"];
                    String foto = dati["foto"];
                    long foto_len = dati["foto_len"];
                    if (foto != "")
                    {
                        foto = foto.Replace("../", "");

                        bool scarica = false;
                        Bitmap imageBitmap = null;
                        byte[] imageBytes = null;

                        string filename = System.IO.Path.Combine(cacheDir, prefixFoto + idp + "_" + sizeImg + ".txt");
                        if (File.Exists(filename))
                        {
                            long file_len = new System.IO.FileInfo(filename).Length;
                            if (foto_len != file_len)
                            {
                                scarica = true;
                            }
                            else
                            {
                                imageBytes = File.ReadAllBytes(filename);
                            }
                        } else scarica = true;

                        if (scarica)
                        {
                            imageBytes = RestScli.ArtFoto(foto);
                            File.WriteAllBytes(filename, imageBytes);
                        }

                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);

                        }

                        res = imageBitmap;
                    }
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