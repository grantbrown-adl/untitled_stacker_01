using UnityEngine;

public class JellyEffect : MonoBehaviour {
    [SerializeField] Mesh initialMesh;
    [SerializeField] Mesh alteredMesh;
    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField] JellyVertex[] jellyVertices;
    [SerializeField] Vector3[] vertexArray;
    [SerializeField] float jellyIntensity = 1.0f;
    [SerializeField] float jellyMass = 1.0f;
    [SerializeField] float jellyRigidity = 1.0f;
    [SerializeField] float jellyDamping = 0.75f;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();

        initialMesh = GetComponent<MeshFilter>().sharedMesh;
        alteredMesh = Instantiate(initialMesh);
        GetComponent<MeshFilter>().sharedMesh = alteredMesh;

        jellyVertices = new JellyVertex[initialMesh.vertices.Length];

        for (int i = 0; i < jellyVertices.Length; i++) {
            jellyVertices[i] = new JellyVertex(i, transform.TransformPoint(alteredMesh.vertices[i]));
        }
    }

    private void FixedUpdate() {
        vertexArray = initialMesh.vertices;

        for (int i = 0; i < jellyVertices.Length; i++) {
            Vector3 target = transform.TransformPoint(vertexArray[jellyVertices[i].index]);

            float intensity = (1 - (meshRenderer.bounds.max.y - target.y) / meshRenderer.bounds.size.y) * jellyIntensity;

            jellyVertices[i].Jiggle(target, jellyMass, jellyRigidity, jellyDamping);

            target = transform.InverseTransformPoint(jellyVertices[i].position);
            vertexArray[jellyVertices[i].index] = Vector3.Lerp(vertexArray[jellyVertices[i].index], target, intensity);
        }

        alteredMesh.vertices = vertexArray;
    }

    public class JellyVertex {
        public int index;
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 force;

        public JellyVertex(int index, Vector3 position) {
            this.index = index;
            this.position = position;
        }

        public void Jiggle(Vector3 target, float mass, float rigidity, float damping) {
            force = (target - position) * rigidity;
            velocity = (velocity + force / mass) * damping;
            position += velocity;

            if ((velocity + force + force / mass).magnitude < Mathf.Epsilon) {
                position = target;
            }
        }
    }
}