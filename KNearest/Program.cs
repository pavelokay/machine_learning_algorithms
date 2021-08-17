using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace KNearest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Файл с тренировочными данными
            string trainingFile = "TrainingData.json";
            List<TrainingData> trainingData = new List<TrainingData>();
            trainingData = await ReadFileAsync<TrainingData>(trainingFile);

            // Файл с тестовыми данными
            string testFile = "TestData.json";
            List<TestData> testData = new List<TestData>();
            testData = await ReadFileAsync<TestData>(testFile);

            // Ввод параметра k
            Console.Write("Enter parameter K (nearest's count): ");
            var successK = int.TryParse(Console.ReadLine(), out int k);
            if (!successK)
            {
                throw new Exception("k is not a number");
            }
            Console.WriteLine();

            var kNearest = new KNearestNeighbors(k, trainingData);
            var dataClassProbabilities = new List<Dictionary<int, double>>();
            var labeledTestData = new List<TrainingData>();

            // Определение класса для тренировочного объекта
            foreach (var testItem in testData)
            {
                var ItemClassProbability = kNearest.CalculateKNearestNeighbors(testItem);
                dataClassProbabilities.Add(ItemClassProbability);
                int clas = 0;
                Console.Write("For test point (");
                Array.ForEach(testItem.Parameters, x => Console.Write(x + " "));
                Console.WriteLine(")");
                foreach (var l in ItemClassProbability.Keys)
                {
                    Console.WriteLine($"Class {l} probability is: {Math.Round(ItemClassProbability[l], 3)}");
                    if (ItemClassProbability[l] > ItemClassProbability[clas]) clas = l;
                }
                Console.WriteLine();
                labeledTestData.Add(new TrainingData { Parameters = testItem.Parameters, Classification = clas });
            }

            string unfile = "C:/Users/Pavel/unlabeledPointsKNearest.json";
            string file = "C:/Users/Pavel/trainingKNearest.json";
            string testFile2 = "C:/Users/Pavel/labaledPointsKNearest.json";
            await WriteUnlabeledDataFile(unfile, testData);
            await WriteUnlabeledDataFile(file, trainingData);
            await WriteUnlabeledDataFile(testFile2, labeledTestData);


            Console.ReadLine();
        }

        private static async Task<List<T>> ReadFileAsync<T>(string file)
        {
            if (String.IsNullOrWhiteSpace(file))
            {
                return null;
            }

            using (FileStream fs = File.OpenRead(file))
            {
                return await JsonSerializer.DeserializeAsync<List<T>>(fs);
            }
        }

        private static async Task WriteUnlabeledDataFile<T>(string file, List<T> data)
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

    }

}
