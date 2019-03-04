namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerMovement : MonoBehaviour
    {
        public LayerMask notPlacable;

        public float jumpForce = 5;
        public float speedMultiplier = 1f;
        public float maxSpeed = 8;
        public float rotationSpeed = 180;
        public float moveControlBoundsX = 0.35f;
        public float moveControlBoundsY = 0.5f;
        public Vector2 screenRes;

        public bool doMove = true;

        private Camera mainCam;
        private Rigidbody meRigid;

        private bool isGrounded = false;
        private bool autoJump = false;
        private float timer;
        private Vector3 moveDir;

        private float camX;
        private float camY;

        private Vector2 moveTouchStart;

        private int idOfMoveTouch = -1;
        private float timer1;

        private void Start()
        {
            mainCam = Camera.main;
            meRigid = GetComponent<Rigidbody>();
            screenRes = new Vector2(Screen.width, Screen.height);
        }

        private void Update()
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
                    moveDir = new Vector3((touch.position.x / screenRes.x) - moveTouchStart.x, 0,
                                  (touch.position.y / screenRes.y) - moveTouchStart.y) * 5;
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
            if (Physics.SphereCast(new Ray(transform.position, Vector3.down), 0.4f, 0.65f, notPlacable, QueryTriggerInteraction.Ignore))
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
                meRigid.AddRelativeForce(moveDir * speedMultiplier, ForceMode.VelocityChange);
            }
            else
            {
                meRigid.AddRelativeForce(moveDir * speedMultiplier / 2f, ForceMode.VelocityChange);
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
                meRigid.velocity = new Vector3(meRigid.velocity.x, jumpForce, meRigid.velocity.z);
                meRigid.drag = 0.5f;
            }
        }
    }
}
