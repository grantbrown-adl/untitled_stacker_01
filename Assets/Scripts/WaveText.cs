using TMPro;
using UnityEngine;

public class WaveText : MonoBehaviour {
    public TMP_Text tmpText;
    public float amplitude = 5.0f;  // Height of the wave
    public float frequency = 1.0f;  // Speed of the wave

    private TMP_TextInfo textInfo;

    void Start() {
        // Cache the TextMeshPro component
        tmpText = GetComponent<TMP_Text>();
        textInfo = tmpText.textInfo;
    }

    void Update() {
        tmpText.ForceMeshUpdate();

        // Get new copy of vertex data if the text has changed
        textInfo = tmpText.textInfo;

        int characterCount = textInfo.characterCount;

        for (int i = 0; i < characterCount; i++) {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            float waveValue = Mathf.Sin(Time.time * frequency + i * 0.5f) * amplitude;
            vertices[vertexIndex + 0].y += waveValue;
            vertices[vertexIndex + 1].y += waveValue;
            vertices[vertexIndex + 2].y += waveValue;
            vertices[vertexIndex + 3].y += waveValue;
        }

        // Push changes into the mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++) {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}