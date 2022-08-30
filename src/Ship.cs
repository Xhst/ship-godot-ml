using System;

using Godot;


namespace ShipML
{
    public class Ship : RigidBody2D
    {
        public float RightForce
        {
            set => _rightForce = value;
        }

        public float LeftForce
        {
            set => _leftForce = value;
        }

        public float RightThrusterAngle
        {
            set => _rightThruster.Rotation = value;
        }
        
        public float LeftThrusterAngle
        {
            set => _leftThruster.Rotation = value;
        }

        private float _rightForce = 49f;
        private float _leftForce = 49f;

        private Node2D _rightThruster;
        private Node2D _leftThruster;

        private ShipNeuralNetwork _shipNeuralNetwork;

        public Vector2 NextTargetPosition;
        public int NextTargetId = 0;
        public int LastTargetId;

        public float DurationTime = 0f;

        [Signal]
        public delegate void ShipDestroyed();

        [Signal]
        public delegate void AllTargetReached();
        

        public override void _Ready()
        {
            _rightThruster = GetNode<Node2D>("%ThrusterR");
            _leftThruster = GetNode<Node2D>("%ThrusterL");
            _shipNeuralNetwork = new ShipNeuralNetwork(this);
        }

        public override void _PhysicsProcess(float delta)
        {
            _shipNeuralNetwork.FeedForward();
            
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

            StatusCheck();
        }

        private void StatusCheck()
        {
            if (Position.x > 1200 || Position.x < -100 || Position.y > 650 || Position.y < -100)
            {
                EmitSignal(nameof(ShipDestroyed), this);
            }

            if (NextTargetId > LastTargetId)
            {
                EmitSignal(nameof(AllTargetReached));
            }
        }
    }
}
