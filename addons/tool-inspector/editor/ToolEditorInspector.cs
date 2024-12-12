#if TOOLS
using Godot;

[Tool]
public partial class ToolEditorInspector : EditorInspectorPlugin {

	public override bool _CanHandle(GodotObject godotObject) {
		if (godotObject is IToolHelper) {
            return true;
        }

		return false;
	}

	public override void _ParseBegin(GodotObject godotObject) {
        IToolHelper toolHelper = godotObject as IToolHelper;
		toolHelper._InspectorBegin(this);
	}

	public override void _ParseEnd(GodotObject godotObject) {
        IToolHelper toolHelper = godotObject as IToolHelper;
		toolHelper._InsectorEnd(this);
	}

	public override void _ParseCategory(GodotObject godotObject, string category) {
		IToolHelper toolHelper = godotObject as IToolHelper;
        toolHelper._InspectorCategory(this, category);
	}

	public override void _ParseGroup(GodotObject godotObject, string group) {
		IToolHelper toolHelper = godotObject as IToolHelper;
        toolHelper._InspectorGroup(this, group);
	}

    public override bool _ParseProperty(GodotObject godotObject, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide) {
		IToolHelper toolHelper = godotObject as IToolHelper;
        return toolHelper._InspectorProperty(this, type, name, hintType, hintString, usageFlags, wide);
    }
}
#endif