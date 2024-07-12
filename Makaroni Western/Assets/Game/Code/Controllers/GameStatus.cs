using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {Menu, Concentration, Duel, Final}
public enum PlayerType {Computer, Person}

public enum GameType {Local, Network}

public static class GameStatus
{
    public static GameState CurrentState = GameState.Duel;
    public static GameType GameType;

    public static void ChangeState(GameState state)
    {
        if(state == GameState.Duel || state == GameState.Concentration)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if(state == GameState.Menu || state == GameState.Final)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }


        CurrentState = state;
    }
}
