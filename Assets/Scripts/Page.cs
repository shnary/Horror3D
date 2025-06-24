using UnityEngine;

public class Page : MonoBehaviour {

    public void Collect() {
        GameManager.Instance.CollectPage();
        gameObject.SetActive(false);
    }
    
}