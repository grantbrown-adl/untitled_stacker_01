using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingTextEffect : MonoBehaviour {
    [SerializeField] TMP_Text loadingText;
    [SerializeField] float interval = 0.5f;
    [SerializeField] int suffixCount = 3;
    [SerializeField] string baseText = "LOADING";
    [SerializeField] char suffix = '.';

    void Start() {
        loadingText = GetComponent<TMP_Text>();
        StartCoroutine(Animate());
    }

    IEnumerator Animate() {
        int dotCount = 0;

        while (true) {
            dotCount = (dotCount + 1) % (suffixCount + 1);
            loadingText.text = baseText + new string(suffix, dotCount);

            yield return new WaitForSecondsRealtime(interval);
        }
    }
}