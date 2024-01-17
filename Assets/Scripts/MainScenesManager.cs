using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScenesManager : MonoBehaviour
{
    [SerializeField] Button btnStart;
    [SerializeField] Button btnExit;

    private void Awake()
    {
        btnStart.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(1);
        });
        btnExit.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
