using System;

using Godot;


namespace ShipML
{
    public class Ship : RigidBody2D
    {
        private float _rightForce = 49f;
        private float _leftForce = 49f;
        
        private Node2D _rightThruster;
        private Node2D _leftThruster;

        public override void _Ready()
        {
            _rightThruster = GetNode<Node2D>("%ThrusterR");
            _leftThruster = GetNode<Node2D>("%ThrusterL");
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
