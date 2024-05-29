using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {
    [SerializeField] float time;

    private void Awake() {
        Destroy(gameObject, time);
    }
}
