using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonobehavior<GameManager>
{
    public User User { get; set; }
    public PlayMode PlayMode { get; set; }
    public TeamColor MyTeamColor { get; set; }
    public Room MyRoom { get; set; }

    [SerializeField] private SceneTags startScene;

    [SerializeField] private FullScreenMode screenMode;

    private SceneTags _currentScene;

    protected override void Awake()
    {
        base.Awake();

        Screen.SetResolution(1920, 1080, screenMode);
    }

    private void Start()
    {
        SceneManager.LoadScene(startScene.ToString(), LoadSceneMode.Additive);
    }

    public bool hasLoggedIn()
    {
        return User != null;
    }

    public void StartGame(PlayMode mode, TeamColor myTeamColor)
    {
        MyTeamColor = myTeamColor;
        PlayMode = mode;

        switch (mode)
        {
            case PlayMode.PvP:
                LoadScene(SceneTags.Game);
                break;
            case PlayMode.PvE:
                LoadScene(SceneTags.Game);
                break;
        }
        
        UIManager.Instance.OnGameStarted(mode);
    }

    private void LoadScene(SceneTags scene)
    {
        SceneManager.UnloadSceneAsync(_currentScene.ToString()).completed += operation =>
        {
            SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
            _currentScene = scene;
        };
    }

    public void UpdateRoom(RoomInfo roomInfo)
    {
        if (MyRoom != null && MyRoom.Id == roomInfo.Id)
        {
            MyRoom.RoomType = roomInfo.RoomType;
            MyRoom.Players = roomInfo.Players;
        }
        else
        {
            MyRoom = new Room()
            {
                Id = roomInfo.Id,
                RoomType = roomInfo.RoomType,
                Players = roomInfo.Players
            };
        }

        if (roomInfo.RoomType == RoomType.PlayRoom)
            UIManager.Instance.SwitchTo(CanvasTags.PlayRoom);
    }

    public void JoinLobby()
    {
        LoadScene(SceneTags.Lobby);
        UIManager.Instance.SwitchTo(CanvasTags.Lobby);
    }
}