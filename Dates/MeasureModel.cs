using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorApp.Dates
{
    public class MeasureModel
    {
        /// <summary>
        /// X 轴数据
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Y 轴数据
        /// </summary>
        public double Value { get; set; }
    }
}