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

namespace b2bApp
{
    class GestToolbar
    {
        public bool creaToolbar(Activity actv, string id_sess)
        {
            var editToolbar = actv.FindViewById<Toolbar>(Resource.Id.edit_toolbar);
            editToolbar.Title = "Menu";
            editToolbar.InflateMenu(Resource.Menu.top_menus);
            editToolbar.MenuItemClick += (sender, e) => {
                if (e.Item.ItemId == Resource.Id.menu_ordini)
                {
                    Intent intent = new Intent(actv, typeof(OrdActivity));
                    intent.PutExtra("id_sess", id_sess);
                    actv.StartActivity(intent);
                }
                if (e.Item.ItemId == Resource.Id.menu_cerca)
                {
                    Intent intent = new Intent(actv, typeof(CatActivity));
                    intent.PutExtra("id_sess", id_sess);
                    intent.PutExtra("cat_padre", "");
                    intent.PutExtra("path", "Ricerca articoli");
                    actv.StartActivity(intent);
                }
                if (e.Item.ItemId == Resource.Id.menu_cart)
                {
                    Intent intent = new Intent(actv, typeof(CartActivity));
                    intent.PutExtra("id_sess", id_sess);
                    actv.StartActivity(intent);
                }

            };

            return true;
        }

        public bool UpperToolBar(Activity actv, IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_esci)
            {
                Intent intent = new Intent(actv, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                actv.StartActivity(intent);
                actv.Finish();
            }
            return true;
        }
    }
}