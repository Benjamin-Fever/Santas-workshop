using System;
using System.IO;
using System.Runtime.InteropServices;
using Godot;
using Steamworks;
[GlobalClass]
public partial class SteamManager : Node {
	public static SteamManager Instance { get; private set; }
	private static uint _appId = 480;

	public SteamManager() {
		if (Instance != null) return;
		NativeLibrary.Load(Path.Join(AppContext.BaseDirectory, "steam_api64.dll"));
		GD.Print(SteamAPI.IsSteamRunning());
		Instance = this;
		try {
			if (SteamAPI.Init()) {
				GD.Print("Steamworks initialized.");
			} else {
				GD.Print("Steamworks failed to initialize.");
			}
		}
		catch (Exception e) {
			GD.PrintErr(e);
		}
	}

	public override void _Ready() {

	}

	public override void _Process(double delta) {
		
	}

	public override void _ExitTree() {
		try {
			SteamAPI.Shutdown();
		}
		catch (Exception e) {
			GD.PrintErr(e);
		}
	}
}
