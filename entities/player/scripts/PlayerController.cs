using Godot;
using System;

public partial class PlayerController : Node {
	[Export] private CharacterBody3D characterBody;
	[Export] private GameInput gameInput;
	[Export] private InteractionDetector interactionDetector;
	[Export] private float speed = 5.0f;

	private void OnInteract() {
		interactionDetector.SelectedInteractable?.Interact(GetParent());
	}

	private void OnAction() {
		interactionDetector.SelectedInteractable?.PerformAction(GetParent());
	}

	public void OnMoveDirection(Vector2 direction){
		Vector3 moveDir = new Vector3(direction.X, 0, direction.Y);
		Vector3 velocity = moveDir * speed * (float)GetProcessDeltaTime();
		characterBody.Velocity = velocity;
	}

	public override void _PhysicsProcess(double delta) {
		characterBody.MoveAndSlide();

		if (characterBody.Velocity.Normalized() == Vector3.Zero) return;
		float rotationSpeed = 10.0f;
		Transform3D transform = characterBody.Transform;
		transform.Basis = transform.Basis.Slerp(Basis.LookingAt(characterBody.Velocity.Normalized()), rotationSpeed * (float)GetProcessDeltaTime());
		characterBody.Transform = transform;
	}

	private void HandleMovement(double delta) {
		
	}

	
}
