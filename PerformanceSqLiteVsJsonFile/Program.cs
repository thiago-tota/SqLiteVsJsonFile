using BenchmarkDotNet.Running;
using PerformanceSqLiteVsJsonFile;
using System.Diagnostics;

internal class Program
{
    public static void Main(string[] args)
    {
        //NugetBenchmark();
        ManualBenchmark();
    }

    static void NugetBenchmark()
    {
        var summary = BenchmarkRunner.Run<DataPersistenceBenchmark>();
        Console.WriteLine(summary);
    }

    static void ManualBenchmark()
    {
        var data = new DataPersistenceBenchmark();

        data.InitializeStocks(100000);
        var time = new Stopwatch();

        Console.WriteLine("Starting data persist into SQLite database!");
        time.Restart();
        data.PersistDataIntoSqLite();
        time.Stop();
        Console.WriteLine($"Data persisted into SQLite database successfully in {time.Elapsed}!");

        Console.WriteLine("Starting data persist into JSON file!");
        time.Restart();
        data.PersistDataIntoJsonFile();
        time.Stop();
        Console.WriteLine($"Data persisted into JSON file successfully in {time.Elapsed}!");


        Console.WriteLine("Starting search into SQLite database!");
        time.Restart();
        var stock = data.SearchByCodeFromSqLite();
        time.Stop();
        if (stock != null)
        {
            Console.WriteLine($"Stock found in SQLite database - Code: {stock.Code}, Name: {stock.Name} in {time.Elapsed}!");
        }
        else
        {
            Console.WriteLine($"Stock not found in SQLite database in {time.Elapsed}!");
        }

        Console.WriteLine("Starting search into Json file!");
        time.Restart();
        data.SearchByCodeFromJsonFile();
        time.Stop();
        if (stock != null)
        {
            Console.WriteLine($"Stock found in Json file - Code: {stock.Code}, Name: {stock.Name} in {time.Elapsed}!");
        }
        else
        {
            Console.WriteLine($"Stock not found in Json file in {time.Elapsed}!");
        }

        Console.WriteLine("Starting search into Json file!");
        time.Restart();
        data.SearchByCodeFromJsonFileFileStream();
        time.Stop();
        if (stock != null)
        {
            Console.WriteLine($"Stock found in Json file using FileStream - Code: {stock.Code}, Name: {stock.Name} in {time.Elapsed}!");
        }
        else
        {
            Console.WriteLine($"Stock not found in Json file using FileStream in {time.Elapsed}!");
        }
    }
}