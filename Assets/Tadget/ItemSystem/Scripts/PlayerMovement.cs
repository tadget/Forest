using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tadget.PlayerStuff
{
    public class PlayerMovement : MonoBehaviour
    {
        public float jumpForce = 5;
        public float SpeedMultiplyer = 1f;
        public float maxSpeed = 8;
        public float rotationSpeed = 180;
        public float moveControlBoundsX = 0.35f;
        public float moveControlBoundsY = 0.5f;

        public bool doMove = true;

        Camera mainCam;
        Rigidbody meRigid;

        bool isGrounded = false;
        bool autoJump = false;
        float timer;
        Vector3 moveDir;

        float camX;
        float camY;

        Vector2 moveTouchStart;
        public Vector2 screenRes;

        int idOfMoveTouch = -1;
        float timer1;

        void Start()
        {
            mainCam = Camera.main;
            meRigid = GetComponent<Rigidbody>();
            screenRes = new Vector2(Screen.width, Screen.height);
        }

        void Update()
        {
            moveDir = Vector3.zero;
            for (int i = 0; i < Input.touchCount; i++) // Mobile controlls
            {
                Touch touch = Input.touches[i];


                if (touch.phase == TouchPhase.Ended)
                {
                    if (idOfMoveTouch == touch.fingerId)
                    {
                        idOfMoveTouch = -1;
                    }

                }

                if (touch.phase == TouchPhase.Began)
                {
                    if (idOfMoveTouch == -1)
                    {
                        moveTouchStart = new Vector2(touch.position.x / screenRes.x, touch.position.y / screenRes.y);
                        if (moveTouchStart.x < moveControlBoundsX && moveTouchStart.y < moveControlBoundsY)
                        {
                            idOfMoveTouch = touch.fingerId;
                        }
                    }
                    if (doMove && idOfMoveTouch != touch.fingerId)
                    {
                        if (timer1 + 0.3f > Time.time)
                        {
                            TryJump();
                            if (isGrounded == false)
                            {
                                autoJump = true;
                                timer = Time.time;
                            }
                        }
                        timer1 = Time.time;
                    }
                }

                if (idOfMoveTouch == touch.fingerId && doMove) // check for movement
                {
                    moveDir = new Vector3((touch.position.x / screenRes.x) - moveTouchStart.x, 0, (touch.position.y / screenRes.y) - moveTouchStart.y) * 5;
                    if (moveDir.sqrMagnitude > 1)
                    {
                        moveDir.Normalize();
                    }
                }

                if (idOfMoveTouch != touch.fingerId) // check for rotation
                {
                    if (doMove && touch.phase == TouchPhase.Moved)
                    {
                        camY += touch.deltaPosition.x / screenRes.x * rotationSpeed;
                        camX -= touch.deltaPosition.y / screenRes.x * rotationSpeed;
                        camX = Mathf.Clamp(camX, -90, 90);

                        mainCam.transform.localRotation = Quaternion.Euler(camX, 0, 0);
                        transform.rotation = Quaternion.Euler(0, camY, 0);
                    }
                }
            }

        }
        private void FixedUpdate()
        {
            //  Debug.Log(meRigid.velocity.magnitude);
            if (Physics.SphereCast(new Ray(transform.position, Vector3.down), 0.4f, 0.65f))
            {
                isGrounded = true;
                meRigid.drag = 6;
            }
            else
            {
                meRigid.drag = 0.5f;
                isGrounded = false;
            }

            if (autoJump && timer + 0.25f > Time.time)
            {
                TryJump();
            }
            else
            {
                autoJump = false;
            }

            //Move

            if (isGrounded)
            {
                meRigid.AddRelativeForce(moveDir * SpeedMultiplyer, ForceMode.VelocityChange);
            }
            else
            {
                meRigid.AddRelativeForce(moveDir * SpeedMultiplyer / 2f, ForceMode.VelocityChange);
            }
            Vector3 horiVeloc = meRigid.velocity;
            horiVeloc.y = 0;

            if (moveDir.sqrMagnitude < 0.1f)
            {
                horiVeloc /= 1.1f;
                if (horiVeloc.sqrMagnitude < 0.1f) 
                {
                    horiVeloc = Vector3.zero;
                }
            }

            if (horiVeloc.sqrMagnitude > maxSpeed * maxSpeed)
            {
                horiVeloc /= horiVeloc.magnitude / maxSpeed;
            }

            horiVeloc.y = meRigid.velocity.y;
            meRigid.velocity = horiVeloc;
            
        }
        private void TryJump()
        {
            if (isGrounded)
            {
                autoJump = false;
                isGrounded = false;
                meRigid.velocity = new Vector3(meRigid.velocity.x, 5f, meRigid.velocity.z);
                meRigid.drag = 0.5f;
            }
        }
    }
}
