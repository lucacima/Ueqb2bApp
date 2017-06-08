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
    [Activity(Label = "Categorie")]
    public class CatActivity : Activity
    {
        String id_sess = "";
        List<Tuple<string,string, int>> cats = new List<Tuple<string, string, int>>();
        List<Tuple<string,string, int>> items= new List<Tuple<string, string, int>>();
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

            ArticoliClass objArt = new ArticoliClass(Application.CacheDir.AbsolutePath);

            EditText etCerca = FindViewById<EditText>(Resource.Id.etCerca);
            Button btCerca = FindViewById<Button>(Resource.Id.btCerca);
            btCerca.Click += async (object sender, EventArgs e) =>
            {
                if (etCerca.Text.Length == 0) return;

                ProgressDialog dialog = new ProgressDialog(this);
                dialog.SetMessage("Ricerca in corso...");
                dialog.SetCancelable(false);
                dialog.Show();

                InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.HideSoftInputFromWindow(etCerca.WindowToken, 0);

                cats.Clear();
                items.Clear();

                var res = await Task.Run(() => {
                    items = objArt.RicercaArticoli(id_sess, etCerca.Text);
                    return true;
                });

                listView.Adapter = new ActivityListItem_Adapter(this, items);
                dialog.Dismiss();

            };

            if ( cat_padre!="0")
            {
                etCerca.Visibility = ViewStates.Invisible;
                btCerca.Visibility = ViewStates.Invisible;
                etCerca.SetHeight(0);
                btCerca.SetHeight(0);
            } else
            {                
                parentContainer.RequestFocus();
            }

            // Categorie
            cats = objArt.ElencoCategorie(cat_padre);

            //Articoli
            if (cats.Count == 0)
            {

                items = objArt.ElencoArticoli(cat_padre);
                listView.Adapter = new ActivityListItem_Adapter(this, items);
            } else
            {
                listView.Adapter = new ActivityListItem_Adapter(this, cats);
            }

        }


        void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (cats.Count > 0)
            {
                String cat_padre = cats[e.Position].Item1;
                Intent intent = new Intent(this, typeof(CatActivity));
                intent.PutExtra("id_sess", id_sess);
                intent.PutExtra("cat_padre", cat_padre);
                path += cats[e.Position].Item2 + "/";
                intent.PutExtra("path", path);
                StartActivity(intent);
            }
            else
            {
                String idp = items[e.Position].Item1;
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

    }
}