using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.Lights;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace lipMe
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private bool _flashEnabled = false;
        private readonly DisplayRequest _displayRequest = new DisplayRequest();
        private MediaCapture _mediaCapture = new MediaCapture();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            _displayRequest.RequestActive();
            await _mediaCapture.InitializeAsync();
            //_mediaCapture.VideoDeviceController.Focus.TrySetAuto( true );


            


            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                _mediaCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
            }

            CameraPreview.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();

            /*SystemNavigationManager.GetForCurrentView().BackRequested += async (s, ev) =>
            {
                _displayRequest.RequestRelease();
                await _mediaCapture.StopPreviewAsync();
                _mediaCapture.Dispose();
                CoreApplication.Exit();
            };*/
        }

        private async Task TakePhotoAsync()
        {
            var stream = new InMemoryRandomAccessStream();

            try
            {
                await _mediaCapture.VideoDeviceController.FocusControl.FocusAsync();
                Debug.WriteLine("Taking photo...");
                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateBmp(), stream);
                Debug.WriteLine("Photo taken!");

                var decoder = await BitmapDecoder.CreateAsync(stream);

                SoftwareBitmap sBitmap = await decoder.GetSoftwareBitmapAsync();

                WriteableBitmap bitmap = new WriteableBitmap(sBitmap.PixelWidth, sBitmap.PixelHeight);

                sBitmap.CopyToBuffer(bitmap.PixelBuffer);
                sBitmap.Dispose();

                var buffer = bitmap.PixelBuffer;
                int center = ((bitmap.PixelWidth*( bitmap.PixelHeight/2 )) + bitmap.PixelWidth/2)*4;

                byte b = buffer.GetByte((uint)center);
                byte g = buffer.GetByte((uint)center + 1);
                byte r = buffer.GetByte((uint)center + 2);

                Debug.WriteLine("Red: " + r);
                Debug.WriteLine("Green: " + g);
                Debug.WriteLine("Blue: " + b);

                ///_displayRequest.RequestRelease();
                ///await _mediaCapture.StopPreviewAsync();
                ///_mediaCapture.Dispose();


                Frame.Navigate( typeof( ItemsPage ), new PixelColor()
                {
                    Red = r,
                    Green = g,
                    Blue= b
                } );
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception when taking a photo: {0}", ex.ToString());
            }

        }

        private void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            TakePhotoAsync();
        }

        private void HighlightButton_Click(object sender, RoutedEventArgs e)
        {
            _flashEnabled = !_flashEnabled;

            _mediaCapture.VideoDeviceController.FlashControl.Enabled = _flashEnabled;
            _mediaCapture.VideoDeviceController.FlashControl.AssistantLightEnabled = _flashEnabled;

            HighlightSymbol.Foreground = ( _flashEnabled ) ? new SolidColorBrush( Colors.LightCoral ) : new SolidColorBrush( Colors.Gray );
        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine( "Unloading" );
            _displayRequest.RequestRelease();
            await _mediaCapture.StopPreviewAsync();
            _mediaCapture.Dispose();
        }
    }
}
