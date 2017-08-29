using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using System.Threading;

namespace b2bApp
{
    [Activity(Label = "Categorie")]
    public class CatActivity : Activity
    {
        String id_sess = "";
        List<Tuple<string,string, int>> cats = new List<Tuple<string, string, int>>();
        List<Tuple<string, string, string, int>> items = new List<Tuple<string, string, string, int>>();
        List<Tuple<string, string, string, int>> items_all = new List<Tuple<string, string, string, int>>();
        String path = "";
        GestToolbar menuToolbar = new GestToolbar();
        ProgressDialog dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            id_sess = Intent.Extras.GetString("id_sess");
            String cat_padre = Intent.Extras.GetString("cat_padre");
            path= Intent.Extras.GetString("path");
            if ( path=="")
            {
                path= Resources.GetString(Resource.String.Ricerca_articoli);
            }

            if ( cat_padre!="")
            {
                SetContentView(Resource.Layout.ricerca);

                ListView listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
                listView.ItemClick += OnListItemClick;  // to be defined

                ThreadPool.QueueUserWorkItem(o => CategorieArticoli(cat_padre));
            }
            else
            {
                SetContentView(Resource.Layout.categorie);

                EditText etCerca = FindViewById<EditText>(Resource.Id.etCerca);
                Button btCerca = FindViewById<Button>(Resource.Id.btCerca);

                btCerca.Click += (object sender, EventArgs e) =>
                {
                    if (etCerca.Text.Length == 0) return;
                    btCerca.Enabled = false;

                    Intent intent = new Intent(this, typeof(RicercaActivity));
                    intent.PutExtra("id_sess", id_sess);
                    intent.PutExtra("keyword", etCerca.Text);
                    StartActivity(intent);

                    btCerca.Enabled = true;
                };

                Button btNaviga = FindViewById<Button>(Resource.Id.btNaviga);
                btNaviga.Click += (object sender, EventArgs e) =>
                {
                    btNaviga.Enabled = false;

                    Intent intent = new Intent(this, typeof(CatActivity));
                    intent.PutExtra("id_sess", id_sess);
                    intent.PutExtra("cat_padre", "0");
                    intent.PutExtra("path", Resources.GetString(Resource.String.Categorie_articoli));
                    StartActivity(intent);

                    btNaviga.Enabled = true;
                };
            }
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = path;
            
            menuToolbar.creaToolbar(this, id_sess);
        }

        private void CategorieArticoli(String cat_padre)
        {
            RunOnUiThread(() =>
            {
                dialog = new ProgressDialog(this);
                dialog.SetMessage(Resources.GetString(Resource.String.Ricerca_in_corso));
                dialog.SetCancelable(false);
                dialog.Show();
            });


            ArticoliClass objArt = new ArticoliClass(Application.CacheDir.AbsolutePath);
            // Categorie
            cats = objArt.ElencoCategorie(cat_padre);
            // Articoli
            items = objArt.ElencoArticoli(id_sess, cat_padre);
            for (int i = 0; i < cats.Count; i++)
            {
                items_all.Add(new Tuple<string, string, string, int>(cats[i].Item1, "", cats[i].Item2, cats[i].Item3));
            }
            for (int i = 0; i < items.Count; i++)
            {
                items_all.Add(new Tuple<string, string, string, int>(items[i].Item1, items[i].Item2, items[i].Item3, items[i].Item4));
            }

            RunOnUiThread(() =>
            {
                ListView listView = FindViewById<ListView>(Resource.Id.List);
                listView.Adapter = new ActivityListItem_Adapter2(this, items_all);

                dialog.Dismiss();
            });
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (items_all[e.Position].Item4== Resource.Drawable.freccia)
            {
                String cat_padre = items_all[e.Position].Item1;
                Intent intent = new Intent(this, typeof(CatActivity));
                intent.PutExtra("id_sess", id_sess);
                intent.PutExtra("cat_padre", cat_padre);
                path = cats[e.Position].Item2;
                intent.PutExtra("path", path);
                StartActivity(intent);
            }
            else
            {
                String idp = items_all[e.Position].Item1;
                Intent intent = new Intent(this, typeof(SchActivity));
                intent.PutExtra("id_sess", id_sess);
                intent.PutExtra("idp", idp);
                StartActivity(intent);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.upper_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            menuToolbar.UpperToolBar(this, item);
            return base.OnOptionsItemSelected(item);
        }

        public class ActivityListItem_Adapter : ArrayAdapter<Tuple<string, string, int>>
        {
            Activity context;
            public ActivityListItem_Adapter(Activity context, IList<Tuple<string, string, int>> objects)
                : base(context, Android.Resource.Id.Text1, objects)
            {
                this.context = context;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                //var view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
                var view = context.LayoutInflater.Inflate(Resource.Layout.lista, null);
                var item = GetItem(position);
                
                view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Item2;
                view.FindViewById<ImageView>(Android.Resource.Id.Icon).SetImageResource(item.Item3);

                return view;
            }
        }

        public class ActivityListItem_Adapter2 : ArrayAdapter<Tuple<string, string, string, int>>
        {
            Activity context;
            public ActivityListItem_Adapter2(Activity context, IList<Tuple<string, string, string, int>> objects)
                : base(context, Android.Resource.Id.Text1, objects)
            {
                this.context = context;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                //var view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
                var item = GetItem(position);
                var view = context.LayoutInflater.Inflate(Resource.Layout.lista, null);
                if (item.Item2 != "")
                {
                    view = context.LayoutInflater.Inflate(Resource.Layout.lista_articoli, null);
                    view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Item2;
                    view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.Item3;
                    view.FindViewById<ImageView>(Android.Resource.Id.Icon).SetImageResource(item.Item4);
                }
                else
                {
                    view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Item3;
                    view.FindViewById<ImageView>(Android.Resource.Id.Icon).SetImageResource(item.Item4);
                }
                return view;
            }
        }


    }
}