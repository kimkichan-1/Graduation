using UnityEngine;

public class ParallaxBackground2 : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxEffect;
    [SerializeField] private bool infiniteScroll;
    [SerializeField] private float textureUnitSizeY;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = new Vector3(cameraTransform.position.x, Mathf.Clamp(cameraTransform.position.y, minY, maxY), cameraTransform.position.z);
        
        if (infiniteScroll && textureUnitSizeY == 0)
        {
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            if (sprite != null)
            {
                Texture2D texture = sprite.texture;
                textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
            }
        }
    }

    void LateUpdate()
    {
        float clampedCameraY = Mathf.Clamp(cameraTransform.position.y, minY, maxY);
        Vector3 currentCameraPosition = new Vector3(cameraTransform.position.x, clampedCameraY, cameraTransform.position.z);
        
        Vector3 deltaMovement = currentCameraPosition - lastCameraPosition;
        transform.position += new Vector3(0, deltaMovement.y * parallaxEffect, 0);
        lastCameraPosition = currentCameraPosition;

        if (infiniteScroll)
        {
            if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
            {
                float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                transform.position = new Vector3(transform.position.x, cameraTransform.position.y - offsetPositionY, transform.position.z);
            }
        }
    }
}
