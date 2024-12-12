using Godot;
using System;

public interface IToolHelper {
    public void _ToolReady();
	public void _GameReady();
	public void _ToolProcess(double delta);
	public void _GameProcess(double delta);
	public void _ToolPhysicsProcess(double delta);
	public void _GamePhysicsProcess(double delta);

    public void _InspectorBegin(ToolEditorInspector inspector);
    public void _InsectorEnd(ToolEditorInspector inspector);
    public void _InspectorCategory(ToolEditorInspector inspector, string category);
    public void _InspectorGroup(ToolEditorInspector inspector, string group);
    public bool _InspectorProperty(ToolEditorInspector inspector, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide);
}