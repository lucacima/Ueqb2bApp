﻿using System;
using System.Collections.Generic;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;
using System.Threading.Tasks;

namespace b2bApp
{
    [Activity(Label = "Benvenuto")]
    public class OrdActivity : Activity
    {
        String id_sess = "";
        List<Tuple<string,string, int>> cats = new List<Tuple<string, string, int>>();
        List<Tuple<string,string, int>> items= new List<Tuple<string, string, int>>();
        String path = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (savedInstanceState != null)
            {
                ;
            }

            // loads the HomeScreen.axml as this activity's view

            id_sess = Intent.Extras.GetString("id_sess");
            SetContentView(Resource.Layout.ordini);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Ultimi Ordini";

            /*
            ListView listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
            listView.ItemClick += OnListItemClick;  // to be defined

            this.Title = path;
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = path;

            ArticoliClass objArt = new ArticoliClass(Application.CacheDir.AbsolutePath);

            // Categorie
            cats = objArt.ElencoCategorie(cat_padre);

            CercaArticoli(cat_padre);
            */
        }

        public async Task<int> CercaArticoli(String cat_padre)
        {
            ProgressDialog dialog = new ProgressDialog(this);
            dialog.SetMessage("Ricerca in corso...");
            dialog.SetCancelable(false);
            dialog.Show();

            ArticoliClass objArt = new ArticoliClass(Application.CacheDir.AbsolutePath);
            ListView listView = FindViewById<ListView>(Resource.Id.List);

            var res = await Task.Run(() => {
                items = objArt.ElencoArticoli(id_sess, cat_padre);
                return true;
            });
            listView.Adapter = new ActivityListItem_Adapter(this, items);

            dialog.Dismiss();
            return 0;
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            String cat_padre = cats[e.Position].Item1;
            Intent intent = new Intent(this, typeof(CatActivity));
            intent.PutExtra("id_sess", id_sess);
            intent.PutExtra("cat_padre", cat_padre);
            path = cats[e.Position].Item2;
            intent.PutExtra("path", path);
            StartActivity(intent);
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
                intent.PutExtra("path", "Ricerca articoli");
                StartActivity(intent);
            }
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

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString("id_sess", id_sess);

            base.OnSaveInstanceState(outState);
        }
    }
}