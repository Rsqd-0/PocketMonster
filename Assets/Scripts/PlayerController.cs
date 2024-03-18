using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f; 
    private Mover mover;
    private Camera mainCamera;
    private void Start()
    {
        mover = gameObject.GetComponent<Mover>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Time.timeScale > 0)
        {
            var timeScaledSpeed = speed * Time.deltaTime;
            var direction = new Vector3();
            
            if (Input.GetKey(KeyCode.W))
            {
                direction = mainCamera.transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction = mainCamera.transform.forward * -1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction = mainCamera.transform.right * -1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction = mainCamera.transform.right;
            }

            direction.y = 0;
            mover.Move(direction * timeScaledSpeed);
            //mover.LookAt(Camera.main.transform.forward);
        }
    }
}