using Godot;
using System;

[GlobalClass]
public partial class RecipeData : Resource {
    [Export] public ItemData[] ingredients;
    [Export] public ItemData result;
    [Export] public float timeToCraft;
    
    public bool CanCraft(Inventory counterInventory, Inventory playerInventory){
        foreach (ItemData ingredient in ingredients){
            if (counterInventory.IsEmpty()) return false;
            if (!counterInventory.HasItem(ingredient) && !playerInventory.HasItem(ingredient)){
                return false;
            }
        }
        return true;
    }

    public override string ToString(){
        string ingredientsString = "";
        for (int i = 0; i < ingredients.Length; i++){
            ItemData ingredient = ingredients[i];
            ingredientsString += ingredient.itemName;
            if (i < ingredients.Length - 1){
                ingredientsString += " + ";
            }
        }

        return "Recipe: \n\t" + ingredientsString + " = " + result.itemName;
    }
}
