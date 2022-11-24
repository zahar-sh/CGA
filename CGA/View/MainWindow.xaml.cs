using CGA.ViewModel;
using CGA1.Model;
using System.Windows;

namespace CGA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnRenderTypeChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                if (bresenhamRadioButton.IsChecked.GetValueOrDefault())
                {
                    viewModel.PainterType = PainterType.Bresenham;
                }
                else if (flatShadingRadioButton.IsChecked.GetValueOrDefault())
                {
                    viewModel.PainterType = PainterType.FlatShading;
                }
            }
        }

        private void OnLightingTypeChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                if (lambertRadioButton.IsChecked.GetValueOrDefault())
                {
                    viewModel.LightingType = LightingType.Lambert;
                }
                else if (phongRadioButton.IsChecked.GetValueOrDefault())
                {
                    viewModel.LightingType = LightingType.Phong;
                }
            }
        }
    }
}
