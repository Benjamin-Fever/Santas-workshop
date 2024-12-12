using System;
using System.Collections.Generic;
using Godot;
using Steamworks;

[GlobalClass]
public partial class SteamManager : SteamNetwork {
	[Export] public PackedScene playerScene;
	[Export] public PackedScene networkPlayerScene;
	[Export] public PackedScene gameScene;

	public static SteamManager Instance;

	public override void _Ready() {
		Instance = this;
	}

    protected override void OnLobbyConnected(CSteamID lobbyID) {
		this.lobbyID = lobbyID;
		SceneManager.ChangeScene(gameScene);
		networkPlayers.Clear();
		networkPlayers.Add(clientID, SceneManager.SpawnEntity(playerScene, SpawnPoint.Position, SpawnPoint.Rotation));
		
    }

    protected override void OnLobbyMemberConnected(CSteamID lobbyID, CSteamID memberID) {
		Vector3 spawnPoint = SpawnPoint.Position;
		Node3D member = SceneManager.SpawnEntity(networkPlayerScene, spawnPoint, SpawnPoint.Rotation);
		networkPlayers.Add(memberID, member);		

		//BoradcastPlayerData(lobbyID);
	}

	private void BoradcastPlayerData(CSteamID lobbyID) {
		Node3D player = networkPlayers[clientID];
		PlayerDataPacket playerDataPacket = new PlayerDataPacket(player.GlobalPosition, player.GlobalRotation);
		BroadcastLobbyData(lobbyID, playerDataPacket);
	}

	protected override void OnLobbyMemberDisconnected(CSteamID lobbyID, CSteamID memberID) {
		if (networkPlayers.ContainsKey(memberID)) {
			networkPlayers[memberID].QueueFree();
			networkPlayers.Remove(memberID);
		}
	}

    protected override void OnLobbyPlayerInput(CSteamID lobbyID, CSteamID userID, PlayerInputPacket playerInputPacket) {
		if (userID == clientID) { return; }
		Utils.TryGetComponent(networkPlayers[userID], out NetworkInput networkInput);
		switch (playerInputPacket.GetInputType()) {
			case PlayerInputPacket.InputType.Interact:
				networkInput.EmitSignal(NetworkInput.SignalName.InteractPressed);
				break;
			case PlayerInputPacket.InputType.Action:
				networkInput.EmitSignal(NetworkInput.SignalName.ActionPressed);
				break;
			case PlayerInputPacket.InputType.Move:
				networkInput.EmitSignal(NetworkInput.SignalName.MoveDirection, playerInputPacket.GetDirection());
				break;
		}
    }

    protected override void OnLobbyPlayerData(CSteamID lobbyID, CSteamID userID, PlayerDataPacket playerDataPacket) {
		if (userID == clientID) { return; }
		Node3D networkPlayer = networkPlayers[userID];
		networkPlayer.GlobalPosition = playerDataPacket.GetPosition();
		networkPlayer.GlobalRotation = playerDataPacket.GetRotation();
    }

    public void CreateLobby() {
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
	}

	public static void SendPlayerData(PlayerInputPacket playerInput) {
		Instance.BroadcastLobbyData(Instance.lobbyID, playerInput);
	}

	public static void SendSyncData(Variant data){
		
	}
}
