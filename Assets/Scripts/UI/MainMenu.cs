using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AudioSource source;
    public AudioClip song;

    public Texture2D cursorTexture;

    public Texture2D crosshairTexture;

    public void Start()
    {
        source.clip = song;
        source.loop = true;
        source.Play();

        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void LoadMain()
    {
        source.Stop();

        Cursor.SetCursor(crosshairTexture, Vector2.zero, CursorMode.Auto);

        SceneManager.LoadScene("Procedural-Generation-Test2");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
