using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace NeuralNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            double[][][] trainLib = new double[10][][];
            trainLib[0] = new double[2][];
            trainLib[0][0] = new double[] { 22, 64, 172 };
            trainLib[0][1] = new double[] { 0 };

            trainLib[1] = new double[2][];
            trainLib[1][0] = new double[] { 22, 81, 182 };
            trainLib[1][1] = new double[] { 0 };

            trainLib[2] = new double[2][];
            trainLib[2][0] = new double[] { 22, 55, 169 };
            trainLib[2][1] = new double[] { 1 };

            trainLib[3] = new double[2][];
            trainLib[3][0] = new double[] { 22, 51, 166 };
            trainLib[3][1] = new double[] { 1 };

            trainLib[4] = new double[2][];
            trainLib[4][0] = new double[] { 22, 100, 185 };
            trainLib[4][1] = new double[] { 0 };

            trainLib[5] = new double[2][];
            trainLib[5][0] = new double[] { 22, 58, 175 };
            trainLib[5][1] = new double[] { 1 };

            trainLib[6] = new double[2][];
            trainLib[6][0] = new double[] { 22, 96, 194 };
            trainLib[6][1] = new double[] { 0 };

            trainLib[7] = new double[2][];
            trainLib[7][0] = new double[] { 22, 111, 179 };
            trainLib[7][1] = new double[] { 0 };

            trainLib[8] = new double[2][];
            trainLib[8][0] = new double[] { 22, 45, 162 };
            trainLib[8][1] = new double[] { 1 };

            trainLib[9] = new double[2][];
            trainLib[9][0] = new double[] { 22, 65, 174 };
            trainLib[9][1] = new double[] { 1 };

            for (var i = 0; i < trainLib.Length; i++)
            {
                trainLib[i][0][0] -= 21;
                trainLib[i][0][1] -= 60;
                trainLib[i][0][2] -= 170;
            }
            int epochs = 5000;
            double learningRate = 0.1;

            var network = new FSNeuralNetwork(learningRate); 

            for (var i = 0; i < epochs; i++)
            {
                var inputs = new List<List<double>>();
                var correctPredictions = new List<double>();
                var predictions = new List<double>();
                for (var j = 0; j < trainLib.Length; j++)
                {
                    network.Train(trainLib[j][0], trainLib[j][1]);
                    inputs.Add(trainLib[j][0].ToList());
                    correctPredictions.Add(trainLib[j][1].FirstOrDefault());
                }

                for (var n = 0; n < trainLib.Length; n++)
                {
                    predictions.Add(network.Predict(inputs[n].ToArray()));
                }
                var trainLose = FSNeuralNetwork.MSE(predictions.ToArray(), correctPredictions.ToArray());
                Console.WriteLine($"Progress: {100 * i / epochs:N4}, Training loss: {trainLose:N5}");
                //Console.Clear();
            }



            for (var i = 0; i < trainLib.Length; i++)
            {
                Console.WriteLine($"For input: {string.Join(',', trainLib[i][0])} the prediction is {network.Predict(trainLib[i][0])}, expected: {trainLib[i][1].FirstOrDefault()}");
            }

            Console.WriteLine($"For input: 22, 85, 170 the prediction is {network.Predict(new double[] { 1, 30, -25 })}, expected: 1");
        }
    }
}
