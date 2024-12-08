using Godot;

[GlobalClass]
public partial class NetworkInput : Node {
	[Signal] public delegate void MoveDirectionEventHandler(Vector2 direction);
	[Signal] public delegate void InteractPressedEventHandler();
	[Signal] public delegate void ActionPressedEventHandler();
	
	public override void _Ready() {
		InteractPressed += () => GD.Print("Interact pressed from network input");
	}

	public override void _Process(double delta) {
		
	}
}
