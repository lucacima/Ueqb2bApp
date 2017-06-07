using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Net;
using Org.Json;
using Android.Content;
using System.IO;

namespace b2bApp
{
    [Activity(Label = "b2bApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = this.Title;
            
            EditText etUtente = FindViewById<EditText>(Resource.Id.etUtente);
            EditText etPassword = FindViewById<EditText>(Resource.Id.etPassword);
            Button btEntra = FindViewById<Button>(Resource.Id.btEntra);

            btEntra.Click += (object sender, EventArgs e) =>
            {
                JSONObject jsobjIn = new JSONObject();
                jsobjIn.Put("username", etUtente.Text);
                jsobjIn.Put("password", etPassword.Text);

                ProgressDialog dialog = new ProgressDialog(this);
                dialog.SetMessage("Accesso in corso");
                dialog.SetCancelable(false);
                dialog.Show();

                string data = jsobjIn.ToString(); //"{\"username\":\"2644\",\"password\":63936}";
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;

                client.UploadStringAsync(new Uri("http://2.115.37.22/umbriaeq/rest/b2brest.php?op=login"), data);
                client.UploadStringCompleted += (object sender2, UploadStringCompletedEventArgs e2) =>
                {
                    String reply = e2.Result;
                    //string reply = client.UploadString("http://2.115.37.22/umbriaeq/rest/b2brest.php?op=login", data);
                    JSONObject jsobj = new JSONObject(reply);
                    string cod = jsobj.GetString("codice");
                    string descr = jsobj.GetString("descrizione");

                    if (cod == "0")
                    {
                        string id_sess = jsobj.GetString("user_id");

                        // Categorie
                        String reply_cat = client.DownloadString("http://2.115.37.22/umbriaeq/rest/b2brest.php?op=catall&session_id=" + id_sess);
                        String filename = Path.Combine(Application.CacheDir.AbsolutePath, "b2bAppCacheC0.txt");
                        StreamWriter streamWriter = new StreamWriter(filename, false);
                        String cat_padre = "0";

                        JSONObject jsobj2 = new JSONObject(reply_cat);
                        string cod2 = jsobj2.GetString("codice");
                        if (cod2 == "0")
                        {
                            JSONArray catArray = jsobj2.GetJSONArray("categorie");
                           
                            for (int i = 0; i < catArray.Length(); i++)
                            {
                                JSONObject item = (JSONObject)catArray.Get(i);
                                String catp = item.GetString("cat_padre");
                                if ( catp!=cat_padre)
                                {
                                    streamWriter.Close();
                                    filename= Path.Combine(Application.CacheDir.AbsolutePath, "b2bAppCacheC" + catp +".txt");
                                    streamWriter=new StreamWriter(filename, false);
                                    cat_padre = catp;
                                }
                                String idc = item.GetString("category_id");
                                String categ = item.GetString("categoria");
                                streamWriter.WriteLine(idc + ";" + categ);
                            }
                            streamWriter.Close();
                        }

                        //Articoli
                        //String reply_art = client.DownloadString("http://2.115.37.22/umbriaeq/rest/b2brest.php?op=articoli/all&session_id=" + id_sess);
                        client.DownloadStringAsync(new Uri("http://2.115.37.22/umbriaeq/rest/b2brest.php?op=articoli/all&session_id=" + id_sess));
                        client.DownloadStringCompleted += (object sender3, DownloadStringCompletedEventArgs e3) =>
                        {
                            filename = Path.Combine(Application.CacheDir.AbsolutePath, "b2bAppCacheA0.txt");
                            streamWriter = new StreamWriter(filename, false);
                            cat_padre = "0";

                            String reply_art = e3.Result;
                            jsobj2 = new JSONObject(reply_art);
                            cod2 = jsobj2.GetString("codice");
                            if (cod2 == "0")
                            {
                                JSONArray catArray = jsobj2.GetJSONArray("articoli");

                                for (int i = 0; i < catArray.Length(); i++)
                                {
                                    JSONObject item = (JSONObject)catArray.Get(i);
                                    String catp = item.GetString("category_id");
                                    if (catp != cat_padre)
                                    {
                                        streamWriter.Close();
                                        filename = Path.Combine(Application.CacheDir.AbsolutePath, "b2bAppCacheA" + catp + ".txt");
                                        streamWriter = new StreamWriter(filename, false);
                                        cat_padre = catp;
                                    }
                                    String idp = item.GetString("product_id");
                                    String nome = item.GetString("nome");
                                    streamWriter.WriteLine(idp + ";" + nome);
                                }
                                streamWriter.Close();
                            }
                        };


                        Intent intent = new Intent(this, typeof(CatActivity));
                        intent.PutExtra("id_sess", id_sess);
                        intent.PutExtra("cat_padre", "0");
                        intent.PutExtra("path", "/");
                        StartActivity(intent);

                        dialog.Dismiss();
                    }

                };
            };


        }
    }
}

