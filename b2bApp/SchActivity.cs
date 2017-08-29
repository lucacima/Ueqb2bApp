using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using System.Json;
using Android.Graphics;
using System.Threading;

namespace b2bApp
{
    [Activity(Label = "Scheda articolo")]
    public class SchActivity : Activity
    {
        String id_sess = "";
        String idp = "";
        String codice = "";
        String nome = "";
        String descrizione = "";
        String sizeImg = "";
        float prezzo = 0;
        float sconto = 0;
        float aliva = 22;
        int riga_cart = 0;
        GestToolbar menuToolbar = new GestToolbar();
        ProgressDialog dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetSoftInputMode(SoftInput.AdjustPan);

            id_sess = Intent.Extras.GetString("id_sess");
            idp = Intent.Extras.GetString("idp");
            riga_cart = Intent.Extras.GetInt("riga_cart");

            SetContentView(Resource.Layout.scheda);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);

            var metrics = Resources.DisplayMetrics;
            var widthInDp = metrics.WidthPixels;
            var heightInDp = metrics.HeightPixels;
            
            if ( widthInDp>heightInDp)
            {
                sizeImg = heightInDp.ToString();
            }else
            {
                sizeImg = widthInDp.ToString();
            }

            ImageView ivfoto = FindViewById<ImageView>(Resource.Id.schFoto);
            TextView tvCodice = FindViewById<TextView>(Resource.Id.tvCodice);
            TextView tvPrezzo = FindViewById<TextView>(Resource.Id.tvPrezzo);
            TextView tvDescr = FindViewById<TextView>(Resource.Id.tvDescr);

            EditText etQta = FindViewById<EditText>(Resource.Id.etQta);
            EditText etNote = FindViewById<EditText>(Resource.Id.etNote);
            Button btAggiungi = FindViewById<Button>(Resource.Id.btAggCarrello);
            Button btElimina = FindViewById<Button>(Resource.Id.btEliCarrello);
            clsCart objCart = new clsCart(Application.CacheDir.AbsolutePath, id_sess);          
            
            if ( riga_cart>0)
            {
                JsonValue dati_cart = objCart.RigaCarrello(riga_cart);
                etQta.Text = dati_cart["Qta"];
                etNote.Text = dati_cart["Note"];
                btAggiungi.Text = Resources.GetString(Resource.String.Aggiorna);
                btElimina.Visibility = ViewStates.Visible;
            }

            btAggiungi.Click += (object sender, EventArgs e) =>
            {
                btAggiungi.Enabled = false;
                if ( etQta.Text!="" )
                {
                    if (riga_cart == 0)
                    {
                        objCart.AggiungiCarrello(idp, codice, nome, etQta.Text, prezzo, sconto, aliva, etNote.Text);
                    } else
                    {
                        objCart.AggiornaCarrello(riga_cart, etQta.Text, etNote.Text);
                    }
                    Intent intent = new Intent(this, typeof(CartActivity));
                    intent.PutExtra("id_sess", id_sess);
                    StartActivity(intent);

                    this.Finish();
                } else
                {
                    Toast.MakeText(this, Resources.GetString(Resource.String.ErrQta), Android.Widget.ToastLength.Short).Show();
                }
                btAggiungi.Enabled = true;
            };

            btElimina.Click += (object sender, EventArgs e) =>
            {
                btElimina.Enabled = false;

                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetMessage(Resources.GetString(Resource.String.ConfElimina));
                alert.SetPositiveButton( (Resources.GetString(Resource.String.Si)) , (senderAlert, args) => {
                    if (riga_cart > 0)
                    {
                        objCart.EliminaCarrello(riga_cart);
                    }
                    Intent intent = new Intent(this, typeof(CartActivity));
                    intent.PutExtra("id_sess", id_sess);
                    StartActivity(intent);

                    this.Finish();
                    btElimina.Enabled = true;
                });
                alert.SetNegativeButton((Resources.GetString(Resource.String.No)), (senderAlert, args) => {
                    btElimina.Enabled = true;
                });
                Dialog dialog = alert.Create();
                dialog.Show();


            };



            LinearLayout parentContainer = FindViewById<LinearLayout>(Resource.Id.parentContainer);
            parentContainer.RequestFocus();

            ArticoliClass objArt = new ArticoliClass(Application.CacheDir.AbsolutePath);
            JsonValue articolo = objArt.DatiArticolo(idp);
            if (articolo != null)
            {
                codice = articolo["codice"];
                nome = articolo["nome"];
                descrizione = articolo["descrizione"];
                prezzo = (float) articolo["prezzo_lordo"];
                sconto = (float) articolo["sconto"];
                aliva = (float) articolo["aliva"];


                ActionBar.Title = nome;
                tvCodice.Text = "Codice: " + codice;
                tvPrezzo.Text = String.Format(Resources.GetString(Resource.String.Prezzo) + ": {0:N2} (-{1:N0}%)", prezzo, sconto); 
                tvDescr.Text = descrizione;
            }

            ThreadPool.QueueUserWorkItem(o => caricaFoto());

            menuToolbar.creaToolbar(this, id_sess);
        }

        private void caricaFoto()
        {
            Bitmap articolo = null;            

            RunOnUiThread(() =>
            {
                dialog = new ProgressDialog(this);

                dialog.SetMessage(Resources.GetString(Resource.String.Aggiornamento_in_corso));
                dialog.SetCancelable(true);
                dialog.Show();
            });

            try
            {
                ArticoliClass objArt = new ArticoliClass(Application.CacheDir.AbsolutePath);
                byte[] imageBytes = objArt.Articolo(id_sess, idp, sizeImg);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    articolo = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);

                }
                if (articolo != null)
                {
                    RunOnUiThread(() =>
                    {
                        ImageView ivfoto = FindViewById<ImageView>(Resource.Id.schFoto);
                        ivfoto.SetImageBitmap(articolo);
                    });
                }
            }
            catch
            {
                ;
            }

            RunOnUiThread(() => dialog.Dismiss());
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
    }
}