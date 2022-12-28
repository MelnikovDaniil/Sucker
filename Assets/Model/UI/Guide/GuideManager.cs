using UnityEngine;

public class GuideManager : MonoBehaviour
{
    public GameObject guideCanvas;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0))
        {
            guideCanvas.SetActive(false);
        }
    }
}
