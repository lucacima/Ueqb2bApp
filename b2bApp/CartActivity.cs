﻿using System;
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
    [Activity(Label = "Carrello")]
    public class CartActivity : Activity
    {
        String id_sess = "";
        List<Tuple<string, string, string, string>> carts;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.carrello); // loads the HomeScreen.axml as this activity's view
            ListView listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
            listView.ItemClick += OnListItemClick;  // to be defined

            id_sess = Intent.Extras.GetString("id_sess");

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Carrello";

            Button btOrdine = FindViewById<Button>(Resource.Id.btOrdine);
//            btOrdine.Click += 

            clsCart objCart = new clsCart(Application.CacheDir.AbsolutePath, id_sess);
            carts = objCart.RigheCarrello();
            listView.Adapter = new ActivityListItem_Adapter(this, carts);
        }


        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(SchActivity));
            intent.PutExtra("id_sess", id_sess);
            intent.PutExtra("idp", carts[e.Position].Item1);
            intent.PutExtra("riga_cart", e.Position+1);
            StartActivity(intent);
            this.Finish();
        }
  
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_cerca)
            {
                Intent intent = new Intent(this, typeof(CatActivity));
                intent.SetFlags(ActivityFlags.ClearTask);
                intent.PutExtra("id_sess", id_sess);
                intent.PutExtra("cat_padre", "0");
                intent.PutExtra("path", "/");
                StartActivity(intent);
                this.Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        public class ActivityListItem_Adapter : ArrayAdapter<Tuple<string,string, string, string>>
        {
            Activity context;
            public ActivityListItem_Adapter(Activity context, IList<Tuple<string,string, string, string>> objects)
                : base(context, Android.Resource.Id.Text1, objects)
            {
                this.context = context;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                //var view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
                var view = context.LayoutInflater.Inflate(Resource.Layout.riga_carrello, null);
                var item = GetItem(position);

                view.FindViewById<TextView>(Resource.Id.nome_cart).Text = item.Item2;
                view.FindViewById<TextView>(Resource.Id.qta_cart).Text = item.Item3;
                view.FindViewById<TextView>(Resource.Id.note_cart).Text = item.Item4;
                return view;
            }
        }

    }
}