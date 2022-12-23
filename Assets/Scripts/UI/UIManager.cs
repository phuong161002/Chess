using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : SingletonMonobehavior<UIManager>
{
    [SerializeField] private Canvas DialougeCanvas;
    [SerializeField] private LoginUIManager loginUIManager;
    [SerializeField] private RegisterUIManager registerUIManager;
    [SerializeField] private LobbyUIManager lobbyUIManager;
    [SerializeField] private PlayRoomUIManager playRoomUIManager;
    [SerializeField] private GameOverUIManager gameOverUIManager;
    [SerializeField] private Dialogue dialoguePrefab;
    [SerializeField] private CanvasTags startCanvas;


    private Dialogue _dialogue;
    private CanvasTags _activeCanvas;

    private void Start()
    {
        _dialogue = Instantiate(dialoguePrefab, DialougeCanvas.transform);
        _dialogue.gameObject.SetActive(false);
        SwitchTo(startCanvas);
    }

    public void SwitchTo(CanvasTags canvasTags)
    {
        HideUI();
        switch (canvasTags)
        {
            case CanvasTags.Login:
                loginUIManager.gameObject.SetActive(true);
                _activeCanvas = CanvasTags.Login;
                break;
            case CanvasTags.Register:
                registerUIManager.gameObject.SetActive(true);
                _activeCanvas = CanvasTags.Register;
                break;
            case CanvasTags.Lobby:
                lobbyUIManager.gameObject.SetActive(true);
                _activeCanvas = CanvasTags.Lobby;
                break;
            case CanvasTags.PlayRoom:
                playRoomUIManager.gameObject.SetActive(true);
                _activeCanvas = CanvasTags.PlayRoom;
                break;
            case CanvasTags.GameOver:
                gameOverUIManager.gameObject.SetActive(true);
                _activeCanvas = CanvasTags.GameOver;
                break;
        }
    }

    public void SwitchToLoginCanvas()
    {
        if (_activeCanvas == CanvasTags.Login)
        {
            return;
        }

        SwitchTo(CanvasTags.Login);
    }

    public void SwitchToRegisterCanvas()
    {
        if (_activeCanvas == CanvasTags.Register)
        {
            return;
        }

        SwitchTo(CanvasTags.Register);
    }
    
    public void HideUI()
    {
        loginUIManager.gameObject.SetActive(false);
        registerUIManager.gameObject.SetActive(false);
        lobbyUIManager.gameObject.SetActive(false);
        gameOverUIManager.gameObject.SetActive(false);
        playRoomUIManager.gameObject.SetActive(false);
        _activeCanvas = CanvasTags.None;
    }

    public void ShowDialogue(string message, DialogueType dialogueType, float lifeTime = 0f)
    {
        _dialogue.Setup(message, dialogueType);
        _dialogue.ShowDialogue();
        if (lifeTime <= 0f)
        {
        }
        else
        {
            Invoke(nameof(HideDialogue), lifeTime);
        }
    }

    public void HideDialogue()
    {
        _dialogue.HideDialogue();
    }

    public void OnGameFinished(string winner)
    {
        gameOverUIManager.OnGameFinished(winner);
    }

    public void ShowRoomInfo(RoomList roomList)
    {
        if (_activeCanvas == CanvasTags.Lobby)
        {
            lobbyUIManager.ShowRoomInfo(roomList);
        }
    }

    public void OnGameStarted(PlayMode playMode)
    {
        playRoomUIManager.isGameStarted = true;
        SwitchTo(CanvasTags.PlayRoom);
    }

    public void OnPlayerExitPlayRoom()
    {
        playRoomUIManager.isGameStarted = false;
    }

}