using System.Collections.Generic;
using UnityEngine;

public class NoteUI : MonoBehaviour
{
    [Header("References")]
    public GameObject noteContent;

    [Header("Input")]
    public KeyCode closeKey = KeyCode.E;

    [Header("Minimap")]
    public GameObject[] minimapObjects;
    public bool hideMinimapWhileOpen = true;


    [Header("Audio")]
    public AudioClip closeSfx;

    private GameObject player;
    private PlayerInteract playerInteract;
    private FPSPlayerControllerIS playerController;
    private CharacterController characterController;
    private bool isOpen;

    private readonly List<bool> minimapPrevStates = new List<bool>(4);
    private bool minimapHiddenThisSession = false;  

    void Awake()
    {
        if (!noteContent) noteContent = gameObject;

        noteContent.SetActive(false);
    }

    void Update()
    {
        if (isOpen && Input.GetKeyDown(closeKey))
        {
            CloseNote();
        }
    }

    public void OpenNote(GameObject playerObject)
    {
        if (!noteContent) noteContent = gameObject;

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        if (isOpen) return;

        player = playerObject != null ? playerObject : GameObject.FindGameObjectWithTag("Player");

        isOpen = true;
        noteContent.SetActive(true);

        if (player)
        {
            playerInteract      = player.GetComponent<PlayerInteract>();
            playerController    = player.GetComponent<FPSPlayerControllerIS>();
            characterController = player.GetComponent<CharacterController>();

            if (playerInteract)      playerInteract.enabled = false;
            if (playerController)    playerController.enabled = false;
            if (characterController) characterController.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        minimapHiddenThisSession = false;
        ToggleMinimap(false);

        Time.timeScale = 0f;
    }

    public void CloseNote()
    {
        if (!isOpen) return;
        isOpen = false;

        noteContent.SetActive(false);

        if (playerInteract)        playerInteract.enabled = true;
        if (playerController)      playerController.enabled = true;
        if (characterController)   characterController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        ToggleMinimap(true);

        Time.timeScale = 1f;

        if (closeSfx && Camera.main)
            AudioSource.PlayClipAtPoint(closeSfx, Camera.main.transform.position);

        player = null;
        playerInteract = null;
        playerController = null;
        characterController = null;
        minimapPrevStates.Clear();
        minimapHiddenThisSession = false;
    }

    void OnDisable()
    {
        if (isOpen) CloseNote();
    }

    private void ToggleMinimap(bool restore)
    {
        if (!hideMinimapWhileOpen || minimapObjects == null) return;

        if (!restore)
        {
            if (minimapHiddenThisSession) return;

            minimapPrevStates.Clear();
            foreach (var go in minimapObjects)
            {
                if (!go) { minimapPrevStates.Add(false); continue; }
                minimapPrevStates.Add(go.activeSelf);
                go.SetActive(false);
            }
            minimapHiddenThisSession = true;
        }
        else
        {
            if (!minimapHiddenThisSession) return; 

            for (int i = 0; i < minimapObjects.Length; i++)
            {
                var go = minimapObjects[i];
                if (!go) continue;

                bool wantActive = (i < minimapPrevStates.Count) ? minimapPrevStates[i] : true;
                go.SetActive(wantActive);
            }
        }
    }
}
