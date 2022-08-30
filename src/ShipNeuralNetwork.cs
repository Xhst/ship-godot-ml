using System;

using Godot;

using MathNet.Numerics.LinearAlgebra;


namespace ShipML
{
    public class ShipNeuralNetwork
    {
        private const float MaxForce = 1f;
        private const float MaxAngle = (float) Math.PI / 2;
        
        private const int InputLayerSize = 7;
        private const int FirstLayerSize = 9;
        private const int SecondLayerSize = 9;
        private const int OutputLayerSize = 4;

        private Vector<float> _input = Vector<float>.Build.Dense(InputLayerSize);
        private Vector<float> _output = Vector<float>.Build.Dense(OutputLayerSize);

        private readonly NeuralNetwork _network;
        private readonly Ship _ship;

        public ShipNeuralNetwork(Ship ship)
        {
            _ship = ship;
            _network = new NeuralNetwork(InputLayerSize, FirstLayerSize, SecondLayerSize, OutputLayerSize);
            
            SetInputFromShip();
        }

        public void FeedForward()
        {
            if (_ship is null) return;

            SetInputFromShip();
            _output = _network.FeedForward(_input);
            SetShipOutput();
        }

        private void SetInputFromShip()
        {
            _input[0] = _ship.NextTargetPosition.x - _ship.Position.x;
            _input[1] = _ship.NextTargetPosition.y - _ship.Position.y;
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