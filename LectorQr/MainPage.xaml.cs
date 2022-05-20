using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


using System.IO;
using System.Reflection;
using Xamarin.Essentials;
using ZXing.Net.Mobile.Forms;
using ZXing.Mobile;
using ZXing;
using LectorQr.Vista;

namespace LectorQr
{
    public partial class MainPage : ContentPage
    {
        bool activarLinterna = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private void swhitchLinterna_Toggled(object sender, ToggledEventArgs e)
        {
         //   activarLinterna = swhitchLinterna.IsToggled;
            EncenderApagarLinterna();
        }

        public void PlaySonido()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream audioSream = assembly.GetManifestResourceStream("LectorQr.sonido.Beep.mp3");
            var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Load(audioSream);
            audio.Play();
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
            }catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            } 
        }

        private async void btnLeerQr_Clicked(object sender, EventArgs e)
        {
            try
            {
                EscanearCodigo();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message.ToString(), "OK");
            }
            

        }

        private async void EscanearCodigo()
        {
            var overlay = new ZXingDefaultOverlay
            {
                ShowFlashButton = false,
                TopText = "Coloca del Código  frente al dispositivo",
                BottomText = "El escaneo es automático",
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


            await Navigation.PushModalAsync(page);

            page.OnScanResult += (result) =>
            {
                page.IsScanning = false;
                page.AutoFocus(0, 500);
                //inicia un hilo en espera
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopModalAsync();
                    //lblLecturaQr.Text = result.Text;
                    PlaySonido();
                });
            };


            
            
            
           

            //if (result != null)
            //{
            //    lblLecturaQr.Text = result.Text;
            //     PlaySonido();
            //}
            //if (activarLinterna)
            //{
            //    Device.BeginInvokeOnMainThread(delegate { escanearPagina.ToggleTorch(); });
            //    swhitchLinterna.IsToggled = false;
            //    activarLinterna=false;
            //}

            //escanearPagina.OnScanResult += (result) =>
            //{
            //    escanearPagina.IsScanning = false;
            //    escanearPagina.AutoFocus(0, 500);
            //    //inicia un hilo en espera
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        Navigation.PopModalAsync();
            //        lblLecturaQr.Text = result.Text;
            //        PlaySonido();
            //    });
            //};

            //await Navigation.PopModalAsync(escanearPagina);


        }

        private async void btnentraga_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new entrada());
        }

        private async void btnsalida_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new salida());
        }
    }
}
