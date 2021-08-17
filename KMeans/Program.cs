using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace KMeans
{
    
    class Program
    {
        static async Task Main(string[] args)
        {
            string dataFile = "UnlabeledData.json";
            int elementsCount = 1000;
            int parametersCount = 3;
            int minParameterValue = -40;
            int maxParameterValue = 40;

            List<UnlabeledData> randomUnlabeledData = CreateRandomUnlabeledData(elementsCount, parametersCount, minParameterValue, maxParameterValue);
            await WriteUnlabeledDataFile(dataFile, randomUnlabeledData);
            //List<UnlabeledData> unlabeledData = new List<UnlabeledData>();
            var unlabeledData = await ReadUnlabeledDataFile<UnlabeledData>(dataFile);

            Console.WriteLine("Enter k count");
            var successK = int.TryParse(Console.ReadLine(), out int k);
            if (!successK)
            {
                throw new Exception("not number");
            }
            Console.WriteLine("Enter iteration count");
            var successIteration = int.TryParse(Console.ReadLine(), out int iterationCount);
            if (!successIteration)
            {
                throw new Exception("not number");
            }

            var kMeans = new KMeans(k, iterationCount);
            var labeledData = kMeans.ClusteringKMeans(unlabeledData).Result;
            string resultFile = "LabeledData.json";
            await WriteUnlabeledDataFile(resultFile, labeledData);

        }

        private static List<UnlabeledData> CreateRandomUnlabeledData(int elementsCount, int parametersCount, int minValue, int maxValue)
        {
            Random rand = new Random();
            List<UnlabeledData> unlabeledData = new List<UnlabeledData>();

            for (var i = 0; i < elementsCount; i++)
            {
                var dataItem = new UnlabeledData { Parameters = new int[parametersCount] };
                for (var j = 0; j < parametersCount; j++)
                {
                    dataItem.Parameters[j] = rand.Next(minValue, maxValue);
                }
                unlabeledData.Add(dataItem);
            }
            return unlabeledData;
        }
        private async static Task WriteUnlabeledDataFile<T>(string file, List<T> data)
        {
            if (String.IsNullOrWhiteSpace(file))
            {
                return;
            }

            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                await JsonSerializer.SerializeAsync<List<T>>(fs, data);
            }

        }

        private async static Task<List<T>> ReadUnlabeledDataFile<T>(string file)
        {
            if (String.IsNullOrWhiteSpace(file))
            {
                return null;
            }

            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                return await JsonSerializer.DeserializeAsync<List<T>>(fs);
            }
        }
    }
}
