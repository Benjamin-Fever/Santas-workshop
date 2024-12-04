using Godot;

public partial class StorageBehaviour : Node {
	[Export] private ItemData storageItem;
	
	private void OnInteracted(Node sender) {
		if (!Utils.TryGetComponent(sender, out Inventory playerInventory)) return;
		if (playerInventory.HasItem(storageItem)){
			playerInventory.RemoveItem(storageItem);
			return;
		}
		if (playerInventory.GetItem(0) == null){
			playerInventory.AddItem(storageItem);
			return;
		}
	}
}
