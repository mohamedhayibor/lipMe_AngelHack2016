using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace lipMe
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemsPage : Page
    {

        private bool _filterOpen = false;

        private List< Product > _allProductsList = new List< Product >();
        public ObservableCollection<Product> ProductsList { get; set; }

        private PixelColor _pixelColor;

        private DispatcherTimer _sliderTimer;

        public ItemsPage()
        {
            this.InitializeComponent();

            GetAllProducts();
            


            ProductsList = new ObservableCollection< Product >();
            ProductsListView.ItemsSource = ProductsList;

            _sliderTimer = new DispatcherTimer() {Interval = TimeSpan.FromMilliseconds( 300 )};
            _sliderTimer.Tick += SliderTimer_Tick;
            _sliderTimer.Stop();



            SystemNavigationManager.GetForCurrentView().BackRequested += ( s, e ) =>
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame == null)
                    return;

                // Navigate back if possible, and if the event has not 
                // already been handled .
                if (rootFrame.CanGoBack && e.Handled == false)
                {
                    e.Handled = true;
                    _allProductsList.Clear();
                    ProductsList.Clear();
                    rootFrame.GoBack();
                }
            };

            
        }

        

        protected override void OnNavigatedTo( NavigationEventArgs e )
        {
            base.OnNavigatedTo( e );

            _pixelColor = e.Parameter as PixelColor;

            DividerRectangle.Fill = new SolidColorBrush(Color.FromArgb( 255, _pixelColor.Red, _pixelColor.Green, _pixelColor.Blue ));
            FilterPanel.BorderBrush = new SolidColorBrush(Color.FromArgb(255, _pixelColor.Red, _pixelColor.Green, _pixelColor.Blue));

            UpdateColors();


            DisplayProducts();
        }

        private void GetAllProducts()
        {
            string[] lines = File.ReadAllLines("lipMeList.csv");

            foreach ( string line in lines )
            {
                string[] splits = line.Split(',');
                if (splits.Length != 9) continue;

                _allProductsList.Add(new Product()
                {
                    Name = splits[0],
                    Company = splits[1],
                    Type = (Product.ProductTypeEnum) Enum.Parse( typeof(Product.ProductTypeEnum), splits[2] ),
                    Color = new SolidColorBrush( Color.FromArgb( 255, Byte.Parse( splits[3] ), Byte.Parse(splits[4]), Byte.Parse(splits[5]))),
                    Price = splits[6],
                    ProductUrl = splits[7],
                    ImageUrl = splits[8]
                });
            }


        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            _filterOpen = !_filterOpen;


            if ( _filterOpen )
            {
                FilterPanel.Visibility = Visibility.Visible;
                FilterSymbolIcon.Foreground = new SolidColorBrush(Colors.LightCoral);
                UpdateColors();
            }
            else
            {
                FilterPanel.Visibility = Visibility.Collapsed;
                FilterSymbolIcon.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void BrightButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pixelColor.Red + 10 > 255 || _pixelColor.Green + 10 > 255 || _pixelColor.Blue + 10 > 255)
            {
                if ( _pixelColor.Red + 1 > 255 || _pixelColor.Green + 1 > 255 || _pixelColor.Blue + 1 > 255 ) return;

                _pixelColor.Red += 1;
                _pixelColor.Green += 1;
                _pixelColor.Blue += 1;
                UpdateColors();
                return;
            }

            _pixelColor.Red+= 10;
            _pixelColor.Green+= 10;
            _pixelColor.Blue+= 10;
            UpdateColors();
        }

        private void DimButton_Click(object sender, RoutedEventArgs e)
        {
            if ( _pixelColor.Red - 10 < 0 || _pixelColor.Green - 10 < 0 || _pixelColor.Blue - 10 < 0 )
            {
                if ( _pixelColor.Red - 1 < 0 || _pixelColor.Green - 1 < 0 || _pixelColor.Blue - 1 < 0 ) return;

                _pixelColor.Red -= 1;
                _pixelColor.Green -= 1;
                _pixelColor.Blue -= 1;
                UpdateColors();
                return;
            }

            _pixelColor.Red-=10;
            _pixelColor.Green-=10;
            _pixelColor.Blue-=10;
            UpdateColors();
        }

        private void UpdateColors()
        {
            RedSlider.Value = _pixelColor.Red;
            GreenSlider.Value = _pixelColor.Green;
            BlueSlider.Value = _pixelColor.Blue;

            DividerRectangle.Fill = new SolidColorBrush(Color.FromArgb(255, _pixelColor.Red, _pixelColor.Green, _pixelColor.Blue));
            FilterPanel.BorderBrush = new SolidColorBrush(Color.FromArgb(255, _pixelColor.Red, _pixelColor.Green, _pixelColor.Blue));
        }

        

        private void DisplayProducts()
        {
            _allProductsList.Sort((a, b) => _pixelColor.GetColorDistance(a).CompareTo(_pixelColor.GetColorDistance(b)));

            ProductsList.Clear();

            int count = 0;
            foreach (Product p in _allProductsList)
            {
                switch ( ItemTypeComboBox.SelectedIndex )
                {
                    case 1:
                        if ( p.Type != Product.ProductTypeEnum.Lipstick ) continue;
                        break;
                    case 2:
                        if (p.Type != Product.ProductTypeEnum.Eyeshadow) continue;
                        break;
                    case 3:
                        if (p.Type != Product.ProductTypeEnum.NailPolish) continue;
                        break;
                }
                ProductsList.Add(p);
                count++;

                if ( count == 25 ) break;
            }
        }

        private void RedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _pixelColor.Red = (byte)RedSlider.Value;
            UpdateColors();
            _sliderTimer.Interval = TimeSpan.FromMilliseconds( 300 );
            _sliderTimer.Start();

        }

        private void SliderTimer_Tick(object sender, object e)
        {
            DisplayProducts();
            _sliderTimer.Stop();
        }

        private void GreenSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _pixelColor.Green = (byte) GreenSlider.Value;
            UpdateColors();
            _sliderTimer.Interval = TimeSpan.FromMilliseconds(300);
            _sliderTimer.Start();
        }

        private void BlueSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _pixelColor.Blue = (byte)BlueSlider.Value;
            UpdateColors();
            _sliderTimer.Interval = TimeSpan.FromMilliseconds(300);
            _sliderTimer.Start();
        }

        private void CameraButton_Click(object sender, RoutedEventArgs e)
        {
            _allProductsList.Clear();
            ProductsList.Clear();
            Frame.Navigate( typeof( MainPage ) );
        }

        private void ItemTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DisplayProducts();
            } catch { }
            
        }

        private async void ProductsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Product p = e.ClickedItem as Product;
            await Windows.System.Launcher.LaunchUriAsync(new Uri( p.ProductUrl ));
        }
    }
}
