using System;
using System.Collections.Generic;
using Godot;
using Steamworks;

public partial class CoopMenu : Control {
	[Export] private Control _friendsList;
	[Export] private Control _invitePopup;
	[Export] private LineEdit _inviteCodeInput;
	[Export] private PackedScene _friendListItemScene;

	private string _mainMenuScene = "res://ui/main_menu/main_menu.tscn";
	private string _lobbyMenuScene = "res://ui/main_menu/lobby_menu.tscn";

	private Dictionary<CSteamID, Control> _friendListItems = new Dictionary<CSteamID, Control>();

	public override void _Ready() {
		OnRefreshTimerTimeout();
		Steam.Instance.LobbyEntered += ChangeToLobbyScene;
	}

	private void ChangeToLobbyScene() {
		SceneManager.ChangeScene(_lobbyMenuScene);
	}


	private void OnBackButtonPressed() {
		if (_invitePopup.Visible) {
			_invitePopup.Visible = false;
			return;
		}

		SceneManager.ChangeScene(_mainMenuScene);
	}

	private void OnInviteButtonPressed() {
		_invitePopup.Visible = true;
	}

	private void OnInviteJoinButtonPressed() {

	}

	private void OnHostNewGameButtonPressed() {
		Steam.CreateLobby();
	}

	private void OnRefreshTimerTimeout() {

		if (Steam.Instance == null) return;
		

		HashSet<CSteamID> lobbySet = Steam.GetFriendsLobbys();
		foreach (Control child in _friendsList.GetChildren()) {
			foreach (KeyValuePair<CSteamID, Control> keyset in _friendListItems){
				if (child == keyset.Value && !lobbySet.Contains(keyset.Key)) {
					_friendListItems.Remove(keyset.Key);
					child.QueueFree();
					break;
				}
			}
		}
		foreach (CSteamID lobby in lobbySet) {
			Control item = _friendListItemScene.Instantiate<Control>();
			string hostId = Steam.GetLobbyData("game_id", lobby);
			if (hostId != Steam._appId.ToString()) continue;
			if (_friendListItems.ContainsKey(lobby)) continue;
			CSteamID host = new CSteamID(ulong.Parse(Steam.GetLobbyData("host_id", lobby)));
			item.GetNode<Label>("HostName").Text = SteamFriends.GetFriendPersonaName(host);
			item.GetNode<Label>("PlayerCount").Text =  $"{Steam.GetLobbyMemberCount(lobby)} / {Steam.GetLobbyMaxMembers()} players";
			item.GetNode<Button>("JoinButton").Pressed += () => {
				Steam.JoinLobby(lobby);
			};
			_friendListItems.Add(lobby, item);
			_friendsList.AddChild(item);
		}
	}



}
