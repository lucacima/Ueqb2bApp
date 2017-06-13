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

using System.Net;
using System.Json;
using Android.Graphics;
using System.IO;
using System.Threading.Tasks;


namespace b2bApp
{
    [Activity(Label = "Scheda articolo")]
    public class SchActivity : Activity
    {
        String id_sess = "";
        String idp = "";
        String nome = "";
        String sizeImg = "";
        int riga_cart = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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
                Tuple<string, string, string, string, string> dati_cart = objCart.RigaCarrello(riga_cart);
                etQta.Text = dati_cart.Item3;
                etNote.Text = dati_cart.Item5;
                btAggiungi.Text = "Aggiorna";
                btElimina.Visibility = ViewStates.Visible;
            }

            btAggiungi.Click += (object sender, EventArgs e) =>
            {
                if ( etQta.Text!="" )
                {
                    if (riga_cart == 0)
                    {
                        String Sconto = "0";
                        objCart.AggiungiCarrello(idp, nome, etQta.Text, tvPrezzo.Text, etNote.Text);
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
                    Toast.MakeText(this, "Specificare la quantità", Android.Widget.ToastLength.Short).Show();
                }
            };

            btElimina.Click += (object sender, EventArgs e) =>
            {
                if (riga_cart > 0)
                {
                    objCart.EliminaCarrello(riga_cart);
                }
                Intent intent = new Intent(this, typeof(CartActivity));
                intent.PutExtra("id_sess", id_sess);
                StartActivity(intent);

                this.Finish();
            };



            LinearLayout parentContainer = FindViewById<LinearLayout>(Resource.Id.parentContainer);
            parentContainer.RequestFocus();

            ArticoliClass objArt = new ArticoliClass(Application.CacheDir.AbsolutePath);
            JsonValue articolo = objArt.DatiArticolo(idp);
            if (articolo != null)
            {
                String codice = articolo["codice"];
                nome = articolo["nome"];
                String descrizione = articolo["descrizione"];
                String Prezzo = articolo["prezzo_lordo"];
                String Sconto = articolo["sconto"];

                ActionBar.Title = nome;
                tvCodice.Text = "Codice: " + codice;
                tvPrezzo.Text = "Prezzo: " + Prezzo + " (Sc. " + Sconto + "%)";
                tvDescr.Text = descrizione;
            }


            FotoArticolo();
        }

        public async Task<int> FotoArticolo()
        { 
            Bitmap articolo = null;
            /*
            ProgressDialog dialog = new ProgressDialog(this);
            dialog.SetMessage("Caricamento foto....");
            dialog.SetCancelable(false);
            dialog.Show();
            */
            try
            {
                var res = await Task.Run(() =>
                {
                    ArticoliClass objArt = new ArticoliClass(Application.CacheDir.AbsolutePath);
                    articolo = objArt.Articolo(id_sess, idp, sizeImg);

                    return 0;
                });

                if ( articolo!=null)
                {
                    ImageView ivfoto = FindViewById<ImageView>(Resource.Id.schFoto);
                    ivfoto.SetImageBitmap(articolo);
                }
            } catch
            {
                ;
            }

            //dialog.Dismiss();

            return 0;
        }
    }
}