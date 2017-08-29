using System;
using System.Collections.Generic;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Threading;
using System.IO;

namespace b2bApp
{
    [Activity(Label = "Benvenuto")]
    public class OrdActivity : Activity
    {
        String id_sess = "";
        List<Tuple<string, string, string, string>> ordini = new List<Tuple<string, string, string, string>>();
        GestToolbar menuToolbar = new GestToolbar();
        ProgressDialog dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetSoftInputMode(SoftInput.AdjustPan);

            if (savedInstanceState != null)
            {
                ;
            }

            // loads the HomeScreen.axml as this activity's view

            id_sess = Intent.Extras.GetString("id_sess");
            SetContentView(Resource.Layout.ordini);


            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = Resources.GetString(Resource.String.Ultimi_ordini);

            ThreadPool.QueueUserWorkItem(o => getElencoOrdini());

            menuToolbar.creaToolbar(this, id_sess);
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            Intent intent = new Intent(this, typeof(DettOrdActivity));
            intent.PutExtra("id_sess", id_sess);
            intent.PutExtra("data_ord", ordini[e.Position].Item1);
            intent.PutExtra("num_ord", ordini[e.Position].Item2);

            StartActivity(intent);
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

        private void getElencoOrdini()
        {
            try
            {                
                RunOnUiThread(() =>
                {                    
                    dialog = new ProgressDialog(this);
                    dialog.SetMessage(Resources.GetString(Resource.String.Aggiornamento_in_corso));
                    dialog.SetCancelable(false);
                    dialog.Show();
                });

                

                OrdiniClass objOrdini = new OrdiniClass(Application.CacheDir.AbsolutePath);

                String ragSoc = objOrdini.RagSocCli(id_sess);
                RunOnUiThread(() => { FindViewById<TextView>(Resource.Id.tvBenvenuto).Text = ragSoc + "\n"; });

                ordini = objOrdini.ElencoOrdini(id_sess);
                ordini.Insert(0, new Tuple<string, string, string, string>(Resources.GetString(Resource.String.Data), Resources.GetString(Resource.String.Num), Resources.GetString(Resource.String.Qta), Resources.GetString(Resource.String.Importo)));

                RunOnUiThread(() =>
                {
                    ListView listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
                    listView.Adapter = new ActivityListItem_Adapter(this, ordini);
                    listView.ItemClick += OnListItemClick;

                    dialog.Dismiss();
                });

            } catch
            {

            }
        }
    }
}