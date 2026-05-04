using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }
}
