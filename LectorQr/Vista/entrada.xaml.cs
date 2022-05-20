using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace LectorQr.Vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class entrada : ContentPage
    {
        bool activarLinterna = false;
        public entrada()
        {
            InitializeComponent();
        }

        private async void btnLeerQr_Clicked(object sender, EventArgs e)
        {

            var overlay = new ZXingDefaultOverlay
            {
                ShowFlashButton = false,
                TopText = "Coloca el Código QR  frente al dispositivo",
                BottomText = "Registrar la ENTRADA",
                Opacity = 1
            };
            overlay.BindingContext = overlay;
            var options = new MobileBarcodeScanningOptions();
            options.PossibleFormats = new List<BarcodeFormat>()
            {
                ZXing.BarcodeFormat.EAN_8,
                ZXing.BarcodeFormat.EAN_13,
                ZXing.BarcodeFormat.AZTEC,
                ZXing.BarcodeFormat.QR_CODE
            };


            var page = new ZXingScannerPage(options, overlay)
            {
                Title = "Entrada",
                DefaultOverlayShowFlashButton = true,
            };


            if (activarLinterna)
                {
                    Device.BeginInvokeOnMainThread(delegate { page.ToggleTorch(); });
                    swhitchLinterna.IsToggled = false;
                    activarLinterna=false;
                }



                await Navigation.PushModalAsync(page);

            page.OnScanResult += (result) =>
            {
                page.IsScanning = false;
                page.AutoFocus(0, 500);
                //inicia un hilo en espera
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync();
                    lblLecturaQr.Text = result.Text;
                    PlaySonido();
                    DisplayAlert("Entrada", "Entrada Registrada", "OK");
                });
            };



        }
        public void PlaySonido()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream audioSream = assembly.GetManifestResourceStream("LectorQr.sonido.Beep.mp3");
            var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Load(audioSream);
            audio.Play();
        }

        private void swhitchLinterna_Toggled(object sender, ToggledEventArgs e)
        {
            activarLinterna = swhitchLinterna.IsToggled;
            EncenderApagarLinterna();
        }
        async private void EncenderApagarLinterna()
        {
            try
            {
                if (activarLinterna)
                {
                    await Flashlight.TurnOnAsync();
                }
                else
                {
                    await Flashlight.TurnOffAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

    }
}