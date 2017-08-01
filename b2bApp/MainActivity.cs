using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Threading;

namespace b2bApp
{
    [Activity(Label = "b2bApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        String id_sess;
        ProgressDialog dialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = Resources.GetString(Resource.String.Titolo); // this.Title;
            
            EditText etUtente = FindViewById<EditText>(Resource.Id.etUtente);
            EditText etPassword = FindViewById<EditText>(Resource.Id.etPassword);
            Button btEntra = FindViewById<Button>(Resource.Id.btEntra);

            btEntra.Click += (object sender, EventArgs e) =>
            {
                ThreadPool.QueueUserWorkItem(o => Entra(etUtente.Text, etPassword.Text));
            };

        }

        private void Entra(string utente, string password)
        {
            String Error = "";
            try
            {
                RunOnUiThread(() =>
                {
                    Button btEntra = FindViewById<Button>(Resource.Id.btEntra);
                    btEntra.Enabled = false;
                    dialog = new ProgressDialog(this);
                    dialog.SetMessage(Resources.GetString(Resource.String.Accesso_in_corso));
                    dialog.SetCancelable(false);
                    dialog.Show();
                });

                clsRestCli objRestCli = new clsRestCli(Application.CacheDir.AbsolutePath);
                id_sess = objRestCli.Login(utente, password);
                if (id_sess != "")
                {
                    ArticoliClass objArt = new ArticoliClass(Application.CacheDir.AbsolutePath);
                    objArt.PrendiCategorie(id_sess);
                    objArt.EliminaCacheArticoli();
                    // Elimino cache elenco ordini
                    OrdiniClass objOrdini = new OrdiniClass(Application.CacheDir.AbsolutePath);
                    objOrdini.EliminaCache();

                    RunOnUiThread(() =>  dialog.Dismiss());

                    Intent intent = new Intent(this, typeof(OrdActivity));
                    intent.PutExtra("id_sess", id_sess);
                    StartActivity(intent);
                    this.Finish();

                    // Faccio un pre-caricamenteo degli articoli con categoria 0
                    objArt.ElencoArticoli(id_sess, "0");


                }
                else
                {
                    // Messaggio utente o password errati
                    Error = Resources.GetString(Resource.String.ErrLogin1);
                }
            }
            catch
            {
                Error = Resources.GetString(Resource.String.ErrLogin2);
            }

            RunOnUiThread(() =>
            {
                if (Error != "")
                {
                    dialog.SetMessage(Error);
                    dialog.SetCancelable(true);
                }

                Button btEntra = FindViewById<Button>(Resource.Id.btEntra);
                btEntra.Enabled = true;
            });
        }
    }
}

