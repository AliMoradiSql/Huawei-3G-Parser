using FastMember;
using Huawei_3G_Parser;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;

internal class Program
{
  public static async Task Main(string[] args)
  {
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    var path = "C:\\Users\\Ali\\Desktop\\Huawei 2G3G, For Mr. Ali\\3G sample";
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
    var myConnection = "Server=DESKTOP-6160J7F\\Sql2022; Database=IdentityServerDb; Trusted_Connection=True; TrustServerCertificate=True;";
    foreach (var file in FilePaths)
    {
      List<Huawei3GModelDto> result = new();
      var map = file.GnareteMapeer();
      result.AddRange(file.GenarateData(map));




      // Pass connection string to sql connection
      using (SqlConnection connection = new SqlConnection(myConnection))
      {

        // open connection and begin transaction
        connection.Open();
        using (SqlTransaction transaction = connection.BeginTransaction())
        {
          //create bulkCopy object to map columns from list to table and set batchsize
          using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            try
            {

              // we should pass all data as a list hear
              using (var reader = ObjectReader.Create(result))
              {

                sqlBulkCopy.BatchSize = 100000; // 20  s
                sqlBulkCopy.DestinationTableName = "Huawei3GNew ";
                sqlBulkCopy.ColumnMappings.Add("Node", "Node");
                sqlBulkCopy.ColumnMappings.Add("Path", "Path");
                sqlBulkCopy.ColumnMappings.Add("ValueTime", "ValueTime");
                sqlBulkCopy.ColumnMappings.Add("Site", "Site");
                sqlBulkCopy.ColumnMappings.Add("Cell", "Cell");
                sqlBulkCopy.ColumnMappings.Add("Class", "Class");
                sqlBulkCopy.ColumnMappings.Add("Parameter", "Parameter");
                sqlBulkCopy.ColumnMappings.Add("Value", "Value");
                sqlBulkCopy.ColumnMappings.Add("CellID", "CellID");
                sqlBulkCopy.ColumnMappings.Add("TRX", "TRX");
                sqlBulkCopy.ColumnMappings.Add("FileInfo", "FileInfo");
                sqlBulkCopy.ColumnMappings.Add("index", "index");
                sqlBulkCopy.WriteToServer(reader);

              }

              transaction.Commit();

            }
            catch (Exception e)
            {

              transaction.Rollback();
              throw e;

            }
            finally
            {
              connection.Close();
              Console.WriteLine(stopwatch.Elapsed);
              Debug.WriteLine(stopwatch.Elapsed);
            }

        }
      }
    }
  }
}
