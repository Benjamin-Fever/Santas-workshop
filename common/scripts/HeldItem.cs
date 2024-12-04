using Godot;

public partial class HeldItem : Node3D {
	private Node3D item;
	private void OnInventoryUpdated(Inventory sender) {
		ItemData itemData = sender.GetItem(0);
		if (itemData == null) {
			item?.QueueFree();
			item = null;
			return;
		}

		item = itemData.itemScene.Instantiate<Node3D>();
		AddChild(item);
	}
}
