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
        string fElenco_foto = "b2bAppCacheElenco_foto.json";

        public ArticoliClass(String cacheDir)
        {
            this.cacheDir = cacheDir;
        }
        
        public bool PrendiCategorie(String id_sess)
        {
            try
            {
                //Elimino file esistenti
                string[] filesCat = Directory.GetFiles(cacheDir, prefixCat + "*.json");
                foreach (string fileCat in filesCat)
                {
                    File.Delete(fileCat);
                }

                clsRestCli RestScli = new clsRestCli(cacheDir);
                JsonArray catArray = RestScli.CatAll(id_sess);

                // Elenco categorie
                String filename = System.IO.Path.Combine(cacheDir, prefixCat + "0.json");
                String cat_padre = "0";

                JsonArray catFArray = new JsonArray();
                for (int i = 0; i < catArray.Count(); i++)
                {
                    JsonValue item = catArray[i];
                    String catp = item["cat_padre"];
                    if (catp != cat_padre)
                    {
                        File.WriteAllText(filename, catFArray.ToString());
                        catFArray.Clear();
                        filename = System.IO.Path.Combine(cacheDir, prefixCat + catp + ".json");
                        cat_padre = catp;
                    }
                    catFArray.Add(item);
                }
                File.WriteAllText(filename, catFArray.ToString());

                // Elenco foto
                JsonArray fotoArray = RestScli.ElencoFoto();
                ScriviElencoFoto(fotoArray);

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
            string[] filesArt = Directory.GetFiles(cacheDir, prefixArt + "*.json");
            foreach (string fileArt in filesArt)
            {
                File.Delete(fileArt);
            }

            return true;
        }

        public bool ScriviArticoliCat(JsonArray artArray, String cat_padre)
        {
            try
            {
                String filename = System.IO.Path.Combine(cacheDir, prefixArt + cat_padre + ".json");
                File.WriteAllText(filename, artArray.ToString());
                for (int i = 0; i < artArray.Count(); i++)
                {
                    JsonValue item = artArray[i];
                    ScriviSchedaArt(item);
                }

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
                string filename = System.IO.Path.Combine(cacheDir, prefixCat + cat_padre + ".json");

                if (File.Exists(filename))
                {
                    String categorie = File.ReadAllText(filename);
                    JsonArray arrCateg = (JsonArray) JsonArray.Parse(categorie);
                    for (int i = 0; i < arrCateg.Count(); i++)
                    {
                        String idp = arrCateg[i]["category_id"];
                        String nome = arrCateg[i]["categoria"];
                        cats.Add(new Tuple<string, string, int>(idp, nome, Resource.Drawable.freccia));
                    }
                }
            }
            catch
            {
                ;
            }
            return cats;
        }

        public List<Tuple<string,string, string, int>> ElencoArticoli(String id_sess, String cat_padre)
        {
            List<Tuple<string, string, string, int>> items = new List<Tuple<string,string,string,int>>();

            try
            {
                JsonArray arrArticoli = new JsonArray();

                string filename = System.IO.Path.Combine(cacheDir, prefixArt + cat_padre + ".json");
                if (!File.Exists(filename))
                {
                    clsRestCli RestScli = new clsRestCli(cacheDir);
                    arrArticoli = RestScli.ArtCat(id_sess, cat_padre);
                    ScriviArticoliCat(arrArticoli, cat_padre);
                } else
                {
                    String articoli = File.ReadAllText(filename);
                    arrArticoli = (JsonArray)JsonArray.Parse(articoli);
                }
                for (int i = 0; i < arrArticoli.Count(); i++)
                {
                    items.Add(new Tuple<string, string, string, int>(arrArticoli[i]["idp"], arrArticoli[i]["codice"], arrArticoli[i]["nome"], Resource.Drawable.cart));
                }
            }
            catch
            {
                ;
            }

            return items;
        }

        public List<Tuple<string, string, string, int>> RicercaArticoli(String id_sess, String keyword)
        {
            List<Tuple<string, string, string, int>> items = new List<Tuple<string, string, string, int>>();

            try
            {
                clsRestCli RestScli = new clsRestCli(cacheDir);
                JsonArray artArray = RestScli.ArtKeyword(id_sess, keyword);

                for (int i = 0; i < artArray.Count(); i++)
                {
                    JsonValue item = artArray[i];
                    String idp = item["idp"];
                    String prodotto = item["nome"];
                    String codice = item["codice"];
                    items.Add(new Tuple<string, string, string, int>(idp, codice, prodotto, Resource.Drawable.cart));

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
            string filename = System.IO.Path.Combine(cacheDir, prefixSch + Articolo["idp"] + ".json");
            File.WriteAllText(filename, Articolo.ToString());
            return true;
        }

        public JsonValue DatiArticolo(String idp)
        {
            JsonValue dati = null;

            string filename = System.IO.Path.Combine(cacheDir, prefixSch + idp + ".json");
            if (File.Exists(filename))
            {
                String strDati = File.ReadAllText(filename);
                dati = JsonValue.Parse(strDati);
            }

            return dati;
        }

        public JsonArray ElencoFoto()
        {
            JsonArray arrFoto = new JsonArray();


            string filename = System.IO.Path.Combine(cacheDir, fElenco_foto);
            if ( File.Exists(filename) )
            {
                string strElenco = File.ReadAllText(filename);
                arrFoto = (JsonArray) JsonValue.Parse(strElenco);
            }

            return arrFoto;
        }

        private bool aggElencoFoto(string nome_foto, long size_foto)
        {
            JsonArray arrFoto = ElencoFoto();

            JsonObject jobj = new JsonObject(new KeyValuePair<string, JsonValue>("filename", nome_foto), new KeyValuePair<string, JsonValue>("size", size_foto));
            arrFoto.Add(jobj);

            ScriviElencoFoto(arrFoto);

            return true;
        }

        private bool ScriviElencoFoto(JsonArray fotoArray)
        {
            string filename = System.IO.Path.Combine(cacheDir, fElenco_foto);
            File.WriteAllText(filename, fotoArray.ToString());

            return true;
        }

        public byte[] Articolo(String id_sess, String idp, String sizeImg)
        {
            byte[] imageBytes = null;

            try
            {

                JsonValue articolo = DatiArticolo(idp);
                JsonArray arrFoto = ElencoFoto();
                string nomeFoto = sizeImg + "x" + sizeImg + "_" + articolo["codice"] + ".jpg";

                string filename = System.IO.Path.Combine(cacheDir, prefixFoto + idp + "_" + sizeImg + ".jpg");
                if (File.Exists(filename))
                {
                    long file_len = new System.IO.FileInfo(filename).Length;
                    
                    for (int i = 0; i < arrFoto.Count; i++)
                    {                        
                        string nomeFoto2 = arrFoto[i]["filename"];

                        if (nomeFoto.ToUpper() == nomeFoto2.ToUpper())
                        {
                            long fotoSize = arrFoto[i]["size"];
                            if (file_len == fotoSize)
                            {
                                imageBytes = File.ReadAllBytes(filename);
                                return imageBytes;
                            }
                            break;
                        }
                    }
                }

                clsRestCli RestScli = new clsRestCli(cacheDir);
                JsonValue dati = RestScli.ArtIdp(id_sess, idp, sizeImg);
                if (dati != null)
                {
                    //JsonValue dati = jsonArt["articolo"];
                    String foto = dati["foto"];
                    imageBytes = RestScli.ArtFoto(foto);
                    File.WriteAllBytes(filename, imageBytes);

                    aggElencoFoto(nomeFoto, imageBytes.Length);
                }
                
                return imageBytes;


                //clsRestCli RestScli = new clsRestCli(cacheDir);
                //JsonValue dati = RestScli.ArtIdp(id_sess, idp, sizeImg);
                //if (dati != null) {
                //    //JsonValue dati = jsonArt["articolo"];
                //    String foto = dati["foto"];
                //    long foto_len = dati["foto_len"];
                //    if (foto != "")
                //    {
                //        foto = foto.Replace("../", "");

                //        bool scarica = false;                        
                        

                //        string filename = System.IO.Path.Combine(cacheDir, prefixFoto + idp + "_" + sizeImg + ".jpg");
                //        if (File.Exists(filename))
                //        {
                //            long file_len = new System.IO.FileInfo(filename).Length;
                //            if (foto_len != file_len)
                //            {
                //                scarica = true;
                //            }
                //            else
                //            {
                //                imageBytes = File.ReadAllBytes(filename);
                //            }
                //        } else scarica = true;

                //        if (scarica)
                //        {
                //            imageBytes = RestScli.ArtFoto(foto);
                //            File.WriteAllBytes(filename, imageBytes);
                //        }
                //    }
                //}
            }
            catch
            {
                ;
            }

            return imageBytes;
        }

    }
    }