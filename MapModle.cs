using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei_3G_Parser
{
  public class MapModle
  {
    public Dictionary<string, string> Cells { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> BTS { get; set; } = new Dictionary<string, string>();

    public string NodeName { get; set; }
    public string FileName { get; set; }
  }
}
