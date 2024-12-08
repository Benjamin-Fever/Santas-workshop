using System;
using Godot;

[GlobalClass]
public partial class SceneManager : Node {
	[Signal] public delegate void SceneLoadedEventHandler();

	public static SceneManager Instance;
	private Node3D entityRoot;
	private Node currentScene;
	
	public override void _Ready() {
		Instance = this;
		
		currentScene = GetChildOrNull<Node>(0);
		entityRoot = new Node3D();
		AddChild(entityRoot);
	}

	public static T SpawnEntity<T>(PackedScene entity, Vector3? position = null, Vector3? rotation = null) where T : Node3D {
		T entity_instance = entity.Instantiate<T>();
		Instance.entityRoot.AddChild(entity_instance);
		entity_instance.GlobalPosition = position ?? Vector3.Zero;
		entity_instance.GlobalRotationDegrees = rotation ?? Vector3.Zero;
		GD.Print("Spawned entity: " + entity_instance.Name);
		return entity_instance;
	}

	public static Node3D SpawnEntity(PackedScene entity, Vector3? position = null, Vector3? rotation = null) {
		return SpawnEntity<Node3D>(entity, position, rotation);
	}

	public static void ChangeScene(PackedScene scene) {
		Instance.currentScene?.Free();
		Instance.currentScene = scene.Instantiate<Node3D>();
		Instance.currentScene.Ready += Instance.OnSceneLoaded;
		Instance.AddChild(Instance.currentScene);
		
	}

	private void OnSceneLoaded() {
		EmitSignal(SignalName.SceneLoaded);
	}
		
}
