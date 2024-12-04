using Godot;
using System;

public partial class PlayerController : Node {
	[Export] private CharacterBody3D characterBody;
	[Export] private GameInput gameInput;
	[Export] private float speed = 5.0f;
	public override void _Ready() {
		gameInput.OnInteract += OnInteract;
	}

	public override void _Process(double delta) {
		
	}

	public override void _PhysicsProcess(double delta) {
		HandleMovement(delta);
	}

	private void HandleMovement(double delta) {
		Vector2 inputVector = gameInput.GetMovementDirection();
		Vector3 moveDir = new Vector3(inputVector.X, 0, inputVector.Y);
		Vector3 velocity = moveDir * speed * (float)delta;
		characterBody.Velocity = velocity;
		characterBody.MoveAndSlide();

		if (moveDir == Vector3.Zero) return;
		float rotationSpeed = 10.0f;
		Transform3D transform = characterBody.Transform;
		transform.Basis = transform.Basis.Slerp(Basis.LookingAt(moveDir), rotationSpeed * (float)delta);
		characterBody.Transform = transform;
	}

	private void OnInteract() {
		GD.Print("Interacted");
	}
}
