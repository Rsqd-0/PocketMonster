using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f; 
    private Mover mover;
    private void Start()
    {
        mover = gameObject.GetComponent<Mover>();
    }

    private void Update()
    {
        if (Time.timeScale > 0)
        {
            var timeScaledSpeed = speed * Time.deltaTime;
            mover.Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized *
                       timeScaledSpeed);
            //mover.LookAt(Camera.main.transform.forward);
        }
    }
}