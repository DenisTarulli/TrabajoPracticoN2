using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
	[SerializeField] private float moveSpeed;

    private void Update ()
    {
        Movement(GetInputs());
    }

    private void Movement(Vector3 moveDir)
    {
        transform.position += moveSpeed * Time.deltaTime * moveDir;
    }

    private Vector3 GetInputs()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new(xInput, 0f, zInput);

        moveDirection.Normalize();

        return moveDirection;
    }
}
