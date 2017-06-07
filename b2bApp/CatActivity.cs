using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Org.Json;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Content.PM;

namespace b2bApp
{
    [Activity(Label = "Categorie")]
    public class CatActivity : Activity
    {
        String id_sess = "";
        List<string> categorie = new List<string>();
        List<string> cat_id = new List<string>();
        List<string> prodotti = new List<string>();
        List<string> prod_id = new List<string>();
        List<Tuple<string, int>> cats = new List<Tuple<string, int>>();
        List<Tuple<string, int>> items = new List<Tuple<string, int>>();
        String path = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.categorie); // loads the HomeScreen.axml as this activity's view
            ListView listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
            listView.ItemClick += OnListItemClick;  // to be defined

            LinearLayout parentContainer = FindViewById<LinearLayout>(Resource.Id.parentContainer);

            id_sess = Intent.Extras.GetString("id_sess");
            String cat_padre = Intent.Extras.GetString("cat_padre");
            path= Intent.Extras.GetString("path");

            this.Title = path;
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = path;

            WebClient client = new WebClient();

            EditText etCerca = FindViewById<EditText>(Resource.Id.etCerca);
            Button btCerca = FindViewById<Button>(Resource.Id.btCerca);
            btCerca.Click += (object sender, EventArgs e) =>
            {
                if (etCerca.Text.Length == 0) return;

                categorie.Clear();
                cat_id.Clear();
                prodotti.Clear();
                prod_id.Clear();
                items.Clear();

                string reply_art = client.DownloadString("http://2.115.37.22/umbriaeq/rest/b2brest.php?op=articoli/cat&session_id=" + id_sess + "&cat_id=" + cat_padre + "&keyword=" + etCerca.Text);

                JSONObject jsobj_art = new JSONObject(reply_art);
                string cod_art = jsobj_art.GetString("codice");
                string descr_art = jsobj_art.GetString("descrizione");
                if (cod_art == "0")
                {
                    JSONArray artArray = jsobj_art.GetJSONArray("articoli");
                    for (int i = 0; i < artArray.Length(); i++)
                    {
                        JSONObject item = (JSONObject)artArray.Get(i);
                        String idp = item.GetString("idp");
                        prod_id.Add(idp);
                        String prodotto = item.GetString("nome");
                        prodotti.Add(prodotto);
                        items.Add(new Tuple<string, int>(prodotto, Resource.Drawable.cart));
                    }
                }
                listView.Adapter = new ActivityListItem_Adapter(this, items);
                parentContainer.RequestFocus();
            };

            if ( cat_padre!="0")
            {
                etCerca.Visibility = Android.Views.ViewStates.Invisible;
                btCerca.Visibility = Android.Views.ViewStates.Invisible;
                etCerca.SetHeight(0);
                btCerca.SetHeight(0);
            } else
            {                
                parentContainer.RequestFocus();
            }

            //string path_file = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath; // Android.OS.Environment.DownloadCacheDirectory.AbsolutePath;
            string filename = Path.Combine(Application.CacheDir.AbsolutePath, "b2bAppCacheC" + cat_padre + ".txt");
            if (File.Exists(filename))
            {
                var streamReader = new StreamReader(filename);

                while (!streamReader.EndOfStream)
                {
                    String riga = streamReader.ReadLine();
                    String[] col = riga.Split(';');
                    cat_id.Add(col[0]);
                    categorie.Add(col[1]);
                    cats.Add(new Tuple<string, int>(col[1], Resource.Drawable.freccia));
                }
                streamReader.Close();
            }
            else
            {                
                String reply_cat = client.DownloadString("http://2.115.37.22/umbriaeq/rest/b2brest.php?op=categorie&session_id=" + id_sess + "&cat_padre=" + cat_padre);
                var streamWriter = new StreamWriter(filename, false);

                JSONObject jsobj = new JSONObject(reply_cat);
                string cod = jsobj.GetString("codice");
                string descr = jsobj.GetString("descrizione");
                if (cod == "0")
                {
                    JSONArray catArray = jsobj.GetJSONArray("categorie");
                    for (int i = 0; i < catArray.Length(); i++)
                    {
                        JSONObject item = (JSONObject)catArray.Get(i);
                        String idc = item.GetString("category_id");
                        cat_id.Add(idc);
                        String categ = item.GetString("categoria");
                        categorie.Add(categ);

                        streamWriter.WriteLine(idc + ";" + categ);
                    }

                }
                streamWriter.Close();
            }

            //Articoli
            if (categorie.Count == 0)
            {
                string filenameA = Path.Combine(Application.CacheDir.AbsolutePath, "b2bAppCacheA" + cat_padre + ".txt");
                if (File.Exists(filenameA))
                {
                    var streamReader = new StreamReader(filenameA);

                    while (!streamReader.EndOfStream)
                    {
                        String riga = streamReader.ReadLine();
                        String[] col = riga.Split(';');
                        prod_id.Add(col[0]);
                        prodotti.Add(col[1]);
                        items.Add(new Tuple<string, int>(col[1], Resource.Drawable.cart));
                    }
                    streamReader.Close();
                }
                else
                {
                    string reply_art = client.DownloadString("http://2.115.37.22/umbriaeq/rest/b2brest.php?op=articoli/cat&session_id=" + id_sess + "&cat_id=" + cat_padre);
                    var streamWriter = new StreamWriter(filenameA, false);

                    JSONObject jsobj_art = new JSONObject(reply_art);
                    string cod_art = jsobj_art.GetString("codice");
                    string descr_art = jsobj_art.GetString("descrizione");
                    if (cod_art == "0")
                    {
                        JSONArray artArray = jsobj_art.GetJSONArray("articoli");
                        for (int i = 0; i < artArray.Length(); i++)
                        {
                            JSONObject item = (JSONObject)artArray.Get(i);
                            String idp = item.GetString("idp");
                            prod_id.Add(idp);
                            String prodotto = item.GetString("nome");
                            prodotti.Add(prodotto);
                            items.Add(new Tuple<string, int>(prodotto, Resource.Drawable.cart));
                            streamWriter.WriteLine(idp + ";" + prodotto);
                        }                        
                    }
                    streamWriter.Close();
                }
                //this.ListAdapter = new ActivityListItem_Adapter(this, items);
                listView.Adapter = new ActivityListItem_Adapter(this, items);
            } else
            {
                //this.ListAdapter = new ActivityListItem_Adapter(this, cats);
                listView.Adapter = new ActivityListItem_Adapter(this, cats);
            }

        }


        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (cat_id.Count > 0)
            {
                String cat_padre = cat_id[e.Position];
                Intent intent = new Intent(this, typeof(CatActivity));
                intent.PutExtra("id_sess", id_sess);
                intent.PutExtra("cat_padre", cat_padre);
                path += categorie[e.Position] + "/";
                intent.PutExtra("path", path);
                StartActivity(intent);
            }
            else
            {
                String idp = prod_id[e.Position];
                Intent intent = new Intent(this, typeof(SchActivity));
                intent.PutExtra("id_sess", id_sess);
                intent.PutExtra("idp", idp);
                StartActivity(intent);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if ( item.ItemId==Resource.Id.menu_cart )
            {
                Intent intent = new Intent(this, typeof(CartActivity));
                intent.PutExtra("id_sess", id_sess);
                StartActivity(intent);
            }
            if (item.ItemId == Resource.Id.menu_cerca)
            {
                Intent intent = new Intent(this, typeof(CatActivity));
                intent.SetFlags(ActivityFlags.ClearTask);
                intent.PutExtra("id_sess", id_sess);
                intent.PutExtra("cat_padre", "0");
                intent.PutExtra("path", "/");
                StartActivity(intent);
            }
            return base.OnOptionsItemSelected(item);
        }

        public class ActivityListItem_Adapter : ArrayAdapter<Tuple<string, int>>
        {
            Activity context;
            public ActivityListItem_Adapter(Activity context, IList<Tuple<string, int>> objects)
                : base(context, Android.Resource.Id.Text1, objects)
            {
                this.context = context;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                //var view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
                var view = context.LayoutInflater.Inflate(Resource.Layout.lista, null);
                var item = GetItem(position);

                view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Item1;
                view.FindViewById<ImageView>(Android.Resource.Id.Icon).SetImageResource(item.Item2);

                return view;
            }
        }

    }
}