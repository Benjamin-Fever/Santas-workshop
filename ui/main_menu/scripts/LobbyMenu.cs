using Godot;
using Steamworks;

public partial class LobbyMenu : PanelContainer {
	[Export] private OptionButton _lobbyMode;
	[Export] private SpinBox _maxPlayers;
	[Export] private Button _inviteButton;
	[Export] private Button _readyButton;
	[Export] private Control _userList;
	[Export] private PackedScene _userListItemScene;


	public override void _Ready() {
		Steam.SetLobbyMemberData("is_ready", bool.FalseString);
		UpdateUserList();
		Steam.Instance.LobbyMemberJoined += (ulong steamiD) => UpdateUserList();
		Steam.Instance.LobbyMemberLeft += (ulong steamiD) => UpdateUserList();
		Steam.Instance.LobbyDataUpdate += (ulong userId, ulong lobbyId, bool success) => UpdateUserList();

		_lobbyMode.ItemSelected += OnLobbyModeChanged;
		_maxPlayers.ValueChanged += OnMaxPlayersValueChanged;
		_inviteButton.Pressed += OnLobbyInviteButtonPressed;
		_readyButton.Pressed += OnLobbyReadyButtonPressed;
	}

	private void UpdateUserList() {
		foreach (Control child in _userList.GetChildren()) {
			child.QueueFree();
		}

		foreach (CSteamID memberId in Steam.GetLobbyMembers()) {
			Control item = _userListItemScene.Instantiate<Control>();
			bool isReady = bool.Parse(Steam.GetLobbyMemberData("is_ready", memberId));
			
			item.GetNode<Label>("Username").Text = SteamFriends.GetFriendPersonaName(memberId);
			item.GetNode<CheckBox>("MarginContainer/CheckBox").Modulate = isReady ? new Color("#40de4d") : new Color("#df4a40");
			_userList.AddChild(item);
		}
	}

	private void OnLobbyModeChanged(long mode) {
		Steam.SetLobbyType((ELobbyType)mode);
	}

	private void OnLobbyInviteButtonPressed() {
		Steam.InviteFriendsOverlay();
	}

	private void OnLobbyReadyButtonPressed() {
		GD.Print("Ready button pressed");
		Steam.SetLobbyMemberData("is_ready", bool.TrueString);
		UpdateUserList();
	}

	private void OnMaxPlayersValueChanged(double value) {
		Steam.SetLobbyMaxMembers((int)value);
	}

	private void OnBackPressed() {
		SceneManager.ChangeScene("res://ui/main_menu/main_menu.tscn");
		Steam.LeaveLobby();
	}
}
