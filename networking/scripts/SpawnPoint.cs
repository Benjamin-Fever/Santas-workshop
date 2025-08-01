using Godot;

[GlobalClass]
public partial class SpawnPoint : Node3D {
	public static new Vector3 Position {get; private set;} = Vector3.Zero;
	public static new Vector3 Rotation {get; private set;} = Vector3.Zero;

	public override void _Ready() {
		Position = GlobalPosition;
		Rotation = GlobalRotationDegrees;
	}
}
