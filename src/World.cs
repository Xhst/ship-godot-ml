using Godot;
using Godot.Collections;

using ShipML;


public class World : Node2D
{
    private const float GenerationDuration = 30f;
    private const int Individuals = 30;
    
    [Export] private PackedScene _shipScene;
    [Export] private PackedScene _targetScene;

    private readonly Vector2 _shipSpawnPoint = new Vector2(512, 450);
    private Vector2[] _targetsPositions;
    private Ship[] _ships;

    private Timer _timer;

    public override void _Ready()
    {
        _ships = new Ship[Individuals];
        
        _targetsPositions = new[] {
            new Vector2(512, 300),
            new Vector2(700, 500),
            new Vector2(200, 500),
            new Vector2(1000, 200),
            new Vector2(500, 400),
            new Vector2(100, 300),
        };

        for (int i = 0; i < _targetsPositions.Length; i++)
        {
            Area2D target = _targetScene.Instance<Area2D>();
            AddChild(target);
            target.Position = _targetsPositions[i];
            target.Connect("body_entered", this, nameof(OnBodyEntered), new Array() { i });
        } 
        
        for (int i = 0; i < Individuals; i++)
        {
            Ship ship = _shipScene.Instance<Ship>();
            AddChild(ship);
            ship.Position = _shipSpawnPoint;
            ship.NextTargetPosition = _targetsPositions[0];
            ship.LastTargetId = _targetsPositions.Length;
            ship.Connect(nameof(Ship.ShipDestroyed), this, nameof(OnShipDestroyed));
            ship.Connect(nameof(Ship.AllTargetReached), this, nameof(OnAllTargetReached));
            _ships[i] = ship;
        }

        StartTimer();
    }

    private async void StartTimer()
    {
        _timer = new Timer {WaitTime = GenerationDuration, Paused = false, Autostart = true};
        AddChild(_timer);
        
        await ToSignal(_timer, "timeout");
        
        OnGenerationTimerTimeout();
    }

    private void OnGenerationTimerTimeout()
    {
        foreach (Ship ship in _ships)
        {
            GD.Print(ship.DurationTime);
        }
    }

    private float CalculateShipScore(Ship ship)
    {
        /*
         * (t1, n1) > (t2, n2) <=> n1 > n2 | n1 = n2 & t1 < t2
         */
        
        return 0f;
    }

    public void OnShipDestroyed(Ship ship)
    {
        ship.DurationTime = GenerationDuration - _timer.TimeLeft;
        RemoveChild(ship);
    }
    
    public void OnAllTargetReached()
    {
    }

    public void OnBodyEntered(Node node, int targetId)
    {
        if (node is Ship ship)
        {
            if (targetId == ship.NextTargetId)
            {
                ship.NextTargetId++;

                if (targetId < _targetsPositions.Length)
                {
                    ship.NextTargetPosition = _targetsPositions[targetId];
                }
            }
        }
    }
}
