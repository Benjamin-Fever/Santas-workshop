using Godot;

public partial class InteractionDetector : ShapeCast3D {
	private Interactable _selectedInteractable;
	public override void _Ready() {
		
	}

	public override void _Process(double delta) {
		if (IsColliding()){
			Node collider = GetCollider(0) as Node;
			Interactable interactable = collider?.GetNodeOrNull<Interactable>("Interactable");
			if (interactable != null &&_selectedInteractable != interactable){
				_selectedInteractable?.Deselect();
				_selectedInteractable = interactable;
				_selectedInteractable.Select();
			}

		}
		else if (_selectedInteractable != null){
			_selectedInteractable?.Deselect();
			_selectedInteractable = null;
		}
	}
}
