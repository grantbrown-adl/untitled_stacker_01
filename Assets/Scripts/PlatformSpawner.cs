using UnityEngine;

public class PlatformSpawner : MonoBehaviour {
    [SerializeField] Platform platformPrefab;
    [SerializeField] MoveDirection moveDirection;

    public void SpawnPlatform() {
        Platform platform = Instantiate(platformPrefab);
        float x = moveDirection == MoveDirection.X ? transform.position.x : Platform.PreviousPlatform.transform.position.x;
        float z = moveDirection == MoveDirection.Z ? transform.position.z : Platform.PreviousPlatform.transform.position.z;


        platform.transform.position = Platform.PreviousPlatform && Platform.PreviousPlatform != GameObject.Find("centre_cube").GetComponent<Platform>()
            ? new Vector3(x, Platform.PreviousPlatform.transform.position.y + platform.transform.localScale.y, z)
            : transform.position;

        platform.MoveDirection = moveDirection;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, platformPrefab.transform.localScale);
    }
}
