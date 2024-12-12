using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class NetworkSynchronizer : ToolHelper {
    private bool _collapsed = false;
    private readonly Array<Variant.Type> _syncableTypes = new(){
        Variant.Type.String,
        Variant.Type.Int,
        Variant.Type.Bool,
        Variant.Type.Vector2,
        Variant.Type.Vector3,
        Variant.Type.Vector4,
        Variant.Type.Vector2I,
        Variant.Type.Vector3I,
        Variant.Type.Vector4I,
        Variant.Type.Color,
        Variant.Type.Float
    };
    
    public override bool _InspectorProperty(ToolEditorInspector inspector, Variant.Type type, string name, PropertyHint hintType, string hintString, PropertyUsageFlags usageFlags, bool wide) {
        if (name != "SyncedProperties") return false;
        // Header
        Button header = new Button(){ 
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, 
            Text = "Synced Properties", 
            Icon = _collapsed ? EditorInterface.Singleton.GetEditorTheme().GetIcon("GuiTreeArrowRight", "EditorIcons") : EditorInterface.Singleton.GetEditorTheme().GetIcon("GuiTreeArrowDown", "EditorIcons"), 
            IconAlignment = HorizontalAlignment.Left,
            Alignment = HorizontalAlignment.Left
        };

        inspector.AddCustomControl(header);

        VBoxContainer propertyEditor = new VBoxContainer(){ Visible = !_collapsed };

        // SyncedProperties
        foreach (Dictionary syncProperty in SyncedProperties) {
            Control column = CreateSyncedPropertyColumn(syncProperty);
            propertyEditor.AddChild(column);
        }

        // Add Property Button
        Button addButton = new Button() { Text = "Add Property" };
        addButton.Pressed += AddProperty;
        propertyEditor.AddChild(addButton);

        header.Pressed += () => { 
            _collapsed = !_collapsed;
            NotifyPropertyListChanged();
        };
        inspector.AddCustomControl(propertyEditor);

        return true;
       
    }

    private void AddProperty() {
        SyncedProperties.Add(new Dictionary() {
            { "nodePath", "" },
            { "property", "" }
        });
        NotifyPropertyListChanged();
    }

    private Control CreateSyncedPropertyColumn(Dictionary syncProperty) {
        EditorInterface editorInterface = EditorInterface.Singleton;
        Theme editorTheme = editorInterface.GetEditorTheme();

        string nodePath = (string)syncProperty["nodePath"];
        string property = (string)syncProperty["property"];

        PanelContainer panelContainer = new PanelContainer();
        HBoxContainer row = new HBoxContainer();
        VBoxContainer column = new VBoxContainer(){ SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        Button removeButton = new Button() { Icon = editorTheme.GetIcon("Remove", "EditorIcons") };

        HBoxContainer nodePathRow = new HBoxContainer();
        HBoxContainer propertyRow = new HBoxContainer();

        Label nodePathLabel = new Label() { Text = "Node Path", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        Button nodePathButton = new Button() { Text = "Asign...", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };

        Label propertyLabel = new Label() { Text = "Property", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        Button propertyButton = new Button() { Text = "Asign...", SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, Disabled = true };


        if (nodePath != "") {
            Node currentNode = GetNodeOrNull(nodePath);
            if (currentNode == null) {
                SyncedProperties[SyncedProperties.IndexOf(syncProperty)]["nodePath"] = "";
                NotifyPropertyListChanged();
            }
            else {
                propertyButton.Disabled = false;
                nodePathButton.Icon = editorTheme.GetIcon(currentNode.GetClass(), "EditorIcons");
                nodePathButton.Text = currentNode.GetName();

                if (property != "") {
                    propertyButton.Text = property;
                    foreach (Dictionary propertyData in currentNode.GetPropertyList()){
                        if ((string)propertyData["name"] != property) continue;
                        Variant.Type type = (Variant.Type)(int)propertyData["type"];
                        Texture2D icon = GetPropertyIcon(type);
                        propertyButton.Icon = icon;
                    }
                }
            }
        }

        

        panelContainer.AddChild(row);
        row.AddChild(column);
        row.AddChild(removeButton);
        column.AddChild(nodePathRow);
        column.AddChild(propertyRow);

        nodePathRow.AddChild(nodePathLabel);
        nodePathRow.AddChild(nodePathButton);

        propertyRow.AddChild(propertyLabel);
        propertyRow.AddChild(propertyButton);

        

        removeButton.Pressed += () => {
            SyncedProperties.Remove(syncProperty);
            NotifyPropertyListChanged();
        };

        nodePathButton.Pressed += () => { CreatePathDialog(syncProperty); };
        propertyButton.Pressed += () => { CreatePropertyDialog(syncProperty); };

        return panelContainer;
    }

    private void CreatePathDialog(Dictionary syncProperty) {
        ConfirmationDialog dialog = new ConfirmationDialog() { Title = "Select a node to sync" };
        dialog.GetOkButton().Disabled = true;
        SceneTree sceneTree = new SceneTree();
        sceneTree.ItemSelected += () => { dialog.GetOkButton().Disabled = false; };
        EditorInterface.Singleton.GetBaseControl().AddChild(dialog);
        dialog.AddChild(sceneTree);
        sceneTree.CreateSceneTree(EditorInterface.Singleton.GetEditedSceneRoot());
        dialog.MinSize = new Vector2I(200, 500);
        dialog.PopupCentered();
        
        dialog.Visible = true;
        dialog.Confirmed += () => {
            Node selectedNode = sceneTree.GetSelectedNode();
            if (selectedNode != null) {
                SyncedProperties[SyncedProperties.IndexOf(syncProperty)]["nodePath"] = GetPathTo(selectedNode);
                SyncedProperties[SyncedProperties.IndexOf(syncProperty)]["property"] = "";
                NotifyPropertyListChanged();
            }
            dialog.QueueFree();
        };
        dialog.Canceled += () => { dialog.QueueFree(); };
    }

    private void CreatePropertyDialog(Dictionary syncProperty){
        ConfirmationDialog dialog = new ConfirmationDialog() { Title = "Select a property to sync" };
        dialog.GetOkButton().Disabled = true;
        EditorInterface.Singleton.GetBaseControl().AddChild(dialog);
        Tree propertyList = new Tree();
        TreeItem root = propertyList.CreateItem();
        root.SetText(0, "Properties");
        foreach (Dictionary propertyData in GetNodeOrNull((string)syncProperty["nodePath"]).GetPropertyList()){
            Variant.Type type = (Variant.Type)(int)propertyData["type"];
            if (!_syncableTypes.Contains(type)) continue;
            TreeItem item = propertyList.CreateItem(root);
            item.SetText(0, (string)propertyData["name"]);
            
            Texture2D icon = GetPropertyIcon(type);
            item.SetIcon(0, icon);
        }

        dialog.AddChild(propertyList);

        dialog.MinSize = new Vector2I(200, 500);
        dialog.PopupCentered();
        dialog.Visible = true;
        dialog.Confirmed += () => {
            dialog.QueueFree();
        };

        propertyList.ItemSelected += () => {  dialog.GetOkButton().Disabled = false; };
        dialog.Canceled += () => { dialog.QueueFree(); };
        dialog.Confirmed += () => {
            syncProperty["property"] = propertyList.GetSelected().GetText(0);
            NotifyPropertyListChanged();
        };
    }

    private Texture2D GetPropertyIcon(Variant.Type type) {
        Theme theme = EditorInterface.Singleton.GetEditorTheme();
        if (theme.HasIcon(type.ToString().ToLower(), "EditorIcons")) {
            return theme.GetIcon(type.ToString().ToLower(), "EditorIcons");
        }
        else if (theme.HasIcon(type.ToString(), "EditorIcons")) {
            return theme.GetIcon(type.ToString(), "EditorIcons");
        }
        return null;
    }
}