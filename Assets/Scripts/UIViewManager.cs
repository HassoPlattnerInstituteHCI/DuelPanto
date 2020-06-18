using UnityEngine;

public class UIViewManager : MonoBehaviour
{
    public GameObject uiGameObject;
    public GameObject screenBlack;

    void Start()
    {
        uiGameObject.SetActive(false);
        screenBlack.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            screenBlack.SetActive(false);
            uiGameObject.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt)) {
            screenBlack.SetActive(true);
            uiGameObject.SetActive(false);
        }
    }
}
