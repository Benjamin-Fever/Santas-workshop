using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource {
    [Export] public string itemName;
    [Export] public Texture icon;
    [Export] public PackedScene itemScene;
}
