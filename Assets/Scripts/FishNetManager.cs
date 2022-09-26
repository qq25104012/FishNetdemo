using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Logging;
using FishNet.Managing.Scened;
using UnityEngine.SceneManagement;

public class FishNetManager : NetworkBehaviour
{
    [SerializeField] private string sceneName = "Level";

    [SerializeField] private GameObject persistentLevelSettingsPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (IsServer)
        {
            GameObject settings = Instantiate(persistentLevelSettingsPrefab);

            Spawn(settings);
        }
    }

    public void LoadScene()
    {
        SceneLoadData sld = new SceneLoadData(sceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
