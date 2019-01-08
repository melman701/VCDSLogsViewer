using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VCDSLogsViewer
{
    public class DataParser
    {
        public List<DataValue> Parse(List<string> header, List<string> data)
        {
            var groups = GetGroups(header[0]);
            var values = CreateDataValues(header, groups);
            FillData(data, values);

            return values;
        }

        private List<Group> GetGroups(string groupLine)
        {
            List<Group> groups = new List<Group>();

            var cells = groupLine.Split(',');
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].StartsWith("Group"))
                {
                    Group g = new Group()
                    {
                        Column = i,
                        Name = cells[i + 1],
                    };

                    groups.Add(g);
                }
            }

            return groups;
        }

        private List<DataValue> CreateDataValues(List<string> dataHeader, List<Group> groups)
        {
            List<DataValue> values = new List<DataValue>();

            var headers = dataHeader[1].Split(',');
            var units = dataHeader[3].Split(',');
            foreach (var group in groups)
            {
                for (int i = group.Column + 1; i < group.Column + 5; i++)
                {
                    values.Add(new DataValue()
                    {
                        TsColumn = group.Column,
                        Group = group,
                        Column = i,
                        Header = headers[i],
                        Units = units[i]
                    });
                }
            }

            return values;
        }

        private void FillData(List<string> dataLines, List<DataValue> values)
        {
            foreach (var line in dataLines)
            {
                var cells = line.Split(',');
                foreach (var value in values)
                {
                    value.Add(float.Parse(cells[value.TsColumn]), float.Parse(cells[value.Column]));
                }
            }
        }
    }
}
