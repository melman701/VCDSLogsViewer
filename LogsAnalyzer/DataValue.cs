using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCDSLogsViewer
{
    public class DataValue
    {
        public Group Group { get; set; }
        public string Header { get; set; }
        public string Units { get; set; }
        public int Column { get; set; }
        public int TsColumn { get; set; }
        public int Count => data?.Count ?? 0;
        public IEnumerable<Tuple<float, float>> Data => data;

        private List<Tuple<float, float>> data;

        public void Add(float timestamp, float value)
        {
            if (data == null)
            {
                data = new List<Tuple<float, float>>();
            }

            data.Add(new Tuple<float, float>(timestamp, value));
        }
    }
}
