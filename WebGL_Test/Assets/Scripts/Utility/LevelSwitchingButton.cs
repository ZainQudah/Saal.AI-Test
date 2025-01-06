using UnityEngine;

public class LevelSwitchingButton : MonoBehaviour
{
    public void SwitchLevel(string scene)
    {
        SceneLoader.Instance.LoadScene(scene);
    }
    
}
