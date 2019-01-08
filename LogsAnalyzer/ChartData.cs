using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCDSLogsViewer
{
    public class ChartData
    {
        public DataValue Value { get; set; }
        public string Header => $"{Value.Group.Name}-{Value.Header}, {Value.Units}";
        public bool IsChecked { get; set; }
    }
}
