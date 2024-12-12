using Godot;

[Tool]
public partial class ToolEditorPlugin : EditorPlugin {
	private ToolEditorInspector instance;

	public override void _EnterTree() {
		instance = new ToolEditorInspector();
		AddInspectorPlugin(instance);
	}

	public override void _ExitTree() {
		RemoveInspectorPlugin(instance);

	}
}