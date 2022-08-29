using System;

using Godot;


namespace ShipML
{
    public class Ship : RigidBody2D
    {
        public float RightForce
        {
            get => _rightForce;
            set => _rightForce = value;
        }

        public float LeftForce
        {
            get => _leftForce;
            set => _leftForce = value;
        }

        public float RightThrusterAngle
        {
            get => _rightThruster.Rotation;
            set => _rightThruster.Rotation = value;
        }
        
        public float LeftThrusterAngle
        {
            get => _leftThruster.Rotation;
            set => _leftThruster.Rotation = value;
        }

        private float _rightForce = 49f;
        private float _leftForce = 49f;

        private Node2D _rightThruster;
        private Node2D _leftThruster;

        public override void _Ready()
        {
            _rightThruster = GetNode<Node2D>("%ThrusterR");
            _leftThruster = GetNode<Node2D>("%ThrusterL");

            GetNode<ShipNeuralNetwork>("/root/ShipNeuralNetwork").Ship = this;
        }

        public override void _PhysicsProcess(float delta)
        {
            AppliedForce = Vector2.Zero;
            AppliedTorque = 0f;

            Vector2 rightForceVector = _rightForce * new Vector2(
                (float) Math.Sin(_rightThruster.GlobalRotation),
                (float) -Math.Cos(_rightThruster.GlobalRotation)
            );

            Vector2 leftForceVector = _leftForce * new Vector2(
                (float) Math.Sin(_leftThruster.GlobalRotation),
                (float) -Math.Cos(_leftThruster.GlobalRotation)
            );
            
            AddForce(_rightThruster.Position, rightForceVector);
            AddForce(_leftThruster.Position, leftForceVector);
        }
    }
}
