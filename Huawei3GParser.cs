using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Huawei_3G_Parser
{
  public static class Huawei3GParser
  {

    public static async Task<List<Huawei3GModelDto>> GenarateData(this FileInfo file, MapModle map)
    {
      string[] Check = { "ADD ", "SET ", "MOD ", "UIN " };
      List<Huawei3GModelDto> result = new();
      using TextReader reder = new StreamReader(file.FullName);
      string line;
      var count = 1;
      while ((line = reder.ReadLine()) != null)
      {
        string _site = string.Empty, _cell = string.Empty, _cellId = string.Empty;
        string _guid = Guid.NewGuid().ToString();
        string _class = string.Empty;

        if (!line.StartsWith("//") && Check.Any(x => line.Contains(x)))
        {
          _class = line.Substring(4, line.IndexOf(':') - 4);
          string[] param = line.Substring(line.IndexOf(':') + 1).Replace("\"", "").Replace(";", "").Split(',');
          if (param != null)
          {
            if (param.Any(x => x.Contains("BTSID=")))
            {
              var t = param.First(x => x.Contains("BTSID=")).Split('=');
              _site = map.BTS.FirstOrDefault(x => x.Key == t[1]).Value;
            }

            if (param.Any(x => x.StartsWith("CELLID=")))
            {
              var tmp = param.First(x => x.Contains("CELLID=")).Split('=');
              var mapedCell = map.Cells.FirstOrDefault(x => x.Key == tmp[1]).Value;

              _cellId = map.Cells.FirstOrDefault(x => x.Key == tmp[1]).Value != null ? tmp[1] : map.NCells.FirstOrDefault(x => x.Key == tmp[1]).Value;

              _cell = map.Cells.FirstOrDefault(x => x.Key == _cellId).Value?.Cell;
              _site = map.Cells.FirstOrDefault(x => x.Key == _cellId).Value?.RNC;

            }

            for (int i = 0; i < param.Length; i++)
            {
              string[] pvalue = param[i].Split('=');
              var c = pvalue.Length;
              if (pvalue.Length > 2)
              {
                throw new Exception("More than 2 = sign found");
              }


              Huawei3GModelDto obj = new()
              {
                Node = map.NodeName,
                FileInfo = map.FileName,
                CellID = string.IsNullOrEmpty(_cellId) ? null : _cellId,
                Cell = string.IsNullOrEmpty(_cell) ? null : _cell,
                Site = string.IsNullOrEmpty(_site) ? null : _site,
                Class = _class,
                Parameter = pvalue[0],
                Value = pvalue[1],
                TRX = null,
                index = _guid,
                ValueTime = DateTime.Now,
                Path = _class + "/" + "CellId=" + _cellId + "/" + "TRXNAME=" + null + ">" + pvalue[0]

              };

              result.Add(obj);
              if (result.Count > 2000000)
              {
                await InsertDataToDataBase.InsertAsync(result);
                result.Clear();

              }
            }
          }


        }
        else
        {
          count++;
        }


      }

      Console.WriteLine(count.ToString());
      //count = 0;

      return result;
    }

    public static MapModle GnareteMapeer(this FileInfo file)
    {
      var dic = new Dictionary<string, string>();
      MapModle map = new();
      string key, value = string.Empty;

      using (TextReader reder = new StreamReader(file.FullName))
      {
        string line;

        var count = 1;
        while ((line = reder.ReadLine()) != null)
        {
          if (line.Contains("SYSOBJECTID"))
          {

            Match match = Regex.Match(line, @"R\d+(H|W)");
            if (match.Success)
            {
              map.FileName = file.Name;
              map.NodeName = match.Value.Trim();
            }

          }

          if (line.Contains("ADJNODE:"))
          {

            string[] param = line.Replace("\"", "").Replace(";", "").Split(',');
            if (param != null)
            {

              key = param.FindKeyOrValue("ANI=");
              value = param.FindKeyOrValue("NAME=");
              //I Add these line for preventing error I must Remove them and empty btsDic per file
              if (map.BTS.Any(x => x.Key == key))
                continue;
              map.BTS.Add(key, value);
            }


          }

          if (line.Contains(" UCELLSETUP:"))
          {

            string[] param = line.Replace("\"", "").Replace(";", "").Split(',');
            if (param != null)
            {

              key = param.FindKeyOrValue("CELLID=");
              value = param.FindKeyOrValue("CELLNAME=");
              var rnc = param.FindKeyOrValue("NODEBNAME=");
              //I Add these line for preventing error I must Remove them and empty btsDic per file
              if (map.Cells.Any(x => x.Key == key))
                continue;
              map.Cells.Add(key, new CellName2RNCMap { Cell = value, RNC = rnc });
            }

          }
          if (line.Contains(" UINTRAFREQNCELL:"))
          {

            string[] param = line.Replace("\"", "").Replace(";", "").Split(',');
            if (param != null)
            {

              value = param.FindKeyOrValue("CELLID=");
              key = param.FindKeyOrValue("NCELLID=");
              //I Add these line for preventing error I must Remove them and empty btsDic per file
              if (map.NCells.Any(x => x.Key == key))
                continue;
              map.NCells.Add(key, value);
            }

          }

        }
        reder.Dispose();
      }

      //5800
      //7000
      return map;
    }

    public static string FindKeyOrValue(this string[] input, string searchKey)
    {
      var obj = input.FirstOrDefault(x => x.Contains(searchKey));
      return obj.Substring(obj.IndexOf('=') + 1, (obj.Length - obj.IndexOf('=')) - 1);
    }
  }
}
