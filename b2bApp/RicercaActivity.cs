using System;
using System.Collections.Generic;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;
using System.Threading.Tasks;
using System.Threading;

namespace b2bApp
{
    [Activity(Label = "Risultato ricerca")]
    public class RicercaActivity : Activity
    {
        String id_sess = "";
        List<Tuple<string,string, string, int>> items= new List<Tuple<string, string, string, int>>();
        GestToolbar menuToolbar = new GestToolbar();
        ProgressDialog dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ricerca); // loads the HomeScreen.axml as this activity's view
            ListView listView = FindViewById<ListView>(Resource.Id.List); // get reference to the ListView in the layout
            listView.ItemClick += OnListItemClick;  // to be defined

            LinearLayout parentContainer = FindViewById<LinearLayout>(Resource.Id.parentContainer);

            id_sess = Intent.Extras.GetString("id_sess");
            String keyword = Intent.Extras.GetString("keyword");

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = Resources.GetString(Resource.String.Risultato_ricerca);

            ThreadPool.QueueUserWorkItem(o => Articoli(keyword));

            menuToolbar.creaToolbar(this, id_sess);
        }

        private void Articoli(String keyword)
        {
            RunOnUiThread(() =>
            {
                dialog = new ProgressDialog(this);
                dialog.SetMessage(Resources.GetString(Resource.String.Ricerca_in_corso));
                dialog.SetCancelable(false);
                dialog.Show();
            });

            try
            {               
                items.Clear();
                ArticoliClass objArt = new ArticoliClass(Application.CacheDir.AbsolutePath);
                items = objArt.RicercaArticoli(id_sess, keyword);

                RunOnUiThread(() =>
                {
                    ListView listView = FindViewById<ListView>(Resource.Id.List);
                    listView.Adapter = new ActivityListItem_Adapter(this, items);
                });
            }
            catch
            {
                ;
            }

            RunOnUiThread(() => dialog.Dismiss());
        }

        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            String idp = items[e.Position].Item1;
            Intent intent = new Intent(this, typeof(SchActivity));
            intent.PutExtra("id_sess", id_sess);
            intent.PutExtra("idp", idp);
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

        public class ActivityListItem_Adapter : ArrayAdapter<Tuple<string, string, string, int>>
        {
            Activity context;
            public ActivityListItem_Adapter(Activity context, IList<Tuple<string, string, string, int>> objects)
                : base(context, Android.Resource.Id.Text1, objects)
            {
                this.context = context;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                //var view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
                var view = context.LayoutInflater.Inflate(Resource.Layout.lista_articoli, null);
                var item = GetItem(position);

                view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Item2;
                view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.Item3;
                view.FindViewById<ImageView>(Android.Resource.Id.Icon).SetImageResource(item.Item4);

                return view;
            }
        }

    }
}