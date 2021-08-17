using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace KNearest
{
    public class KNearestNeighbors
    {
        private readonly int k;
        private readonly List<TrainingData> trainingData;
        public KNearestNeighbors(int k, List<TrainingData> trainingData)
        {
            this.k = k;
            this.trainingData = trainingData;
        }
        public Dictionary<int, double> CalculateKNearestNeighbors(TestData testData)
        {
            var c = trainingData.Select(x => x.Classification).ToHashSet();
            return KNNClassifier(FindNeighbours(k, trainingData, testData), c);
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
        private NeighbourIndex[] FindNeighbours(int k, List<TrainingData> trainingData, TestData testData)
        {
            NeighbourIndex[] neighbourIndexes = new NeighbourIndex[trainingData.Count];

            int[] testPoint = testData.Parameters;

            for (var i = 0; i < trainingData.Count; i++)
            {
                NeighbourIndex neighbourIndex = new NeighbourIndex();

                neighbourIndex.Index = i;
                int[] trainPoint = trainingData[i].Parameters;
                if (trainPoint.Length != testPoint.Length)
                {
                    throw new Exception("parameters doesn't equal");
                }

                neighbourIndex.Distance = EuclideanDistance(trainPoint, testPoint);

                neighbourIndex.Classification = trainingData[i].Classification;

                neighbourIndexes[i] = neighbourIndex;
            }

            Array.Sort(neighbourIndexes);

            //NeighbourIndex[] kNearestNeighbours = new NeighbourIndex[k];

            //string testPointCoords = "Test point: (";
            //for (var i = 0; i < testPoint.Length; i++)
            //{
            //    testPointCoords += testPoint[i];
            //    if (i + 1 != testPoint.Length)
            //    {
            //        testPointCoords += ",";
            //    }
            //}
            //testPointCoords += ")";
            //Console.WriteLine(testPointCoords);

            //Console.WriteLine("Nearest   /  Distance  /  Class");
            //Console.WriteLine("==========================");
            //for (var i = 0; i < k; i++)
            //{
            //    int classification = neighbourIndexes[i].Classification;
            //    double dist = neighbourIndexes[i].Distance;

            //    string trainPointCoords = "(";
            //    for (var j = 0; j < trainingData[neighbourIndexes[i].Index].Parameters.Length; j++)
            //    {
            //        trainPointCoords += trainingData[neighbourIndexes[i].Index].Parameters[j];
            //        if (j + 1 != trainingData[neighbourIndexes[i].Index].Parameters.Length)
            //        {
            //            trainPointCoords += ",";
            //        }
            //    }
            //    trainPointCoords += ")";

            //    Console.WriteLine(trainPointCoords + "  :  " + Math.Round(dist, 3).ToString().PadRight(10) + classification);

            //    kNearestNeighbours[i] = neighbourIndexes[i];
            //}
            //return kNearestNeighbours;
            return neighbourIndexes.Take(k).ToArray();
        }

        private Dictionary<int, double> KNNClassifier(NeighbourIndex[] neighbourIndexes, HashSet<int> classes)
        {
            Dictionary<int, double> classProbabilities = new Dictionary<int, double>();

            for (var i = 0; i < classes.Count(); i++)
            {
                double classCount = neighbourIndexes.Count(x => x.Classification == classes.ElementAt(i));
                classProbabilities.Add(classes.ElementAt(i), classCount / neighbourIndexes.Count());  
            }

            //for (var i = 0; i < classes.Count(); i++)
            //{
            //    Console.WriteLine($"Class {classes.ElementAt(i)} probability is: {Math.Round(classProbabilities[classes.ElementAt(i)], 3)}");
            //}

            //Console.WriteLine();
            //Console.WriteLine();

            return classProbabilities;
        }
    }
}
