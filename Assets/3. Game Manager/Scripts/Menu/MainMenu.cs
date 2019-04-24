using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animation mainMenuAnimator;
    [SerializeField] private AnimationClip fadeOutAnimation;
    [SerializeField] private AnimationClip fadeInAnimation;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    public void OnFadeOutComplete()
    {
        Debug.Log("FadeOut Complete");
    }

    public void OnFadeInComplete()
    {
        Debug.Log("FadeIn Complete");

        UIManager.Instance.SetDummyCameraActive(true);
    }

    void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (previousState == GameManager.GameState.PREGAME && currentState == GameManager.GameState.RUNNING)
        {
            FadeOut();
        }
    }

    public void FadeIn()
    {
        mainMenuAnimator.Stop();
        mainMenuAnimator.clip = fadeInAnimation;
        mainMenuAnimator.Play();
    }

    public void FadeOut()
    {
        UIManager.Instance.SetDummyCameraActive(false);

        mainMenuAnimator.Stop();
        mainMenuAnimator.clip = fadeOutAnimation;
        mainMenuAnimator.Play();
    }
}
