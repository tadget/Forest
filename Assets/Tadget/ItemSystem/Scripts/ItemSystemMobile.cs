using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tadget.PlayerStuff;

namespace Tadget
{
    public class ItemSystemMobile : MonoBehaviour
    {
        Camera MainCam;
        public LayerMask BuildMask;
        public LayerMask OnlyPickable;
        public LayerMask OnlyPlacable;

        public GameObject ItemInHand;
        public GameObject Hand;

        SpringJoint tempSpring;
        Rigidbody tempRigid;
        PlayerMovement meMario; //lol

        int holdMode = 0; //0 = none, 1= holding, 2 = action mode, 3 = build mode;
        float tapTime;


        bool touchCooldown = false;
        Vector2 touchZoomBegan;

        float zoom;
        float rotation;

        int itemTouchId = -1;

        private void Start()
        {
            //Debug.Log(BuildMask.value);
            MainCam = Camera.main;
            meMario = GetComponent<PlayerMovement>();
        }
        private void Update()
        {
            for (int i = 0; i < Input.touchCount; i++) // Mobile controlls
            {
                Touch touch = Input.touches[i];

                if (holdMode == 0) // not picked up
                {
                    if (itemTouchId == -1)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            if (tapTime + 0.3f > Time.time)
                            {
                                Debug.Log("Mode: 0 - double tap - use item");
                                UseItemFunction(touch.position);
                                meMario.doMove = false;
                            }
                            if (IsPressedOnItem(touch.position))
                            {
                                Debug.Log("Mode: 0 - touch began");

                                tapTime = Time.time;
                                itemTouchId = touch.fingerId;
                                break;
                            }
                        }
                    }

                    if (touch.fingerId == itemTouchId)
                    {
                        if (touch.phase == TouchPhase.Stationary)
                        {
                            if (Time.time > tapTime + 0.25f)
                            {
                                if (IsPressedOnItem(touch.position))
                                {
                                    Debug.Log("Mode: 0 - pick up item");

                                    itemTouchId = -1;
                                    TryPickUpItem(touch.position);
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("Mode: 0 - pick up failed" + " mode: " + touch.phase);
                            itemTouchId = -1;
                        }
                    }
                }     // not picked up

                else if (holdMode == 1) // holding mode
                {
                    if (itemTouchId == -1 || itemTouchId == touch.fingerId)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            if (IsPressedOnItemInHand(touch.position))
                            {
                                itemTouchId = touch.fingerId;
                                if (tapTime + 0.3f > Time.time)//if double tap
                                {
                                    Debug.Log("Mode: 1 - double tapped, to action mode");
                                    itemTouchId = -1;
                                    InitiateActionMode();
                                }
                                tapTime = Time.time;
                            }
                            else
                            {
                                //Debug.Log("Mode: 1 - missed double tapped");
                                itemTouchId = -1;
                            }
                        }
                    }
                    if (itemTouchId == touch.fingerId)
                    {
                        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                        {
                            if (tapTime + 0.3f < Time.time)
                            {
                                Debug.Log("Mode: 1 - long press, to build mode");

                                InitiateBuildMode();
                            }
                        }


                        if (touch.phase == TouchPhase.Ended)
                        {
                            if (!IsPressedOnItemInHand(touch.position))
                            {
                                Debug.Log("Mode: 1 - short swipe, drop item");
                                itemTouchId = -1;
                                DropItem();
                            }
                        }
                    }
                }// holding mode

                else if (holdMode == 2)// action mode
                {
                    if (itemTouchId == -1 || itemTouchId == touch.fingerId)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            if (IsPressedOnItemInHand(touch.position))
                            {
                                itemTouchId = touch.fingerId;
                                if (tapTime + 0.3f > Time.time)//if double tap
                                {
                                    Debug.Log("Mode: 2 - double tap, reset item");
                                    itemTouchId = -1;
                                    ResetItem();
                                }
                                tapTime = Time.time;
                            }
                            else
                            {
                                Debug.Log("Mode: 2 - missed double tapped");
                                itemTouchId = -1;
                            }
                        }
                    }
                    if (itemTouchId == touch.fingerId && touch.phase == TouchPhase.Stationary && Time.time > tapTime + 0.3f)
                    {
                        Debug.Log("Mode: 2 - call function of item");
                        itemTouchId = -1;
                        UseItemFunction(touch.position);

                    }
                }// action mode

                if (holdMode == 3) // build mode
                {
                    if (touch.fingerId == itemTouchId)
                    {
                        BuildModeDragNDrop(touch.position, touch.phase == TouchPhase.Ended);

                        if (touch.phase == TouchPhase.Ended)
                        {
                            itemTouchId = -1;
                        }
                    }
                    else
                    {
                        if (touch.phase == TouchPhase.Moved)
                        {
                            zoom += touch.deltaPosition.y / meMario.screenRes.y * 5;
                            zoom = Mathf.Clamp(zoom, 2f, 5f);
                            rotation -= touch.deltaPosition.x / meMario.screenRes.x * 360;
                        }
                    }

                }// build mode
            }

            if (itemTouchId != -1)
            {
                meMario.doMove = false;
            }
            else
            {
                meMario.doMove = true;
            }
        }
        private void FixedUpdate()
        {
            if (tempSpring == null && ItemInHand != null)
            {
                DropItem();
                Debug.Log("broke");
            }

            if (holdMode == 1)
            {
                Hand.transform.position = MainCam.transform.position + new Vector3(0, -1f, 0) + MainCam.transform.forward * zoom;
                ItemInHand.transform.rotation = Quaternion.Lerp(ItemInHand.transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0), 0.25f);
                // tempSpring.anchor = transform.transform.InverseTransformPoint(MainCam.transform.position + MainCam.transform.forward * zoom);
            }
            else if (holdMode == 2)
            {
                Hand.transform.position = MainCam.transform.position + new Vector3(0, -1f, 0) + MainCam.transform.forward * 2 + transform.right * 1.5f;
                ItemInHand.transform.rotation = Quaternion.Lerp(ItemInHand.transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0), 0.25f);
            }
            else if (holdMode == 3 && !isLookingAtStorage)
            {
                Hand.transform.position = snappedPos;
                ItemInHand.transform.rotation = Quaternion.Lerp(ItemInHand.transform.rotation, snappedRot, 0.25f);
            }

            if (ItemInHand != null)
            {
                if (ItemInHand.transform.localScale.x != 1 && !isLookingAtStorage)
                {
                    ItemInHand.transform.localScale = Vector3.Lerp(ItemInHand.transform.localScale, Vector3.one, 0.2f);
                }
                NudgeItem();
            }
        }

        bool IsPressedOnItem(Vector3 pressPosition)
        {
            return Physics.Raycast(MainCam.ScreenPointToRay(pressPosition), 5, OnlyPickable); // layer 9 = pickable
        }

        bool IsPressedOnItemInHand(Vector3 pressPosition)
        {
            RaycastHit hit;
            if (Physics.Raycast(MainCam.ScreenPointToRay(pressPosition), out hit, 5, OnlyPlacable))
            {
                if (ItemInHand == null)
                {
                    return false;
                }
                return hit.transform.gameObject == ItemInHand ? true : false;
            }
            return false;
        }

        void TryPickUpItem(Vector3 pressPosition)
        {
            RaycastHit hit;
            if (Physics.Raycast(MainCam.ScreenPointToRay(pressPosition), out hit, 5, OnlyPickable))
            {
                zoom = Mathf.Clamp(hit.distance, 2f, 5f);
                holdMode = 1;

                ItemInHand = hit.transform.gameObject;

                PickUpItem();
            }
        }

        void PickUpItem()
        {
            ItemInHand.transform.parent = null;
            ChangeLayerOfItemHierarchy(10);

            tempSpring = Hand.AddComponent<SpringJoint>();
            tempRigid = ItemInHand.GetComponent<Rigidbody>();

            tempSpring.connectedBody = tempRigid;
            tempSpring.spring = 200;
            tempSpring.breakForce = 2000;
            tempSpring.anchor = new Vector3(0, 0, 0);
            tempSpring.autoConfigureConnectedAnchor = false;
            tempSpring.connectedAnchor = new Vector3(0, 0, 0);

            tempRigid.drag = 10;
            tempRigid.isKinematic = false;
            tempRigid.constraints = RigidbodyConstraints.FreezeRotation;
            tempRigid.useGravity = false;

            ChangeColliderOfItemHierarchy(true);

            if (ItemInHand.GetComponent<Item>())
            {
                ChangeTriggerOfItemHierarchy(false);
            }
        }

        Vector3 snappedPos;
        Quaternion snappedRot;
        bool isLookingAtStorage = false;

        void BuildModeDragNDrop(Vector3 pressPosition, bool placeCall)
        {
            RaycastHit hit;
            if (Physics.Raycast(MainCam.ScreenPointToRay(pressPosition), out hit, zoom, BuildMask))
            {
                if (hit.transform.GetComponent<Storage>() && ItemInHand.GetComponent<Item>()) // place on storage
                {
                    if (!isLookingAtStorage)
                    {
                        ChangeTriggerOfItemHierarchy(true);
                        isLookingAtStorage = true;
                    }
                    hit.transform.GetComponent<Storage>().DoTheThing(hit.point, Hand.transform, ItemInHand.transform, placeCall);
                }
                else // place on ground
                {
                    if (isLookingAtStorage)
                    {
                        ChangeTriggerOfItemHierarchy(false);
                        isLookingAtStorage = false;
                    }

                    snappedPos = new Vector3(Mathf.RoundToInt(hit.point.x * 2) / 2f,
                        Mathf.RoundToInt(hit.point.y * 2) / 2f,
                        Mathf.RoundToInt(hit.point.z * 2) / 2f);

                    snappedRot = Quaternion.Euler(0, Mathf.RoundToInt(rotation / 45f) * 45, 0);

                    //tempSpring.anchor = transform.InverseTransformPoint(snappedPos);
                    if (placeCall)
                    {
                        ItemInHand.transform.position = new Vector3(Mathf.RoundToInt(ItemInHand.transform.position.x * 2) / 2f,
                            Mathf.RoundToInt(ItemInHand.transform.position.y * 2) / 2f,
                            Mathf.RoundToInt(ItemInHand.transform.position.z * 2) / 2f);
                        ItemInHand.transform.rotation = snappedRot;
                    }
                }

                if (placeCall)
                {
                    Debug.Log("PLACE");
                    isLookingAtStorage = false;
                    tempRigid.isKinematic = true;
                    DropItem();
                }
            }
            else
            {
                if (isLookingAtStorage)
                {
                    ChangeTriggerOfItemHierarchy(false);
                    isLookingAtStorage = false;
                }
                snappedPos = MainCam.transform.position + MainCam.ScreenPointToRay(pressPosition).direction * zoom;
                //Debug.Log(zoom);
                snappedRot = Quaternion.Euler(0, rotation, 0);

                //tempSpring.anchor = transform.InverseTransformPoint(snappedPos);
                if (placeCall)
                {
                    Debug.Log("RESET");
                    ResetItem();
                }
            }
        }

        void InitiateActionMode()
        {
            holdMode = 2;
        }

        void InitiateBuildMode()
        {
            rotation = MainCam.transform.eulerAngles.y + 180;
            holdMode = 3;
            tempRigid.drag = 20;
        }

        void DropItem()
        {
            holdMode = 0;

            Destroy(tempSpring);
            tempRigid = ItemInHand.GetComponent<Rigidbody>();

            tempRigid.drag = 0;
            tempRigid.constraints = RigidbodyConstraints.None;
            tempRigid.useGravity = true;
            NudgeItem();
            ChangeColliderOfItemHierarchy(false);

            ChangeLayerOfItemHierarchy(9);
            ItemInHand = null;

        }

        void ResetItem()
        {

            touchCooldown = true;
            GameObject tempItem = ItemInHand;

            DropItem();

            ItemInHand = tempItem;
            PickUpItem();
            holdMode = 1;
        }

        void NudgeItem()
        {
            tempRigid.AddForce(0, 0.05f, 0);
        }

        void ChangeLayerOfItemHierarchy(int newLayer)
        {
            List<Transform> childs = new List<Transform>();
            childs.Add(ItemInHand.transform);
            while (childs.Count != 0)
            {
                Transform child = childs[0];
                childs.RemoveAt(0);
                for (int i = 0; i < child.childCount; i++)
                {
                    childs.Add(child.GetChild(i));
                }
                child.gameObject.layer = newLayer;
            }
        }
        void ChangeColliderOfItemHierarchy(bool isNoFriction)
        {
            List<Transform> childs = new List<Transform>();
            childs.Add(ItemInHand.transform);
            while (childs.Count != 0)
            {
                Transform child = childs[0];
                childs.RemoveAt(0);
                for (int i = 0; i < child.childCount; i++)
                {
                    childs.Add(child.GetChild(i));
                }

                if (child.GetComponent<Collider>())
                {
                    if (isNoFriction)
                    {
                        PhysicMaterial mat = new PhysicMaterial("noFriction");
                        mat.dynamicFriction = 0;
                        mat.staticFriction = 0;
                        mat.bounciness = 0;
                        mat.bounceCombine = PhysicMaterialCombine.Minimum;
                        mat.frictionCombine = PhysicMaterialCombine.Minimum;
                        child.GetComponent<Collider>().material = mat;
                    }
                    else
                    {
                        child.GetComponent<Collider>().material = null;
                    }
                }
            }
        }
        void ChangeTriggerOfItemHierarchy(bool isTrigger)
        {
            List<Transform> childs = new List<Transform>();
            childs.Add(ItemInHand.transform);
            while (childs.Count != 0)
            {
                Transform child = childs[0];
                childs.RemoveAt(0);
                for (int i = 0; i < child.childCount; i++)
                {
                    childs.Add(child.GetChild(i));
                }

                if (child.GetComponent<Collider>())
                {
                    child.GetComponent<Collider>().isTrigger = isTrigger;
                }
            }
        }

        void UseItemFunction(Vector3 pressPosition)
        {
            RaycastHit hit;
            if (Physics.Raycast(MainCam.ScreenPointToRay(pressPosition), out hit, 5, OnlyPickable + OnlyPlacable))
            {
                if (hit.transform.GetComponent<Crafting>())
                {
                    hit.transform.GetComponent<Crafting>().Check4Craft();
                }
            }
        }
    }
}