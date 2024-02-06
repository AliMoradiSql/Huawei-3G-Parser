using FastMember;
using Huawei_3G_Parser;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;

internal class Program
{
  public static async Task Main(string[] args)
  {

    var path = "C:\\Users\\Ali\\Desktop\\Huawei 2G3G, For Mr. Ali\\3G sample\\Current";
    var FilePaths = new DirectoryInfo(path).GetFiles().ToList();

    MapModle mapModle = new MapModle();
    //var options = new ParallelOptions()
    //{
    //  MaxDegreeOfParallelism = 2
    //};
    //await Parallel.ForEachAsync(FilePaths, options, async (i,c) => {
    //  mapModle = i.GnareteMapeer();
    //  result.AddRange(i.GenarateData(mapModle));
    //});

    foreach (var file in FilePaths)
    {
      List<Huawei3GModelDto> result = new();

      var map = file.GnareteMapeer();
      result.AddRange(await file.GenarateData(map));
      await InsertDataToDataBase.InsertAsync(result);
    }
  }
}
