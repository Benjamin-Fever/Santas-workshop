using Godot;
using System;

[GlobalClass]
public partial class GameInput : Node {
	[Signal] public delegate void InteractPressedEventHandler();

	public Vector2 GetMovementDirection(){
		Vector2 direction = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		return direction.Normalized();
	}

    public override void _UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("interact")){
			EmitSignal(SignalName.InteractPressed);
		}
    }
}
