using UnityEngine;

public class UIViewManager : MonoBehaviour
{
    public GameObject uiGameObject;

    void Start()
    {
        uiGameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            uiGameObject.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt)) {
            uiGameObject.SetActive(false);
        }
    }
}
