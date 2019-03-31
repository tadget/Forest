namespace Tadget
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class ItemSystemMobile : MonoBehaviour
    {
        public PhysicMaterial noFric;
        public Material buildMat;
        public Material buildSnapMat;
        private Dictionary<Renderer, Material[]> rendererMaterialDict;

        public LayerMask buildMask;
        public LayerMask onlyPickable;
        public LayerMask onlyPlaceable;
        public LayerMask onlyInteractable;

        public GameObject itemInHand;
        public GameObject hand;

        private Camera mainCam;
        private SpringJoint tempSpring;
        private Rigidbody itemInHandRigidbody;
        private PlayerMovement playerMovement;

        private HoldMode holdMode;
        private float lastItemBeginTouchTime;
        private float lastItemEndTouchTime;
        private int lastItemTouchFingerId = -1;

        private Vector2 touchZoomBegan;

        private float zoom;
        private float rotation;

        private enum HoldMode
        {
            NONE = 0,
            HOLDING = 1,
            ACTION = 2,
            BUILD = 3
        }

        private void Start()
        {
            mainCam = Camera.main;
            playerMovement = GetComponent<PlayerMovement>();
        }

        private Touch touch;

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                TouchItem();
            }


            /*for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);

                switch (holdMode)
                {
                    case HoldMode.NONE:
                        ProcessHoldModeNone();
                        break;
                    case HoldMode.HOLDING:
                        ProcessHoldModeHolding();
                        break;
                    case HoldMode.ACTION:
                        break;
                    case HoldMode.BUILD:
                        ProcessHoldModeBuild();
                        break;
                    default:
                        break;
                }

                playerMovement.doMove = lastItemTouchFingerId == -1;

            }*/
        }

        private float lastInteractableTouchTime;
        private void TouchItem()
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                {
                    if(IsTouchingItem(touch.position, onlyInteractable))
                        lastInteractableTouchTime = Time.time;
                    break;
                }
                case TouchPhase.Stationary:
                {
                    if (Time.time - lastInteractableTouchTime > 1f)
                        TryUseItem(touch.position, onlyInteractable);
                    break;
                }
                case TouchPhase.Ended:
                {
                    break;
                }
            }
        }

        private void ProcessHoldModeNone()
        {
            if (lastItemTouchFingerId == -1)
            {
                if (touch.phase == TouchPhase.Began && IsTouchingItem(touch.position, onlyPickable))
                {
                    lastItemBeginTouchTime = Time.time;
                    lastItemTouchFingerId = touch.fingerId;
                }
            }
            else if (lastItemTouchFingerId == touch.fingerId)
            {
                if (touch.phase == TouchPhase.Stationary)
                {
                    if (lastItemBeginTouchTime + 0.25f < Time.time)
                    {
                        lastItemTouchFingerId = -1;
                        if (TryPickUpItem(touch.position))
                            holdMode = HoldMode.HOLDING;
                    }
                }
                else
                {
                    lastItemTouchFingerId = -1;

                    if (touch.phase == TouchPhase.Ended)
                    {
                        TryUseItem(touch.position, onlyPickable + onlyPlaceable);
                        playerMovement.doMove = false;
                    }
                }
            }
        }

        private void ProcessHoldModeHolding()
        {
            if (lastItemTouchFingerId == -1)
            {
                if (touch.phase == TouchPhase.Began && IsTouchingItem(touch.position, onlyPlaceable))
                {
                    lastItemTouchFingerId = touch.fingerId;
                    lastItemBeginTouchTime = Time.time;
                }
            }
            else if (lastItemTouchFingerId == touch.fingerId)
            {
                if (touch.phase == TouchPhase.Stationary)
                {
                    if (lastItemBeginTouchTime + 0.01f < Time.time)
                    {
                        InitiateBuildMode();
                    }
                }
                else
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        if (Time.time - lastItemEndTouchTime < 0.3f)
                            DropItem();

                        lastItemEndTouchTime = Time.time;
                    // TryUseItem(touch.position);
                    }

                    lastItemTouchFingerId = -1;

                }
            }
        }

        private void ProcessHoldModeBuild()
        {
            if (touch.fingerId == lastItemTouchFingerId)
            {
                BuildModeDragNDrop(touch.position, touch.phase == TouchPhase.Ended);
                if (touch.phase == TouchPhase.Ended)
                {
                    lastItemTouchFingerId = -1;
                }
            }
            else
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    zoom += touch.deltaPosition.y / playerMovement.screenRes.y * 5;
                    zoom = Mathf.Clamp(zoom, 3f, 5f);
                    rotation -= touch.deltaPosition.x / playerMovement.screenRes.x * 360;
                }
            }
        }

        private void FixedUpdate()
        {
            if (holdMode == HoldMode.HOLDING)
            {
                if(tempSpring == null)
                    DropItem();
                else if (itemInHand.transform.localScale.x != 1 && !isLookingAtStorage)
                    itemInHand.transform.localScale = Vector3.Lerp(
                        itemInHand.transform.localScale, Vector3.one, Time.deltaTime * 3f);
            }
            else if (holdMode == HoldMode.BUILD)
            {
                if (!isLookingAtStorage)
                {
                    hand.transform.position = snappedPos;
                    itemInHand.transform.rotation = Quaternion.Lerp(itemInHand.transform.rotation, snappedRot, 0.25f);
                }
            }
        }

        private bool IsTouchingItem(Vector3 touchPosition, int layerMask)
        {
            return Physics.SphereCast(mainCam.ScreenPointToRay(touchPosition), 0.2f, 5, layerMask);
        }

        RaycastHit pickupItemHit;
        private bool TryPickUpItem(Vector3 pressPosition)
        {
            if (Physics.SphereCast(mainCam.ScreenPointToRay(pressPosition), 0.2f, out pickupItemHit, 3, onlyPickable))
            {
                zoom = Mathf.Clamp(pickupItemHit.distance, 1f, pickupItemHit.distance);
                ResetHandPosition();
                PickUpItem(pickupItemHit.transform.gameObject);
                return true;
            }
            return false;
        }

        private void PickUpItem(GameObject item)
        {
            itemInHand = item;
            itemInHand.transform.parent = null;
            rendererMaterialDict = Tools.GetRendererMaterialDict(itemInHand);
            Tools.SetLayerRecursively(itemInHand, LayerMask.NameToLayer("Placeable"));
            tempSpring = hand.AddComponent<SpringJoint>();
            itemInHandRigidbody = itemInHand.GetComponent<Rigidbody>();

            tempSpring.connectedBody = itemInHandRigidbody;
            tempSpring.spring = 200;
            tempSpring.breakForce = 2000;
            tempSpring.anchor = new Vector3(0, 0, 0);
            tempSpring.autoConfigureConnectedAnchor = false;
            tempSpring.connectedAnchor = new Vector3(0, 0, 0);

            itemInHandRigidbody.drag = 10;
            itemInHandRigidbody.isKinematic = false;
            itemInHandRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            itemInHandRigidbody.useGravity = false;

            Tools.SetPhysicMaterialRecursively(itemInHand, noFric);

            if (itemInHand.GetComponent<Item>())
                Tools.SetTriggerRecursively(itemInHand, false);
        }

        private void DropItem()
        {
            holdMode = HoldMode.NONE;

            Destroy(tempSpring);

            if (itemInHandRigidbody)
            {
                itemInHandRigidbody.drag = 0;
                itemInHandRigidbody.constraints = RigidbodyConstraints.None;
                itemInHandRigidbody.useGravity = true;
            }

            Tools.SetPhysicMaterialRecursively(itemInHand, null);
            Tools.SetLayerRecursively(itemInHand, LayerMask.NameToLayer("Pickable"));
            if (rendererMaterialDict != null)
                Tools.SetRendererMaterials(rendererMaterialDict);

            itemInHand = null;
        }

        private RaycastHit tryUseItemHit;
        private InteractionActivator tempActivator;
        private void TryUseItem(Vector3 pressPosition, LayerMask layerMask)
        {
            if (Physics.Raycast(mainCam.ScreenPointToRay(pressPosition), out tryUseItemHit, 5, layerMask))
            {
                tempActivator = tryUseItemHit.transform.GetComponent<InteractionActivator>();
                if(tempActivator) tempActivator.SendMessage("Activate");
            }
        }

        private void ResetHandPosition()
        {
            hand.transform.position = mainCam.transform.position + new Vector3(0, -1f, 0) + mainCam.transform.forward * 3.5f;
        }

        private void InitiateBuildMode()
        {
            if (rendererMaterialDict != null)
                Tools.SetRendererMaterials(rendererMaterialDict.Keys.ToArray(), buildMat);
            holdMode = HoldMode.BUILD;
            itemInHandRigidbody.drag = 20;
            // TODO: init snaps
        }

        private void ExitBuildMode()
        {
            if (rendererMaterialDict != null)
                Tools.SetRendererMaterials(rendererMaterialDict);
            holdMode = HoldMode.HOLDING;
            itemInHandRigidbody.drag = 10;
        }

        private Vector3 snappedPos;
        private Quaternion snappedRot;
        private bool isLookingAtStorage = false;
        bool sameMatUpdateLock = false;
        RaycastHit buildModeHit;

        private void BuildModeDragNDrop(Vector3 pressPosition, bool placeCall)
        {
            bool isPlaced = false;

            // Placing somewhere
            if (Physics.Raycast(mainCam.ScreenPointToRay(pressPosition), out buildModeHit, zoom, buildMask))
            {
                if (sameMatUpdateLock)
                {
                    if (rendererMaterialDict != null)
                        Tools.SetRendererMaterials(rendererMaterialDict.Keys.ToArray(), buildSnapMat);
                    sameMatUpdateLock = !sameMatUpdateLock;
                }

                // Try to place in storage
                if (buildModeHit.transform.GetComponent<Storage>())
                {
                    if (!isLookingAtStorage)
                    {
                        Tools.SetTriggerRecursively(itemInHand, true);
                        isLookingAtStorage = true;
                    }

                    isPlaced = buildModeHit.transform.GetComponent<Storage>().TrySnapItemToSlot(buildModeHit.point, hand.transform, itemInHand.transform, placeCall);

                    if (placeCall && isPlaced)
                    {
                        isLookingAtStorage = false;
                        itemInHandRigidbody.isKinematic = true;
                        DropItem();
                    }
                }
                else
                {
                    if (isLookingAtStorage)
                    {
                        Tools.SetTriggerRecursively(itemInHand,false);
                        isLookingAtStorage = false;
                    }
                }

                // Try to place on ground
                if (!isPlaced && buildModeHit.normal.y > 0.9f && buildModeHit.point.y < 5f)
                {
                    isPlaced = true;
                    snappedPos = new Vector3(Mathf.RoundToInt(buildModeHit.point.x * 2) / 2f, buildModeHit.point.y, Mathf.RoundToInt(buildModeHit.point.z * 2) / 2f);
                    snappedRot = Quaternion.Euler(0, Mathf.RoundToInt(rotation / 45f) * 45, 0);

                    if (placeCall)
                    {
                        // TODO: remove this somehow
                        itemInHand.transform.position = new Vector3(
                            Mathf.RoundToInt(itemInHand.transform.position.x * 2) / 2f,
                            itemInHand.transform.position.y,
                            Mathf.RoundToInt(itemInHand.transform.position.z * 2) / 2f);
                        itemInHand.transform.rotation = snappedRot;

                        isLookingAtStorage = false;
                        itemInHandRigidbody.isKinematic = true;

                        DropItem();
                    }
                }
            }

            // Floating
            if (!isPlaced)
            {
                if (!sameMatUpdateLock)
                {
                    if (rendererMaterialDict != null)
                        Tools.SetRendererMaterials(rendererMaterialDict.Keys.ToArray(), buildMat);
                    sameMatUpdateLock = !sameMatUpdateLock;
                }
                if (isLookingAtStorage)
                {
                    Tools.SetTriggerRecursively(itemInHand,false);
                    isLookingAtStorage = false;
                }

                snappedPos = mainCam.transform.position + mainCam.ScreenPointToRay(pressPosition).direction * zoom;
                snappedRot = Quaternion.Euler(0, rotation, 0);

                if (placeCall)
                    ExitBuildMode();
            }
        }
    }
}
