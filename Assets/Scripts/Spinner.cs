using UnityEngine;

public class Spinner : MonoBehaviour
{
    // Config Settings
    [SerializeField] float rotationSpeed = 150;

    // Update is called once per frame
    void Update()
    {
        var rotation = new Vector3(0, 0, rotationSpeed * Time.deltaTime);
        transform.Rotate(rotation);
    }
}
