using Godot;

[GlobalClass]
public partial class Inventory : Node {
	[Signal] public delegate void InventoryUpdatedEventHandler(Inventory sender);
	[Export] private int maxItems = 1;
	[Export] private ItemData[] items;

	public override void _Ready() {
		items = new ItemData[maxItems];
	}

	public bool AddItem(ItemData item) {
		for (int i = 0; i < items.Length; i++) {
			if (items[i] == null) {
				items[i] = item;
				EmitSignal(SignalName.InventoryUpdated, this);
				return true;
			}
		}
		return false;
	}

	public bool RemoveItem(ItemData item) {
		for (int i = 0; i < items.Length; i++) {
			if (items[i] == item) {
				items[i] = null;
				EmitSignal(SignalName.InventoryUpdated, this);
				return true;
			}
		}
		return false;
	}

	public bool HasItem(ItemData item) {
		for (int i = 0; i < items.Length; i++) {
			if (items[i] == item) {
				return true;
			}
		}
		return false;
	}

	public ItemData GetItem(int index) {
		if (index < 0 || index >= items.Length) {
			return null;
		}
		return items[index];
	}
}
