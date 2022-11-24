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

        private void Bresenham_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                if (bresenhamRadoiButton.IsChecked.GetValueOrDefault())
                {
                    viewModel.PainterType = PainterType.Bresenham;
                }
                else if (flatShadingRadoiButton.IsChecked.GetValueOrDefault())
                {
                    viewModel.PainterType = PainterType.FlatShading;
                }
            }
        }
    }
}
