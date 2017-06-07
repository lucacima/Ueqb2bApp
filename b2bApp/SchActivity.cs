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
using Org.Json;
using Android.Graphics;
using System.IO;

namespace b2bApp
{
    [Activity(Label = "Scheda articolo")]
    public class SchActivity : Activity
    {
        String id_sess = "";
        String idp = "";
        String nome = "";
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
            String sizeImg = "";
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
                Tuple<string, string, string, string> dati_cart = objCart.RigaCarrello(riga_cart);
                etQta.Text = dati_cart.Item3;
                etNote.Text = dati_cart.Item4;
                btAggiungi.Text = "Aggiorna";
                btElimina.Visibility = ViewStates.Visible;
            }

            btAggiungi.Click += (object sender, EventArgs e) =>
            {
                if ( etQta.Text!="" )
                {
                    if (riga_cart == 0)
                    {
                        objCart.AggiungiCarrello(idp, nome, etQta.Text, etNote.Text);
                    } else
                    {
                        objCart.AggiornaCarrello(riga_cart, etQta.Text, etNote.Text);
                    }
                    this.Finish();
                } else
                {
                    Android.Widget.Toast.MakeText(this, "Specificare la quantità", Android.Widget.ToastLength.Short).Show();
                }
            };

            btElimina.Click += (object sender, EventArgs e) =>
            {
                if (riga_cart > 0)
                {
                    objCart.EliminaCarrello(riga_cart);
                }
                this.Finish();
            };



                LinearLayout parentContainer = FindViewById<LinearLayout>(Resource.Id.parentContainer);
            parentContainer.RequestFocus();

            ProgressDialog dialog = new ProgressDialog(this);
//            dialog.SetMessage("Accesso in corso");
            dialog.SetCancelable(false);
            dialog.Show();

            WebClient client = new WebClient();
            client.DownloadStringAsync(new Uri("http://2.115.37.22/umbriaeq/rest/b2brest.php?op=schedart&session_id=" + id_sess + "&idp=" + idp + "&size=" + sizeImg));
            client.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e2) =>
            {
                //string reply_art = client.DownloadString("http://2.115.37.22/umbriaeq/rest/b2brest.php?op=schedart&session_id=" + id_sess + "&idp=" + idp + "&size=" + sizeImg);
                string reply_art = e2.Result;

                JSONObject jsobj = new JSONObject(reply_art);
                string cod = jsobj.GetString("codice");
                string descr = jsobj.GetString("descrizione");
                if (cod == "0")
                {
                    JSONObject dati = jsobj.GetJSONObject("articolo");
                    String codice = dati.GetString("codice");
                    nome = dati.GetString("nome");
                    String descrizione = dati.GetString("descrizione");
                    String foto = dati.GetString("foto");
                    String Prezzo = dati.GetString("prezzo_lordo");
                    String Sconto = dati.GetString("sconto");
                    String Disponibile = dati.GetString("disponibile");

                    foto = foto.Replace("../", "");

                    ActionBar.Title = nome;
                    tvCodice.Text = "Codice: " + codice;
                    tvPrezzo.Text = "Prezzo: " + Prezzo + " (Sc. " + Sconto + "%)";
                    tvDescr.Text = descrizione;


                    //string path_file = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath; 
                    string filename = System.IO.Path.Combine(Application.CacheDir.AbsolutePath, "b2bAppCacheF" + codice + "_" + sizeImg + ".jpg");
                    Bitmap imageBitmap = null;
                    if (File.Exists(filename))
                    {
                        var imageBytes = File.ReadAllBytes(filename);
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        }
                        ivfoto.SetImageBitmap(imageBitmap);
                    }
                    try
                    {
                        client.DownloadDataAsync(new Uri("http://2.115.37.22/umbriaeq/" + foto));
                        client.DownloadDataCompleted += (object sender2, DownloadDataCompletedEventArgs e) =>
                        {
                            var imageBytes = e.Result;
                            if (imageBytes != null && imageBytes.Length > 0)
                            {
                                imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                            }
                            ivfoto.SetImageBitmap(imageBitmap);

                            File.WriteAllBytes(filename, imageBytes);
                        };
                    }
                    catch
                    {
                        // Imposto foto "No Image"
                        imageBitmap = null;
                    }
                    dialog.Dismiss();
                }
            };
        }
    }
}