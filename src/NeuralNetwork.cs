using System.Linq;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;


namespace ShipML
{
    public class NeuralNetwork
    {
        private int[] _layersNeuronsCount;
        private Matrix<float>[] _weights;
        private Vector<float>[] _biases;
        

        public NeuralNetwork(params int[] neuronsCount)
        {
            _layersNeuronsCount = neuronsCount;

            _weights = new Matrix<float>[neuronsCount.Length - 1];
            _biases = new Vector<float>[neuronsCount.Length - 1];

            for (int i = 0; i < neuronsCount.Length - 1; i++)
            {
                _weights[i] = Matrix<float>.Build.Random(neuronsCount[i+1], neuronsCount[i]);
                _biases[i] = Vector<float>.Build.Dense(neuronsCount[i+1]);
            }
        }

        public Vector<float> FeedForward(Vector<float> input)
        {
            Vector<float> temp = input;

            for (int i = 0; i < _layersNeuronsCount.Length - 1; i++)
            {
                temp = _weights[i] * temp + _biases[i];
                
                temp.Map(x => SpecialFunctions.Logistic(x), Zeros.Include);

            }

            return temp;
        }

    }
}