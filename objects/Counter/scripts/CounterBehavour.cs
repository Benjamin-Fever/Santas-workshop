using Godot;

public partial class CounterBehavour : Node {
	private void OnInteracted(Node sender) {
		if (!Utils.TryGetComponent(sender, out Inventory playerInventory)) return;
		if (!Utils.TryGetComponent(GetParent(), out Inventory counterInventory)) return;

		ItemData playerItem = playerInventory.GetItem(0);
		ItemData counterItem = counterInventory.GetItem(0);

		if (playerItem == null && counterItem == null) return;
		if (playerItem != null && counterItem != null) return;
		if (playerItem != null && counterItem == null) {
			counterInventory.AddItem(playerItem);
			playerInventory.RemoveItem(playerItem);
		}

		if (playerItem == null && counterItem != null) {
			playerInventory.AddItem(counterItem);
			counterInventory.RemoveItem(counterItem);
		}
	}

	private void OnSelected() {
		GD.Print("Selected Counter");
	}

	private void OnDeselected() {
		GD.Print("Deselected Counter");
	}
}
