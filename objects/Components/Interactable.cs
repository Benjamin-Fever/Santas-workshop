using Godot;
using System;

[GlobalClass]
public partial class Interactable : Node {
	[Signal] public delegate void OnSelectedEventHandler();
	[Signal] public delegate void OnDeselectedEventHandler();
	[Signal] public delegate void OnInteractedEventHandler();

	public void Select(){
		EmitSignal(SignalName.OnSelected);
	}

	public void Deselect(){
		EmitSignal(SignalName.OnDeselected);
	}

	public void Interact(){
		EmitSignal(SignalName.OnInteracted);
	}
}
