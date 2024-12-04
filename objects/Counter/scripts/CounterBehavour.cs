using Godot;

public partial class CounterBehavour : Node {
	private void OnInteracted() {
		GD.Print("Interacted with Counter");
	}

	private void OnSelected() {
		GD.Print("Selected Counter");
	}

	private void OnDeselected() {
		GD.Print("Deselected Counter");
	}
}
