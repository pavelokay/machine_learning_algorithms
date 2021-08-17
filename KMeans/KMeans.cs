using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KMeans
{
    public struct Centroid
    {
        public int[] Parameters { get; set; }
        public int Classification { get; set; }
    }

    public struct UnlabeledDistances
    {
        public int[] Parameters { get; set; }
        public double[] DistancesToCentroids { get; set; }
    }
    public class KMeans
    {
        private readonly int K;
        private readonly int IterationCount;
        public KMeans(int k, int iterationCount)
        {
            K = k;
            IterationCount = iterationCount;
        }
        public async Task<List<LabeledData>> ClusteringKMeans(List<UnlabeledData> unlabeledData)
        {
            int parametersCount = unlabeledData.FirstOrDefault().Parameters.Length;
            List<Centroid> centroids = InitializeKCentroids(K, parametersCount, unlabeledData);

            List<UnlabeledDistances> unlabeledDistances= new List<UnlabeledDistances>(); 
            foreach(var dataPoint in unlabeledData)
            {
                var unlebeledPoint = new UnlabeledDistances { Parameters = dataPoint.Parameters, DistancesToCentroids = new double[centroids.Count] };
                unlabeledDistances.Add(unlebeledPoint);
            }

            string unfile = "C:/Users/Pavel/testun.json";
            string file = "C:/Users/Pavel/test.json";
            string initCentroidFile = "C:/Users/Pavel/initCentroid.json";
            string centroidFile = "C:/Users/Pavel/centroid.json";
            await WriteUnlabeledDataFile(unfile, unlabeledData);
            await WriteUnlabeledDataFile(initCentroidFile, centroids);

            var labeledData = new List<LabeledData>();
            for (var i = 0; i < IterationCount; i++)
            {
                for (var j = 0; j < unlabeledDistances.Count; j++)
                {
                    unlabeledDistances[j] = (DistanceCalculation(centroids, unlabeledDistances[j]));
                }
                labeledData = FindClosestToCentroid(unlabeledDistances, centroids);
                centroids = RecalculateKCentroids(K, labeledData, parametersCount, centroids);

                await WriteUnlabeledDataFile(file, labeledData);
                await WriteUnlabeledDataFile(centroidFile, centroids);
            }

            return labeledData;

        }


        private List<Centroid> InitializeKCentroids(int k, int parametersCount, List<UnlabeledData> unlabeledData)
        {
            List<int> minParams = new List<int>();
            List<int> maxParams = new List<int>();
            for (var i = 0; i < parametersCount; i++)
            {
                minParams.Add(unlabeledData.Min(x => x.Parameters[i]));
                maxParams.Add(unlabeledData.Max(x => x.Parameters[i]));
            }
            Random rand = new Random();
            List<Centroid> kCentroids = new List<Centroid>();
            bool equalFlag = false;
            for (var i = 0; i < k; i++)
            {
                var centroid = new Centroid { Parameters = new int[parametersCount], Classification = i };
                for (var j = 0; j < parametersCount; j++)
                {
                    centroid.Parameters[j] = rand.Next(minParams[j], maxParams[j]);
                }

                for (var n = 0; n < kCentroids.Count; n++)
                {
                    if (centroid.Parameters.SequenceEqual(kCentroids[n].Parameters))
                    {
                        equalFlag = true;
                        break;
                    }
                }

                if (equalFlag)
                {
                    i--;
                    continue;
                }
                kCentroids.Add(centroid);
            }
            return kCentroids;
        }

        private List<Centroid> RecalculateKCentroids(int k, List<LabeledData> labeledData, int parametersCount, List<Centroid> previousCentroids)
        {
            List<Centroid> centroids = new List<Centroid>();
            for (var i = 0; i < k; i++)
            {
                var centroid = new Centroid { Parameters = new int[parametersCount], Classification = i };
                for (var j = 0; j < parametersCount; j++)
                {
                    if (labeledData.Count(x => x.Classification == i) != 0)
                    {
                        var classificationData = labeledData.Where(x => x.Classification == i);
                        centroid.Parameters[j] = classificationData.Sum(x => x.Parameters[j]) / labeledData.Count(x => x.Classification == i);
                    }
                    else
                    {
                        centroid.Parameters[j] = previousCentroids[i].Parameters[j];
                    }
                }
                centroids.Add(centroid);
            }
            return centroids;
        }

        private UnlabeledDistances DistanceCalculation(List<Centroid> centroids, UnlabeledDistances unlabeledDistances)
        {
            for (var i = 0; i < centroids.Count; i++)
            {
                unlabeledDistances.DistancesToCentroids[i] = EuclideanDistance(centroids[i].Parameters, unlabeledDistances.Parameters);
            }
            return unlabeledDistances;
        }
        private List<LabeledData> FindClosestToCentroid(List<UnlabeledDistances> unlabeledDistances, List<Centroid> centroids)
        {
            List<LabeledData> labeledDataList = new List<LabeledData>();
            for (var i = 0; i < unlabeledDistances.Count; i++)
            {
                double minDistance = unlabeledDistances[i].DistancesToCentroids.Min();
                int classification = Array.IndexOf(unlabeledDistances[i].DistancesToCentroids, minDistance);

                LabeledData labeledData = new LabeledData() {Parameters = new int[unlabeledDistances[i].Parameters.Length]};
                for (var j = 0; j < unlabeledDistances[i].Parameters.Length; j++)
                {
                    labeledData.Parameters[j] = unlabeledDistances[i].Parameters[j];
                }
                labeledData.Classification = classification;

                labeledDataList.Add(labeledData);
            }
            return labeledDataList;
        }
        private double EuclideanDistance(int[] firstPoint, int[] secondPoint)
        {
            double distance = 0.0;
            for (var i = 0; i < firstPoint.Length; i++)
            {
                distance += Math.Pow((firstPoint[i] - secondPoint[i]), 2);
            }
            return Math.Sqrt(distance);
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
    }
}
