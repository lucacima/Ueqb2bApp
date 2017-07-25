using System;
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
    public class DettOrdActivity : Activity
    {
        String id_sess = "";
        String num_ord = "";
        String data_ord = "";

        List<Tuple<string, string, string, string>> ordini = new List<Tuple<string, string, string, string>>();
        GestToolbar menuToolbar = new GestToolbar();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (savedInstanceState != null)
            {
                ;
            }

            // loads the HomeScreen.axml as this activity's view

            id_sess = Intent.Extras.GetString("id_sess");
            data_ord = Intent.Extras.GetString("data_ord");
            num_ord = Intent.Extras.GetString("num_ord");

            SetContentView(Resource.Layout.ordini);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "Ordine n." + num_ord + " - "+ data_ord;

            OrdiniClass objOrdini = new OrdiniClass(Application.CacheDir.AbsolutePath);

            FindViewById<TextView>(Resource.Id.tvBenvenuto).Text = "Dettaglio ordine:\n";
            

            ordini= objOrdini.DettOrdine(id_sess, data_ord, num_ord);
            ordini.Insert(0, new Tuple<string, string, string, string>("Codice", "Descrizione", "Qta", "Imp"));

            ListView listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
            listView.Adapter = new ActivityListItem_Adapter(this, ordini);

            menuToolbar.creaToolbar(this, id_sess);
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

        public class ActivityListItem_Adapter : ArrayAdapter<Tuple<string, string, string, string>>
        {
            Activity context;
            public ActivityListItem_Adapter(Activity context, IList<Tuple<string, string, string, string>> objects)
                : base(context, Android.Resource.Id.Text1, objects)
            {
                this.context = context;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                //var view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
                var view = context.LayoutInflater.Inflate(Resource.Layout.dett_ordine, null);
                var item = GetItem(position);

                view.FindViewById<TextView>(Resource.Id.tvCodArt).Text = item.Item1;
                view.FindViewById<TextView>(Resource.Id.tvDesArt).Text = item.Item2;
                view.FindViewById<TextView>(Resource.Id.tvQta).Text = item.Item3;
                view.FindViewById<TextView>(Resource.Id.tvImp).Text = item.Item4;
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