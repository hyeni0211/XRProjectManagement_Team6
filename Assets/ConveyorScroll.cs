using UnityEngine;

public class ConveyorScroll : MonoBehaviour
{
    public float scrollSpeed = 1f;
    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        mat.mainTextureOffset = new Vector2(offset, 0);
    }
}