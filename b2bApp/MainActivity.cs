using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Threading.Tasks;

namespace b2bApp
{
    [Activity(Label = "b2bApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
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
                ProgressDialog dialog = new ProgressDialog(this);
                dialog.SetMessage("Accesso in corso...");
                dialog.SetCancelable(false);
                dialog.Show();

                var res= Task.Run(() => {
                    try
                    {
                        clsRestCli objRestCli = new clsRestCli(Application.CacheDir.AbsolutePath);
                        String id_sess = objRestCli.Login(etUtente.Text, etPassword.Text);
                        if (id_sess != "")
                        {
                            objRestCli.CatAll(id_sess);
                            objRestCli.ArtAll(id_sess);

                            Intent intent = new Intent(this, typeof(CatActivity));
                            intent.PutExtra("id_sess", id_sess);
                            intent.PutExtra("cat_padre", "0");
                            intent.PutExtra("path", "/");
                            StartActivity(intent);

                            this.Finish();
                        }
                        else
                        {
                            // Messaggio utente o password errati
                            Toast.MakeText(this, "Utente o password non validi", Android.Widget.ToastLength.Short).Show();
                        }
                    } catch
                    {
                        Toast.MakeText(this, "Errore in fase di autenticazione", Android.Widget.ToastLength.Short).Show();
                    }

                    dialog.Dismiss();
                } );
            };

        }
    }
}

