using Godot;
using System;
using System.Collections.Generic;

public partial class SceneTree : Tree {
    private Dictionary<ulong, Node> _nodeMap = new Dictionary<ulong, Node>();
    public void CreateSceneTree(Node sceneRoot) {
        Clear();
        TreeItem root = CreateNodeItem(sceneRoot);
        CreateSceneTree(root, sceneRoot);
    }

    private void CreateSceneTree(TreeItem treeParent, Node nodeParent) {
        foreach (Node child in nodeParent.GetChildren()) {
            TreeItem item = CreateNodeItem(child, treeParent);
            CreateSceneTree(item, child);
        }   
    }

    private TreeItem CreateNodeItem(Node node, TreeItem parent = null) {
        TreeItem item = CreateItem(parent);
        item.SetIcon(0, EditorInterface.Singleton.GetEditorTheme().GetIcon(node.GetClass(), "EditorIcons"));
        item.SetText(0, node.GetName());
        _nodeMap[item.GetInstanceId()] = node;
        return item;
    }

    public Node GetSelectedNode() {
        TreeItem selectedItem = GetSelected();
        if (selectedItem == null) {
            return null;
        }
        return _nodeMap[selectedItem.GetInstanceId()];
    }
}