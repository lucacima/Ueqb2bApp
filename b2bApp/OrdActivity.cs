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
    public class OrdActivity : Activity
    {
        String id_sess = "";
        List<Tuple<string, string, string, string>> ordini = new List<Tuple<string, string, string, string>>();

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

            OrdiniClass objOrdini = new OrdiniClass(Application.CacheDir.AbsolutePath);

            FindViewById<TextView>(Resource.Id.tvBenvenuto).Text = "Benvenuto Luca, \nDi seguito gli ultimi ordini:\n";
            
            ordini= objOrdini.ElencoOrdini(id_sess);
            ordini.Insert(0, new Tuple<string, string, string, string>("Data", "Num", "Qta", "Imp"));

            ListView listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
            listView.Adapter = new ActivityListItem_Adapter(this, ordini);
            listView.ItemClick += OnListItemClick;  // to be defined
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            /*
            Intent intent = new Intent(this, typeof(CatActivity));
            intent.PutExtra("id_sess", id_sess);
            StartActivity(intent);
            */
            OrdiniClass objOrdini = new OrdiniClass(Application.CacheDir.AbsolutePath);
            var xx = objOrdini.DettOrdine(id_sess, ordini[e.Position].Item1, ordini[e.Position].Item2);

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
                var view = context.LayoutInflater.Inflate(Resource.Layout.lista_ordini, null);
                var item = GetItem(position);

                view.FindViewById<TextView>(Resource.Id.tvDDoc).Text = item.Item1;
                view.FindViewById<TextView>(Resource.Id.tvNDoc).Text = item.Item2;
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