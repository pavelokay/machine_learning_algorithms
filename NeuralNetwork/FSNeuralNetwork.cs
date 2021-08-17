using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNetwork
{
    class FSNeuralNetwork
    {
        private double learningRate {get; }
        private double[,] weights_0_1 { get; }
        private double[,] weights_1_2 { get; }

        //private double[] bias { get; }

        public FSNeuralNetwork(double learningRate = 0.1)
        {
            this.learningRate = learningRate;
            weights_0_1 = new double[2, 3];
            weights_1_2 = new double[1, 2];
            Random random = new Random();
            for (var i = 0; i < 3; i++)
            {
                weights_0_1[0, i] = random.NextDouble();
                weights_0_1[1, i] = random.NextDouble();
            }
            for (var i = 0; i < 2; i++)
            {
                weights_1_2[0, i] = random.NextDouble();
            }
        }

        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public double Predict(double[] inputs)
        {
            double[] inputs_1 = new double[2];
            double[] outputs_1 = new double[2];

            double[] inputs_2 = new double[1];
            double[] outputs_2 = new double[1];

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    inputs_1[i] += inputs[j] * weights_0_1[i, j]; 
                }
                outputs_1[i] = Sigmoid(inputs_1[i]);
            }

            for (var i = 0; i < 1; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    inputs_2[i] += outputs_1[j] * weights_1_2[i, j];
                }
                outputs_2[i] = Sigmoid(inputs_2[i]);
            }
            return outputs_2[0];
        }
        public void Train(double[] inputs, double[] expectedPredict)
        {
            double[] inputs_1 = new double[2];
            double[] outputs_1 = new double[2];

            double[] inputs_2 = new double[1];
            double[] outputs_2 = new double[1];

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    inputs_1[i] += inputs[j] * weights_0_1[i, j];
                }
                outputs_1[i] = Sigmoid(inputs_1[i]);
            }

            for (var i = 0; i < 1; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    inputs_2[i] += outputs_1[j] * weights_1_2[i, j];
                }
                outputs_2[i] = Sigmoid(inputs_2[i]);
            }

            double nnPredict = outputs_2[0];

            double[] errorLayer2 = new double[1];
            double[] derivSigmoidLayer2 = new double[1];
            double[] weightDeltaLayer2 = new double[1];
            errorLayer2[0] = nnPredict - expectedPredict[0];
            derivSigmoidLayer2[0] = nnPredict * (1 - nnPredict);
            weightDeltaLayer2[0] = errorLayer2[0] * derivSigmoidLayer2[0];


            for (var i = 0; i < weights_1_2.Length; i++)
            {
                weights_1_2[0, i] -= outputs_1[i] * weightDeltaLayer2[0] * learningRate;
            }

            double[] errorLayer1 = new double[2];
            double[] derivSigmoidLayer1 = new double[2];
            double[] weightDeltaLayer1 = new double[2];
            for (var i = 0; i < 2; i++)
            {
                errorLayer1[i] = weightDeltaLayer2[0] * weights_1_2[0, i];
                derivSigmoidLayer1[i] = outputs_1[i] * (1 - outputs_1[i]);
                weightDeltaLayer1[i] = errorLayer1[i] * derivSigmoidLayer1[i];
            }

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    weights_0_1[i, j] -= inputs[j] * weightDeltaLayer1[i] * learningRate;
                }
            }

        }

        public static double MSE(double[] x, double[] y)
        {
            var predictions = new double[x.Length];
            for (var i = 0; i < x.Length; i++)
            {
                predictions[i] = Math.Pow(x[i] - y[i], 2);
            }
            return predictions.Average();
        }

    }
}
