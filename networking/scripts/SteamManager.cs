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

	private Dictionary<CSteamID, Node3D> networkPlayers = new Dictionary<CSteamID, Node3D>();
	private CSteamID lobbyID;

	public override void _Ready() {
		Instance = this;
	}

    protected override void OnLobbyConnected(CSteamID lobbyID) {
		this.lobbyID = lobbyID;
		SceneManager.ChangeScene(gameScene);

		SceneManager.SpawnEntity(playerScene, SpawnPoint.Position);

		foreach (CSteamID member in GetLobbyMembers(lobbyID)) {
			if (member == clientID) { continue; }
			SceneManager.SpawnEntity(networkPlayerScene, SpawnPoint.Position);
		}
    }

    protected override void OnLobbyMemberConnected(CSteamID lobbyID, CSteamID memberID) {
		networkPlayers.Add(memberID, SceneManager.SpawnEntity(networkPlayerScene, SpawnPoint.Position));
	}

	protected override void OnLobbyMemberDisconnected(CSteamID lobbyID, CSteamID memberID) {
		if (networkPlayers.ContainsKey(memberID)) {
			networkPlayers[memberID].QueueFree();
			networkPlayers.Remove(memberID);
		}
	}

    protected override void OnLobbyUserMsg(CSteamID lobbyID, CSteamID userID, byte[] data) {
		if (PlayerInputData.TryParse(data, out PlayerInputData playerInput)){
			if (userID == clientID) { return; }
			if (!networkPlayers.ContainsKey(userID)) { return; }
			if (playerInput.inputType == PlayerInputData.InputType.Interact) {
				GD.Print("Interact pressed");
				networkPlayers[userID].EmitSignal(NetworkInput.SignalName.InteractPressed);
			}
			else if (playerInput.inputType == PlayerInputData.InputType.Action) {
				GD.Print("Action pressed");
				networkPlayers[userID].EmitSignal(NetworkInput.SignalName.ActionPressed);
			}
			else if (playerInput.inputType == PlayerInputData.InputType.Move) {
				GD.Print("Move direction: " + playerInput.direction);
				GD.Print(networkPlayers[userID]);
				Utils.TryGetComponent(networkPlayers[userID], out NetworkInput networkInput);
				networkInput.EmitSignal(NetworkInput.SignalName.MoveDirection, playerInput.direction);
			}
		}
    }

    public void CreateLobby() {
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
	}

	public static void SendPlayerData(PlayerInputData playerInput) {
		byte[] bytes = playerInput.ToData();
		Instance.BroadcastLobbyData(Instance.lobbyID, bytes);
	}

	public struct PlayerInputData {
		public enum InputType {
			Move,
			Interact,
			Action
		}

		public InputType inputType;
		public Vector2 direction;

		public byte[] ToData() {
        	return System.Text.Encoding.UTF8.GetBytes($"InputData,{(int)inputType},{direction.X},{direction.Y}");
    	}

		public static bool TryParse(byte[] data, out PlayerInputData playerInput) {
			try {
				playerInput = FromData(data);
				return true;
			}
			catch {
				playerInput = new PlayerInputData();
				return false;
			}
		}

		public static PlayerInputData FromData(byte[] data) {
			string dataString = System.Text.Encoding.UTF8.GetString(data);
			string[] dataParts = dataString.Split(',');
			if (dataParts[0] != "InputData") {
				throw new Exception("Invalid data type");
			}

			InputType inputType = Enum.Parse<InputType>(dataParts[1]);
			Vector2 direction = new Vector2(float.Parse(dataParts[2]), float.Parse(dataParts[3]));
			return new PlayerInputData(){
				inputType = inputType,
				direction = direction
			};
		}
	}

    
}
