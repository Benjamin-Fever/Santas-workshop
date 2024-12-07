using Godot;
using System;

[GlobalClass]
public partial class GameInput : Node {
	[Signal] public delegate void MoveDirectionEventHandler(Vector2 direction);
	[Signal] public delegate void InteractPressedEventHandler();
	[Signal] public delegate void ActionPressedEventHandler();

	public Vector2 GetMovementDirection(){
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		return direction.Normalized();
	}

    public override void _UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("interact")){
			EmitSignal(SignalName.InteractPressed);
		}
		else if (@event.IsActionPressed("action")){
			EmitSignal(SignalName.ActionPressed);
		}
    }

	private Vector2 _prevDirection = Vector2.Zero;
    public override void _Process(double delta) {
		if (_prevDirection != GetMovementDirection()){
			_prevDirection = GetMovementDirection();
			EmitSignal(SignalName.MoveDirection, _prevDirection);
		}
    }
}
