using System.Windows.Controls;

namespace VCDSLogsViewer
{
    /// <summary>
    /// Interaction logic for ChartControl.xaml
    /// </summary>
    public partial class ChartControl : UserControl
    {
        public ChartControl()
        {
            InitializeComponent();

            uiChart.AxisY = new LiveCharts.Wpf.AxesCollection();
        }
    }
}
