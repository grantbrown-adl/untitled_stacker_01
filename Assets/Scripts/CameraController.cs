using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] Transform[] points;
    [SerializeField] int index = 0;
    [SerializeField] int modulo = 4;
    [SerializeField] Vector3 offset;

    [SerializeField] float transitionDuration = 1.0f;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float transitionProgress = 0f;

    [Header("Angle Lerping")]
    [SerializeField] Vector3 targetEulerAngles;
    [SerializeField] float lookAtLerpSpeed = 0.1f;
    [SerializeField] bool lookAt;

    public Vector3 Offset { get => offset; set => offset = value; }

    void Awake() {
        LookAtOrigin();
    }

    void Update() {
        //LerpLookatAngles();

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) {
            int indexDirection = Input.GetKeyDown(KeyCode.A) ? -1 : 1;
            index = WrapIndex(index + indexDirection, modulo);
            StartTransition();
        }

        if (GameManager.Instance.CameraTransitioning) {
            UpdateTransition();
        }
    }

    public void LookAtOrigin() {
        transform.position = points[index].position;
        //transform.rotation = points[index].rotation;
        if (lookAt) transform.LookAt(Vector3.zero);

        Vector3 eulerAngles = transform.eulerAngles;
        //Vector3 eulerAngles = currentEulerRotation;
        eulerAngles += offset;
        transform.eulerAngles = eulerAngles;

        //StartCoroutine(LerpItHard(eulerAngles, lookAtLerpSpeed));
    }

    IEnumerator LerpItHard(Vector3 targetEulerAngles, float duration) {
        Vector3 startEulerAngles = transform.eulerAngles;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            float newX = Mathf.LerpAngle(startEulerAngles.x, targetEulerAngles.x, t);
            float newY = Mathf.LerpAngle(startEulerAngles.y, targetEulerAngles.y, t);
            float newZ = Mathf.LerpAngle(startEulerAngles.z, targetEulerAngles.z, t);

            transform.eulerAngles = new Vector3(newX, newY, newZ);

            yield return null;
        }

        transform.eulerAngles = targetEulerAngles;
        points[index].eulerAngles = targetEulerAngles;
        //currentEulerRotation = transform.eulerAngles;
    }

    private void LerpLookatAngles() {
        Vector3 currentEulerAngles = transform.eulerAngles;
        targetEulerAngles = currentEulerAngles + offset;

        float newX = Mathf.LerpAngle(currentEulerAngles.x, targetEulerAngles.x, lookAtLerpSpeed * Time.deltaTime);
        float newY = Mathf.LerpAngle(currentEulerAngles.y, targetEulerAngles.y, lookAtLerpSpeed * Time.deltaTime);
        float newZ = Mathf.LerpAngle(currentEulerAngles.z, targetEulerAngles.z, lookAtLerpSpeed * Time.deltaTime);

        transform.eulerAngles = new Vector3(newX, newY, newZ);
    }

    private void StartTransition() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        targetPosition = points[index].position;
        targetRotation = Quaternion.LookRotation(Vector3.zero - targetPosition);

        Vector3 targetEulerAngles = targetRotation.eulerAngles;
        targetEulerAngles += offset;
        targetRotation = Quaternion.Euler(targetEulerAngles);

        GameManager.Instance.CameraTransitioning = true;
        transitionProgress = 0f;
    }

    private void UpdateTransition() {
        transitionProgress += Time.deltaTime / transitionDuration;
        if (transitionProgress >= 1f) {
            transitionProgress = 1f;
            GameManager.Instance.CameraTransitioning = false;
        }

        transform.position = Vector3.Lerp(startPosition, targetPosition, transitionProgress);
        transform.rotation = Quaternion.Lerp(startRotation, targetRotation, transitionProgress);
    }

    private int WrapIndex(int value, int modulo) {
        return (value % modulo + modulo) % modulo;
    }
}