using UnityEngine;

public class Diamond : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ydhPlayerCharacter player = other.GetComponent<ydhPlayerCharacter>();
        if (player != null)
        {
            DiamondManager diamondManager = FindFirstObjectByType<DiamondManager>();
            if (diamondManager != null)
            {
                diamondManager.CollectDiamond();
            }
            else
            {
                Debug.LogError("未找到 DiamondManager!");
            }
            Destroy(gameObject);
        }
    }
}