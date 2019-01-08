using LiveCharts;
using LiveCharts.Geared;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VCDSLogsViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate bool Checker(string line);

        private List<string> header = new List<string>();
        private List<ChartData> chartDatas;

        public MainWindow()
        {
            InitializeComponent();
            uiButtonSelectFile.Click += UiButtonSelectFile_Click;
            uiButtonUpdate.Click += UiButtonUpdate_Click;
        }

        private void UiButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            List<string> units = new List<string>();
            SeriesCollection charts = new SeriesCollection();
            foreach (var chartData in chartDatas)
            {
                if (chartData.IsChecked)
                {
                    var i = units.FindIndex(x => x.Equals(chartData.Value.Units));
                    if (i == -1)
                    {
                        units.Add(chartData.Value.Units);
                        i = units.Count - 1;
                    }
                    var chart = new GLineSeries()
                    {
                        Values = new ChartValues<float>(chartData.Value.Data.Select(x => x.Item2).ToList()),
                        DataLabels = false,
                        LineSmoothness = 0,
                        ScalesYAt = i,
                    };
                    charts.Add(chart);
                }
            }
            uiChartControl.uiChart.Series = charts;

            AxesCollection axes = new AxesCollection();
            AxisPosition pos = AxisPosition.LeftBottom;
            int colorIndex = 0;
            foreach (var unit in units)
            {
                axes.Add(new Axis
                {
                    Title = unit,
                    Position = pos,
                    ShowLabels = true,
                    Foreground = new SolidColorBrush(CartesianChart.Colors[colorIndex]),
                });
                if (pos == AxisPosition.LeftBottom)
                {
                    pos = AxisPosition.RightTop;
                }
                colorIndex++;
            }
            uiChartControl.uiChart.AxisY = axes;
        }

        private void UiButtonSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() ?? false)
            {
                uiTextBoxFileName.Text = openFileDialog.FileName;
                var lines = LoadFile(uiTextBoxFileName.Text);
                var (header, data) = SplitLines(lines, (string line) => {
                    return line == string.Empty;
                });

                this.header = header;
                uiTextBlockInfo.Text = string.Join("\n", header);

                chartDatas = ParseData(data);
                uiListData.ItemsSource = chartDatas;
            }
        }

        private List<string> LoadFile(string filePath)
        {
            List<string> lines = new List<string>();
            using (var fs = new StreamReader(filePath, Encoding.GetEncoding(1251)))
            {
                while (!fs.EndOfStream)
                {
                    var line = fs.ReadLine();
                    lines.Add(line);
                }
            }
            return lines;
        }

        private List<ChartData> ParseData(List<string> dataLines)
        {
            var (dhead, dvalues) = SplitLines(dataLines, (string line) =>
            {
                return line.StartsWith("Marker");
            });

            Debug.WriteLine(string.Join("\n", dhead));

            DataParser parser = new DataParser();
            var data = parser.Parse(dhead, dvalues);

            List<ChartData> chartDatas = new List<ChartData>();
            foreach (var value in data)
            {
                chartDatas.Add(new ChartData()
                {
                    Value = value,
                });
            }

            return chartDatas;
        }

        private static (List<string>, List<string>) SplitLines(List<string> lines, Checker checker)
        {
            List<string> first = new List<string>();
            List<string> second = new List<string>();

            int index = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (checker(lines[i]))
                {
                    index = i + 1;
                    break;
                }
            }

            if (index > -1)
            {
                first = lines.GetRange(0, index);
                second = lines.GetRange(index, lines.Count - index);
            }

            return (first, second);
        }
    }
}
