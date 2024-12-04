using Godot;
using System;

public static class Utils {
    public static bool TryGetComponent<T>(Node caller, out T component) where T : Node{
        foreach (Node child in caller.GetChildren()) {
            if (child is T) {
                component = (T)child;
                return true;
            }
        }
        component = null;
        return false;
    }

}