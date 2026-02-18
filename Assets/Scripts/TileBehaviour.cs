using UnityEngine;
public class TileBehaviour : MonoBehaviour
{
    public Renderer rend;
    public AudioClip hitSound;           
    private AudioSource audioSource;     

    private Rigidbody rb;
    private Color defaultColor;
    private float initialZ;

    public Color DefaultColor => defaultColor;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultColor = rend.material.color;
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (transform.position.z > initialZ)
        {
            Vector3 pos = transform.position;
            pos.z = initialZ;
            transform.position = pos;

            // Reset velocity so it doesn't keep pushing forward
            if (rb.linearVelocity.z > 0)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, 0f);
        }
    }

    void OnMouseDown()
    {
        Vector3 hitDirection;

        if (WallGenerator.Instance.CameraDir)
        {
            // Direction from camera to this object
            hitDirection = (transform.position - Camera.main.transform.position).normalized;
        }
        else
        {
            // Fixed backward direction in world space
            hitDirection = Vector3.back;
        }

        rb.AddForce(hitDirection * Random.Range(65f, 75f), ForceMode.Impulse);

        WallGenerator.Instance.StartSpreadEffect(transform.position);
        audioSource.PlayOneShot(hitSound);
    }

    public void SetColor(Color color) => rend.material.color = color;
    public void ResetColor() => rend.material.color = defaultColor;
}
