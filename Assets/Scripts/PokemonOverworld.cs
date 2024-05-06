using System.Collections;
using UnityEngine;

public class PokemonOverworld : MonoBehaviour
{
    private Vector3 initialPosition;
    private float moveSpeed = 1f;
    private float movementRadius = 2f;
    private float moveInterval = 3f;
    private bool isMoving = false;
    private Coroutine movementCoroutine;

    private void Awake()
    {
        initialPosition = transform.parent.position;
    }

    private void Start()
    {
        StartMovement();
    }

    private void StartMovement()
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);
        
        movementCoroutine = StartCoroutine(RandomMovement());
    }

    private IEnumerator RandomMovement()
    {
        while (true)
        {
            if (!isMoving)
            {
                Vector3 targetPosition = initialPosition + Random.insideUnitSphere * movementRadius;
                targetPosition.y = transform.position.y;

                yield return MovePokemon(targetPosition);

                yield return new WaitForSeconds(moveInterval);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator MovePokemon(Vector3 targetPosition)
    {
        isMoving = true;

        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position += moveDirection * (Time.deltaTime * moveSpeed);

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            yield return null;
        }

        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement player))
        {
            Game.Instance.FightScene(gameObject);
        }
    }

    public void StopMovement()
    {
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);
        
        isMoving = false;
    }

    public void ResumeMovement()
    {
        StartMovement();
    }
}
