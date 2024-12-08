using Godot;

[GlobalClass]
public partial class SpawnPoint : Node3D {
	public static new Vector3 Position;

	public override void _Ready() {
		Position = GlobalPosition;
	}
}
