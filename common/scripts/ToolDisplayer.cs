using Godot;

[GlobalClass]
public partial class ToolDisplayer : Node3D {
	public override void _Ready() {
		if (!Engine.IsEditorHint()) {
			Visible = false;
		}
	}
}
