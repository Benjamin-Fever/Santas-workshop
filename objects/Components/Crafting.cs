using Godot;

public partial class Crafting : Node {
	[Export] private RecipeData[] recipes;
	[Export] private Inventory counterInventory;
	
	public void Craft(Node sender){
		if (counterInventory.IsEmpty()) return;
		Utils.TryGetComponent(sender, out Inventory playerInventory);
		RecipeData recipe = GetCraftableRecipe(playerInventory);
		if (recipe == null) return;
		foreach (ItemData ingredient in recipe.ingredients){
			counterInventory.RemoveItem(ingredient);
			if (recipe.ingredients.Length > 1){
				playerInventory.RemoveItem(ingredient);
			}
		}
		counterInventory.AddItem(recipe.result);
	}

	public RecipeData GetCraftableRecipe(Inventory playerInventory){
		foreach (RecipeData recipe in recipes){
			if (recipe.CanCraft(counterInventory, playerInventory)){
				return recipe;
			}
		}
		return null;
	}
}
