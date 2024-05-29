using UnityEngine;
using UnityEngine.SceneManagement;

public class Platform : MonoBehaviour {
    [SerializeField] float speed;
    [SerializeField] float platformDifference;
    [SerializeField] bool isStopped;
    [SerializeField] bool initialCube = false;
    [SerializeField] bool spawnNew;
    [SerializeField] GameObject fallingPiece;

    static Platform previousPlatform;
    static Platform currentPlatform;
    public static Platform CurrentPlatform { get => currentPlatform; private set => currentPlatform = value; }
    public static Platform PreviousPlatform { get => previousPlatform; private set => previousPlatform = value; }
    public MoveDirection MoveDirection { get; set; }
    public bool SpawnNew { get => spawnNew; set => spawnNew = value; }

    private void Awake() {
        if (previousPlatform == null) { previousPlatform = this; }
        if (currentPlatform != null) { previousPlatform = currentPlatform; }
        currentPlatform = this;
        if (speed <= 0 && !initialCube) { speed = 1.0f; }

        spawnNew = true;

        GetComponent<Renderer>().material.color = GetRandomColour();

        transform.localScale = new Vector3(previousPlatform.transform.localScale.x, transform.localScale.y, previousPlatform.transform.localScale.z);
    }

    private Color GetRandomColour() {
        return new(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));
    }

    private void Update() {
        if (isStopped) { return; }
        transform.position += speed * Time.deltaTime * (MoveDirection == MoveDirection.X ? transform.right : transform.forward);

    }

    public void StopPlatform() {
        if (initialCube) return;
        isStopped = true;
        // Slice that mud pie
        float differenceZ = transform.position.z - previousPlatform.transform.position.z;
        float differenceX = transform.position.x - previousPlatform.transform.position.x;
        float localZ = previousPlatform.transform.localScale.z;
        float localX = previousPlatform.transform.localScale.x;
        platformDifference = MoveDirection == MoveDirection.Z ? differenceZ : differenceX;

        if (Mathf.Abs(platformDifference) >= (MoveDirection == MoveDirection.Z ? localZ : localX)) {
            previousPlatform = null;
            currentPlatform = null;
            SceneManager.LoadScene(0);
            spawnNew = false;
            return;
        }

        float direction = platformDifference > 0 ? 1.0f : -1.0f;
        if (MoveDirection == MoveDirection.Z) SlicePlatformZ(platformDifference, direction);
        else SlicePlatformX(platformDifference, direction);
    }

    void UpdateScore(float score) {
        GameManager.Instance.Score = score;
    }

    private void SlicePlatformX(float platformDifference, float direction) {
        float newSizeOnX = previousPlatform.transform.localScale.x - Mathf.Abs(platformDifference);
        float fallingPieceSize = transform.localScale.x - newSizeOnX;

        UpdateScore(newSizeOnX);

        float newPositionOnX = previousPlatform.transform.position.x + platformDifference / 2.0f;

        transform.localScale = new Vector3(newSizeOnX, transform.localScale.y, transform.localScale.z);
        transform.position = new Vector3(newPositionOnX, transform.position.y, transform.position.z);

        float edge = transform.position.x + (newSizeOnX / 2.0f * direction);
        float fallingPiecePosition = edge + (fallingPieceSize / 2.0f * direction);

        SpawnFallingPiece(fallingPiecePosition, fallingPieceSize);
    }

    private void SlicePlatformZ(float platformDifference, float direction) {
        float newSizeOnZ = previousPlatform.transform.localScale.z - Mathf.Abs(platformDifference);
        float fallingPieceSize = transform.localScale.z - newSizeOnZ;

        UpdateScore(newSizeOnZ);

        float newPositionOnZ = previousPlatform.transform.position.z + platformDifference / 2.0f;

        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newSizeOnZ);
        transform.position = new Vector3(transform.position.x, transform.position.y, newPositionOnZ);

        float edge = transform.position.z + (newSizeOnZ / 2.0f * direction);
        float fallingPiecePosition = edge + (fallingPieceSize / 2.0f * direction);

        SpawnFallingPiece(fallingPiecePosition, fallingPieceSize);
    }

    private void SpawnFallingPiece(float position, float size) {
        GameObject platform = Instantiate(fallingPiece);

        if (MoveDirection == MoveDirection.Z) {
            platform.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, size);
            platform.transform.position = new Vector3(transform.position.x, transform.position.y, position);
        } else {
            platform.transform.localScale = new Vector3(size, transform.localScale.y, transform.localScale.z);
            platform.transform.position = new Vector3(position, transform.position.y, transform.position.z);
        }

        platform.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;
    }
}
