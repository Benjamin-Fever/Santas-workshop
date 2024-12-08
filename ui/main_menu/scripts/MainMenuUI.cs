using Godot;

public partial class MainMenuUI : Control {
	private void OnStartGamePressed() {
		SteamManager.Instance.CreateLobby();
	}
}
