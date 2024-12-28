using Godot;

public partial class MainMenuUI : Control {
	private string _coopMenuScene = "res://ui/main_menu/coop_menu.tscn";
	private string _gameScene = "res://scenes/demo_scene.tscn";

	private void OnNewPressed() {
		SceneManager.ChangeScene(_gameScene);
	}

	private	void OnLoadPressed() {
		// TODO: Load the load game scene
	}

	private void OnCoopPressed() {
		SceneManager.ChangeScene(_coopMenuScene);
	}

	private void OnSettingsPressed() {
		// TODO: Load the settings scene
	}

	private void OnExitPressed() {
		GetTree().Quit();
	}
}
