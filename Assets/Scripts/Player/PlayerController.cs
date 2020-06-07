using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController ctrl;
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public GameObject tablet;

    Vector3 velocity;
    bool isGrounded;
    bool tabletActive = false;
    bool alarmActive = false;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private void Start() {
        tablet.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move =  transform.right * x + transform.forward * z;

        ctrl.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if(Input.GetKeyDown(KeyCode.T)) {
            tabletActive = !tabletActive;
            tablet.SetActive(tabletActive);
        }

        if(tabletActive && Input.GetKeyDown(KeyCode.I)) {
            alarmActive = !alarmActive;
            GameObject.Find("SecurityCamera01").GetComponent<Alarm>().alarmActive = alarmActive;
        }

        velocity.y += gravity * Time.deltaTime;
        ctrl.Move(velocity * Time.deltaTime);
    }
}
