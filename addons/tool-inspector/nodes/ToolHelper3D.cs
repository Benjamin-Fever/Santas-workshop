using System;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class ToolHelper3D : Node3D, IToolHelper {

	[Export] public bool GameVisible = true;

	public sealed  override void _Ready() {
		if (Engine.IsEditorHint()) {
			Visible = true;
			_ToolReady();
		} else {
			Visible = GameVisible;
			_GameReady();
		}
	}



	public sealed override void _Process(double delta) {
		if (Engine.IsEditorHint()) {
			_ToolProcess(delta);
		} else {
			_GameProcess(delta);
		}
	}

	public sealed override void _PhysicsProcess(double delta) {
		if (Engine.IsEditorHint()) {
			_ToolPhysicsProcess(delta);
		} else {
			_GamePhysicsProcess(delta);
		}
	}


    public virtual void _ToolReady() {}

    public virtual void _GameReady() {}

    public virtual void _ToolProcess(double delta) {}

    public virtual void _GameProcess(double delta) {}

    public virtual void _ToolPhysicsProcess(double delta) {}

    public virtual void _GamePhysicsProcess(double delta) {}

    public virtual void _InspectorBegin(ToolEditorInspector inspector) {}

    public virtual void _InsectorEnd(ToolEditorInspector inspector) {}

    public virtual void _InspectorCategory(ToolEditorInspector inspector, string category) {}

    public virtual void _InspectorGroup(ToolEditorInspector inspector, string group) {}
	public bool _InspectorProperty(ToolEditorInspector inspector, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide){
		return false;
	}
}
