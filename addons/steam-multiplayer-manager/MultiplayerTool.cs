#if TOOLS
using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[Tool]
public partial class MultiplayerTool : EditorPlugin {
	private Control _buttonsHbox => GetNode<Control>("../@Panel@13/@VBoxContainer@14/@EditorTitleBar@15/@EditorRunBar@4112/@PanelContainer@4046/@HBoxContainer@4047");
	private Button _buildButton => _buttonsHbox.GetChild<Button>(0);
	private Button _playButton => _buttonsHbox.GetChild<Button>(1);
	private Button _stopButton => _buttonsHbox.GetChild<Button>(4);
	private Button _multiplayerButton;
	private Callable buildCallable;

	public override void _EnterTree() {
		buildCallable = (Callable)_buildButton.GetSignalConnectionList("pressed")[0]["callable"];
		AddMultiplayerButton();
		UpdateButtons();
		
	}


    public override void _ExitTree() {
		_buildButton.Pressed -= CloseSandbox;
		_stopButton.Pressed -= CloseSandbox;
		_multiplayerButton.Free();
	}

	private void AddMultiplayerButton() {
		_multiplayerButton = new Button();
		_multiplayerButton.Pressed += OpenSandbox;
		_multiplayerButton.Text = "2";
		_multiplayerButton.Icon = _buttonsHbox.GetChild<Button>(1).Icon;

		_buttonsHbox.AddChild(_multiplayerButton);
		_buttonsHbox.MoveChild(_multiplayerButton, 2);
	}

	private void UpdateButtons() {
		// Disconnect the default build button and connect it to our custom method
		_buildButton.Disconnect("pressed", buildCallable);
		_buildButton.Pressed += CloseSandbox;
		_buildButton.Connect("pressed", buildCallable);

		_stopButton.Pressed += CloseSandbox;
	}

	private void OpenSandbox() {
		_playButton.EmitSignal("pressed");
		StartSandboxProgram("\"C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\Godot\\Godot Console.lnk\" --path \"C:\\Users\\gamer\\Documents\\Santas-workshop\"");

	}
	
	private void CloseSandbox() {
		StartSandboxProgram("/wait cmd /c & \"C:\\Program Files\\Sandboxie-Plus\\Start.exe\" /box:Dev_Box taskkill /f /im godot.exe");
	}

	private void StartSandboxProgram(string program) {
		Process.Start("C:\\Program Files\\Sandboxie-Plus\\Start.exe", program);
	}
}
#endif
