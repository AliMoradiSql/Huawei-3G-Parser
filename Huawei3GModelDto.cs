using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huawei_3G_Parser
{
  public class Huawei3GModelDto
  {
    public string Node { get; set; }
    public string Class { get; set; }
    public string Path { get; set; }
    public string Site { get; set; }
    public string Cell { get; set; }
    public string Parameter { get; set; }
    public string Value { get; set; }
    public DateTime ValueTime { get; set; }
    public string CellID { get; set; }
    public string TRX { get; set; }
    public string FileInfo { get; set; }
    public string index { get; set; }
  }
}
