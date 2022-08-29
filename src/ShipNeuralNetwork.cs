using System;

using Godot;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;


namespace ShipML
{
    public class ShipNeuralNetwork : Node
    {
        private const float MaxForce = 1f;
        private const float MaxAngle = (float) Math.PI / 2;
        
        private const int InputLayerSize = 7;
        private const int OutputLayerSize = 4;
        
        private const int FirstLayerSize = 8;
        private const int SecondLayerSize = 8;

        private Vector<float> _input = Vector<float>.Build.Dense(InputLayerSize);
        private Vector<float> _output = Vector<float>.Build.Dense(OutputLayerSize);

        private Matrix<float> _weights01 = Matrix<float>.Build.Random(FirstLayerSize, InputLayerSize);
        private Matrix<float> _weights12 = Matrix<float>.Build.Random(SecondLayerSize, FirstLayerSize);
        private Matrix<float> _weights23 = Matrix<float>.Build.Random(OutputLayerSize, SecondLayerSize);

        private Vector<float> _bias1 = Vector<float>.Build.Dense(FirstLayerSize);
        private Vector<float> _bias2 = Vector<float>.Build.Dense(SecondLayerSize);
        private Vector<float> _bias3 = Vector<float>.Build.Dense(OutputLayerSize);

        private Ship _ship;
        public Ship Ship
        {
            set
            {
                _ship = value;
                SetInputFromShip();
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            if (_ship is null) return;

            SetInputFromShip();
            FeedForward(_input);
        }

        private void FeedForward(Vector<float> input)
        {
            var temp = _weights01 * input + _bias1;
            temp.Map(x => SpecialFunctions.Logistic(x), Zeros.Include);

            temp = _weights12 * temp + _bias2;
            temp.Map(x => SpecialFunctions.Logistic(x), Zeros.Include);

            _output = _weights23 * temp + _bias3;
            _output.Map(x => SpecialFunctions.Logistic(x), Zeros.Include);

            SetShipOutput();
        }

        private void SetInputFromShip()
        {
            _input[0] = 512;
            _input[1] = 300;
            _input[2] = _ship.LinearVelocity.x;
            _input[3] = _ship.LinearVelocity.y;
            _input[4] = (float) Math.Cos(_ship.GlobalRotation);
            _input[5] = (float) Math.Sin(_ship.GlobalRotation);
            _input[6] = _ship.AngularVelocity;
        }

        private void SetShipOutput()
        {
            _ship.LeftForce = MaxForce * _output[0];
            _ship.LeftThrusterAngle = MaxAngle * (_output[1] * 2 - 1);
            _ship.RightForce = MaxForce * _output[2];
            _ship.RightThrusterAngle = MaxAngle * (_output[3] * 2 - 1);
        }
    }
}