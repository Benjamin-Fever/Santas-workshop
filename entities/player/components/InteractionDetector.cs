using Godot;

[GlobalClass]
public partial class InteractionDetector : ShapeCast3D {
	public Interactable SelectedInteractable {get; private set;}
	
	public override void _Process(double delta) {
		if (IsColliding()){
			Node collider = GetCollider(0) as Node;
			Interactable interactable = collider?.GetNodeOrNull<Interactable>("Interactable");
			if (interactable != null &&SelectedInteractable != interactable){
				SelectedInteractable?.Deselect();
				SelectedInteractable = interactable;
				SelectedInteractable.Select();
			}

		}
		else if (SelectedInteractable != null){
			SelectedInteractable?.Deselect();
			SelectedInteractable = null;
		}
	}
}
