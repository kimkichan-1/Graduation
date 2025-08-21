using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxEffect;
    [SerializeField] private bool infiniteScroll;
    [SerializeField] private float textureUnitSizeX;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        lastCameraPosition = new Vector3(Mathf.Clamp(cameraTransform.position.x, minX, maxX), cameraTransform.position.y, cameraTransform.position.z);
        
        if (infiniteScroll && textureUnitSizeX == 0)
        {
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            if (sprite != null)
            {
                Texture2D texture = sprite.texture;
                textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
            }
        }
    }

    void LateUpdate()

    {
        float clampedCameraX = Mathf.Clamp(cameraTransform.position.x, minX, maxX);
        Vector3 currentCameraPosition = new Vector3(clampedCameraX, cameraTransform.position.y, cameraTransform.position.z);
        
        Vector3 deltaMovement = currentCameraPosition - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, 0, 0);
        lastCameraPosition = currentCameraPosition;

        if (infiniteScroll)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x - offsetPositionX, transform.position.y, transform.position.z);
            }
        }
    }
}