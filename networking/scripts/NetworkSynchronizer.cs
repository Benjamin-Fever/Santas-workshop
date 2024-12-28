using Godot;
using Godot.Collections;
using System;


public partial class NetworkSynchronizer : ToolHelper {

    [Export] public bool Enabled { get; set; } = true;
    [Export] public Node RootNode { get; set; }
    [Export] public Array<Dictionary> SyncedProperties { get; set; } = new Array<Dictionary>(){};
    public ulong NetworkId = 0;

    public override void _GameReady() {
        if (!Enabled) return;

    }

    public override void _GameProcess(double delta) {
        if (!Enabled) return;
        if (NetworkId == 0) 
        foreach (Dictionary property in SyncedProperties) {
            if (!property.ContainsKey("nodePath") || !property.ContainsKey("property")) continue;
            Node node = GetNodeOrNull<Node>((string)property["nodePath"]);
            if (node == null) continue;
            string propertyName = (string)property["property"];
            Variant value = node.Get(propertyName);
            if (value.VariantType == Variant.Type.Nil) continue;
            GD.Print(value);
        }
        //SteamManager.SendSyncData(SyncedProperties);
    }
}