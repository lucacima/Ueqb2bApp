using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace b2bApp
{
    [Activity(Label = "Carrello")]
    public class CartActivity : Activity
    {
        String id_sess = "";
        List<Tuple<string, string, int, float, int, int, string>> carts;
        clsCart objCart;

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

            objCart = new clsCart(Application.CacheDir.AbsolutePath, id_sess);
            EditText etNote = FindViewById<EditText>(Resource.Id.etNote);
            Button btOrdine = FindViewById<Button>(Resource.Id.btOrdine);
            btOrdine.Click += (object sender, EventArgs e) =>
            {
                // Controlli                 
                int id_ord = objCart.InviaOrdine(etNote.Text);

                Toast.MakeText(this, "Ordine n." + id_ord.ToString() +" creato correttamente", Android.Widget.ToastLength.Short).Show();
                //Messaggio dopo
                this.Finish();
            };

            carts = objCart.ListaCarrello();
            listView.Adapter = new ActivityListItem_Adapter(this, carts);

            float tot = objCart.TotaleCarrello();
            TextView tvTot = FindViewById<TextView>(Resource.Id.tvTot);
            tvTot.Text = tvTot.Text + " " + String.Format("{0:N2}", tot);

            if ( carts.Count==0)
            {
                btOrdine.Visibility = ViewStates.Invisible;
                TextView tvNote = FindViewById<TextView>(Resource.Id.tvNote);
                tvNote.Visibility = ViewStates.Invisible;
                etNote.Visibility = ViewStates.Invisible;
            }
        }


        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(SchActivity));
            intent.PutExtra("id_sess", id_sess);
            string idp = objCart.IdpCarrello(e.Position+1);
            intent.PutExtra("idp", idp);
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
                intent.PutExtra("path", "Ricerca articoli");
                StartActivity(intent);
                this.Finish();
            }
            return base.OnOptionsItemSelected(item);
        }

        public class ActivityListItem_Adapter : ArrayAdapter<Tuple<string, string, int, float, int, int, string>>
        {
            Activity context;
            public ActivityListItem_Adapter(Activity context, IList<Tuple<string, string, int, float, int, int, string>> objects)
                : base(context, Android.Resource.Id.Text1, objects)
            {
                this.context = context;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var view = context.LayoutInflater.Inflate(Resource.Layout.riga_carrello, null);
                var item = GetItem(position);

                view.FindViewById<TextView>(Resource.Id.codice_cart).Text = item.Item1;
                view.FindViewById<TextView>(Resource.Id.nome_cart).Text = item.Item2;
                view.FindViewById<TextView>(Resource.Id.qta_cart).Text = String.Format("{0:N0}", item.Item3);
                String str_prezzo = String.Format("{0:N2}({1:N0}%)", item.Item4, item.Item5);
                view.FindViewById<TextView>(Resource.Id.prz_cart).Text = str_prezzo;
                view.FindViewById<TextView>(Resource.Id.iva_cart).Text = String.Format("{0:N0}%",item.Item6);
                float netto = (float)item.Item4 - ((float)item.Item4 * (float)item.Item5 / 100);
                float importo = netto * (int)item.Item3;
                float importoIva = importo + (importo * (int)item.Item6 / 100);

                String str_importo = String.Format("{0:N2}", importoIva);
                view.FindViewById<TextView>(Resource.Id.imp_cart).Text = str_importo;
                view.FindViewById<TextView>(Resource.Id.note_cart).Text = item.Item7;
                return view;
            }
        }

    }
}