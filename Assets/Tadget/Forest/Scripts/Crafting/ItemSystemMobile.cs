namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ItemSystemMobile : MonoBehaviour
    {

        public PhysicMaterial noFric;
        public Material buildMat;
        public Material buildSnapMat;
        private Material[] savedMatsOfItem;

        public LayerMask buildMask;
        public LayerMask onlyPickable;
        public LayerMask onlyPlacable;

        public GameObject itemInHand;
        public GameObject hand;
        public GameObject myBackpack;

        private Camera mainCam;
        private SpringJoint tempSpring;
        private Rigidbody tempRigid;
        private PlayerMovement playerMovement;

        private int holdMode = 0; // 0 = none, 1 = holding, 2 = action mode (removed), 3 = build mode;
        private float tapTime;

        private Vector2 touchZoomBegan;

        private float zoom;
        private float rotation;

        private int itemTouchId = -1;

        private Animator anim;

        private void Start()
        {
            mainCam = Camera.main;
            playerMovement = GetComponent<PlayerMovement>();
            anim = myBackpack.GetComponent<Animator>();
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
                            /* if (tapTime + 0.3f > Time.time)
                             {
                                 // Debug.Log("Mode: 0 - double tap - use item");
                                 UseItemFunction(touch.position);
                                 playerMovement.doMove = false;
                             }*/

                            if (IsPressedOnItem(touch.position))
                            {
                                // Debug.Log("Mode: 0 - touch began");
                                tapTime = Time.time;
                                itemTouchId = touch.fingerId;
                                break;
                            }
                        }
                    }
                    if (touch.fingerId == itemTouchId)
                    {
                        if (touch.phase == TouchPhase.Ended)
                        {
                            UseItemFunction(touch.position);
                            playerMovement.doMove = false;
                            // Debug.Log("Mode: 0 - tapped - use item");
                        }
                        if (touch.phase == TouchPhase.Stationary)
                        {
                            if (Time.time > tapTime + 0.25f)
                            {
                                if (IsPressedOnItem(touch.position))
                                {
                                    // Debug.Log("Mode: 0 - pick up item");
                                    itemTouchId = -1;
                                    TryPickUpItem(touch.position);
                                }
                            }
                        }
                        else
                        {
                            // Debug.Log("Mode: 0 - pick up failed" + " mode: " + touch.phase);
                            itemTouchId = -1;
                        }
                    }
                } // not picked up
                else if (holdMode == 1) // holding mode
                {
                    if (itemTouchId == -1 || itemTouchId == touch.fingerId)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            if (IsPressedOnItemInHand(touch.position))
                            {
                                itemTouchId = touch.fingerId;
                                /* if (tapTime + 0.3f > Time.time) //if double tap
                                 {
                                     // Debug.Log("Mode: 1 - double tapped, to action mode");
                                     itemTouchId = -1;
                                     UseItemFunction(touch.position);
                                 }*/
                                tapTime = Time.time;
                            }
                            else
                            {
                                // Debug.Log("Mode: 1 - missed double tapped");
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
                                // Debug.Log("Mode: 1 - long press, to build mode");
                                InitiateBuildMode();
                            }
                        }
                        if (touch.phase == TouchPhase.Ended)
                        {

                            if (!IsPressedOnItemInHand(touch.position))
                            {
                                // Debug.Log("Mode: 1 - short swipe, drop item");
                                itemTouchId = -1;
                                DropItem();
                                itemInHand = null;
                            }
                            else if (tapTime + 0.3f > Time.time)
                            {
                                itemTouchId = -1;
                                UseItemFunction(touch.position);
                            }
                        }
                    }
                } // holding mode
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
                            zoom += touch.deltaPosition.y / playerMovement.screenRes.y * 5;
                            zoom = Mathf.Clamp(zoom, 3f, 5f);
                            rotation -= touch.deltaPosition.x / playerMovement.screenRes.x * 360;
                        }
                    }
                } // build mode
            }
            if (itemTouchId != -1)
            {
                playerMovement.doMove = false;
            }
            else
            {
                playerMovement.doMove = true;
            }

        }
        private Vector3 myHold1;
        private void FixedUpdate()
        {
            if (tempSpring == null && itemInHand != null)
            {
                DropItem();
                itemInHand = null;
                // Debug.Log("broke");
            }
            if (holdMode == 1)
            {
                //hand.transform.localPosition =  myHold1;
                itemInHand.transform.rotation = Quaternion.Lerp(itemInHand.transform.rotation,
                    Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0), 0.25f);
                // tempSpring.anchor = transform.transform.InverseTransformPoint(MainCam.transform.position + MainCam.transform.forward * zoom);
            }
            /*else if (holdMode == 2) // lol removed
            {
                hand.transform.position = mainCam.transform.position + new Vector3(0, -1f, 0) +
                                          mainCam.transform.forward * 2 + transform.right * 1.5f;
                itemInHand.transform.rotation = Quaternion.Lerp(itemInHand.transform.rotation,
                    Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0), 0.25f);
            }*/
            else if (holdMode == 3 && !isLookingAtStorage)
            {
                hand.transform.position = snappedPos;
                itemInHand.transform.rotation = Quaternion.Lerp(itemInHand.transform.rotation, snappedRot, 0.25f);
            }
            if (itemInHand != null)
            {
                if (itemInHand.transform.localScale.x != 1 && !isLookingAtStorage)
                {
                    itemInHand.transform.localScale = Vector3.Lerp(itemInHand.transform.localScale, Vector3.one, 0.2f);
                }
                NudgeItem();
            }
        }
        private bool IsPressedOnItem(Vector3 pressPosition)
        {
            return Physics.SphereCast(mainCam.ScreenPointToRay(pressPosition), 0.2f, 5, onlyPickable);
            // return Physics.Raycast(mainCam.ScreenPointToRay(pressPosition), 5, onlyPickable); // layer 9 = pickable
        }
        private bool IsPressedOnItemInHand(Vector3 pressPosition)
        {
            RaycastHit hit;
            //if (Physics.Raycast(mainCam.ScreenPointToRay(pressPosition), out hit, 5, onlyPlacable))
            if (Physics.SphereCast(mainCam.ScreenPointToRay(pressPosition), 0.2f, out hit, 5, onlyPlacable))
            {
                if (itemInHand == null)
                {
                    return false;
                }

                return hit.transform.gameObject == itemInHand ? true : false;
            }

            return false;
        }

        private void TryPickUpItem(Vector3 pressPosition)
        {
            RaycastHit hit;
            //if (Physics.Raycast(mainCam.ScreenPointToRay(pressPosition), out hit, 5, onlyPickable))
            if (Physics.SphereCast(mainCam.ScreenPointToRay(pressPosition), 0.2f, out hit, 5, onlyPickable))
            {
                zoom = Mathf.Clamp(hit.distance, 3f, 5f);
                holdMode = 1;
                itemInHand = hit.transform.gameObject;
                RemoveSavedItem(itemInHand);
                savedMatsOfItem = ChangeMaterialsOfItemHierarchy(true);
                PickUpItem();
                myHold1 = mainCam.transform.position + new Vector3(0, -1f, 0) + mainCam.transform.forward * 3.5f;
                hand.transform.position = myHold1;

            }
        }

        private void PickUpItem()
        {
            if (myBackpack == itemInHand)
            {
                myBackpack.GetComponent<Animator>().enabled = false;
                myBackpack = null;
            }
            //ChangeMaterialOfItemHierarchy(pickedUpMat);
            itemInHand.transform.parent = null;
            ChangeLayerOfItemHierarchy(LayerMask.NameToLayer("Placeable"));
            tempSpring = hand.AddComponent<SpringJoint>();
            tempRigid = itemInHand.GetComponent<Rigidbody>();

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

            if (itemInHand.GetComponent<Item>())
            {
                ChangeTriggerOfItemHierarchy(false);
            }
        }

        private Vector3 snappedPos;
        private Quaternion snappedRot;
        private bool isLookingAtStorage = false;
        bool sameMatUpdateLock = false;

        private void BuildModeDragNDrop(Vector3 pressPosition, bool placeCall)
        {

            bool isNotPlacable = true; // fix
            RaycastHit hit;
            if (Physics.Raycast(mainCam.ScreenPointToRay(pressPosition), out hit, zoom, buildMask))
            {
                isNotPlacable = false;
                if (sameMatUpdateLock)
                {
                    ChangeMaterialOfItemHierarchy(buildSnapMat);
                    sameMatUpdateLock = !sameMatUpdateLock;
                }

                //try place in backpack
                bool notPlacingInStorage = true; // false if placed in backpack
                if (hit.transform.GetComponent<Storage>() && itemInHand.GetComponent<Item>()) // place on storage
                {
                    if (!isLookingAtStorage)
                    {
                        ChangeTriggerOfItemHierarchy(true);
                        isLookingAtStorage = true;
                    }

                    notPlacingInStorage = hit.transform.GetComponent<Storage>().TrySnapItemToSlot(hit.point, hand.transform, itemInHand.transform, placeCall);
                    isNotPlacable = notPlacingInStorage;
                    if (placeCall && !notPlacingInStorage)
                    {
                        isLookingAtStorage = false;
                        tempRigid.isKinematic = true;
                        DropItem();
                        itemInHand = null;
                        SaveItem(hit.transform.gameObject);
                    }
                }
                //====

                //try place on ground
                if (notPlacingInStorage) // place on ground
                {
                    // if in bounds
                    if (hit.normal.y > 0.9f && hit.point.y < 5f) // if in bounds
                    {
                        isNotPlacable = false;
                        if (isLookingAtStorage)
                        {
                            ChangeTriggerOfItemHierarchy(false);
                            isLookingAtStorage = false;
                        }

                        snappedPos = new Vector3(Mathf.RoundToInt(hit.point.x * 2) / 2f, hit.point.y, Mathf.RoundToInt(hit.point.z * 2) / 2f);

                        snappedRot = Quaternion.Euler(0, Mathf.RoundToInt(rotation / 45f) * 45, 0);

                        //tempSpring.anchor = transform.InverseTransformPoint(snappedPos);
                        if (placeCall)
                        {        
                            itemInHand.transform.position = new Vector3(
                                Mathf.RoundToInt(itemInHand.transform.position.x * 2) / 2f,
                                itemInHand.transform.position.y,
                                Mathf.RoundToInt(itemInHand.transform.position.z * 2) / 2f);
                            itemInHand.transform.rotation = snappedRot;


                            isLookingAtStorage = false;
                            tempRigid.isKinematic = true;
                            
                            DropItem();
                            SaveItem(itemInHand);
                            itemInHand = null;
                        }
                    }
                    else
                    {
                        isNotPlacable = true;
                    }
                }
                //==========

                //==============
            } // is placing somewhere

            if (isNotPlacable)// fix
            {
                if (!sameMatUpdateLock)
                {
                    ChangeMaterialOfItemHierarchy(buildMat);
                    sameMatUpdateLock = !sameMatUpdateLock;
                }
                if (isLookingAtStorage)
                {
                    ChangeTriggerOfItemHierarchy(false);
                    isLookingAtStorage = false;
                }

                snappedPos = mainCam.transform.position + mainCam.ScreenPointToRay(pressPosition).direction * zoom;
                myHold1 = snappedPos;
                hand.transform.position = myHold1;
                // Debug.Log(zoom);
                snappedRot = Quaternion.Euler(0, rotation, 0);

                //tempSpring.anchor = transform.InverseTransformPoint(snappedPos);
                if (placeCall)
                {
                    // Debug.Log("RESET");
                    ResetItem();
                }
            } // is floating 
        }

        private void InitiateBuildMode()
        {
            ChangeMaterialOfItemHierarchy(buildMat);
            rotation = mainCam.transform.eulerAngles.y + 180;
            holdMode = 3;
            tempRigid.drag = 20;
        }

        private void DropItem()
        {
            holdMode = 0;

            Destroy(tempSpring);
            tempRigid = itemInHand.GetComponent<Rigidbody>();

            tempRigid.drag = 0;
            tempRigid.constraints = RigidbodyConstraints.None;
            tempRigid.useGravity = true;
            NudgeItem();
            ChangeColliderOfItemHierarchy(false);

            ChangeLayerOfItemHierarchy(LayerMask.NameToLayer("Pickable"));
            ChangeMaterialsOfItemHierarchy(false);

            
        }

        private void ResetItem()
        {
            GameObject tempItem = itemInHand;

            DropItem();

            itemInHand = tempItem;
            PickUpItem();
            holdMode = 1;
        }

        private void NudgeItem()
        {
            tempRigid.AddForce(0, 0.05f, 0);
        }

        private void ChangeLayerOfItemHierarchy(int newLayer)
        {
            List<Transform> children = new List<Transform>();
            children.Add(itemInHand.transform);
            while (children.Count != 0)
            {
                Transform child = children[0];
                children.RemoveAt(0);
                for (int i = 0; i < child.childCount; i++)
                {
                    children.Add(child.GetChild(i));
                }

                child.gameObject.layer = newLayer;
            }
        }

        private Material[] ChangeMaterialsOfItemHierarchy(bool readTwriteF)
        {
            int b = 0;
            List<Material> mats = new List<Material>();

            List<Transform> children = new List<Transform>();
            children.Add(itemInHand.transform);
            while (children.Count != 0)
            {
                Transform child = children[0];
                children.RemoveAt(0);
                for (int i = 0; i < child.childCount; i++)
                {
                    children.Add(child.GetChild(i));
                }
                tempRend = child.GetComponent<Renderer>();

                if (tempRend != null)
                {
                    if (readTwriteF)
                    {
                        for (int i = 0; i < tempRend.materials.Length; i++)
                        {
                            mats.Add(tempRend.materials[i]);
                        }
                        //mats.Add(child.GetComponent<Renderer>().material);
                    }
                    else
                    {
                        tempmats = new Material[tempRend.materials.Length];
                        for (int i = 0; i < tempRend.materials.Length; i++)
                        {
                            tempmats[i] = savedMatsOfItem[b];
                            //tempRend.materials[i] = savedMatsOfItem[b];
                            b++;
                        }
                        tempRend.materials = tempmats;
                    }
                }
            }
            return mats.ToArray();
        }

        private Renderer tempRend;
        private Material[] tempmats;

        private Material ChangeMaterialOfItemHierarchy(Material mat)
        {
            List<Transform> children = new List<Transform>();
            children.Add(itemInHand.transform);
            while (children.Count != 0)
            {
                Transform child = children[0];
                children.RemoveAt(0);
                for (int i = 0; i < child.childCount; i++)
                {
                    children.Add(child.GetChild(i));
                }

                tempRend = child.GetComponent<Renderer>();
                if (tempRend != null)
                {
                    tempmats = new Material[tempRend.materials.Length];
                    for (int i = 0; i < tempRend.materials.Length; i++)
                    {
                        tempmats[i] = mat;
                        //Debug.Log( "change mat" + i);
                        //tempRend.materials = mat; // neveikia
                    }
                    tempRend.materials = tempmats; // veikia
                }
            }
            return null;
        }

        private void ChangeColliderOfItemHierarchy(bool isNoFriction)
        {
            List<Transform> children = new List<Transform>();
            children.Add(itemInHand.transform);
            while (children.Count != 0)
            {
                Transform child = children[0];
                children.RemoveAt(0);
                for (int i = 0; i < child.childCount; i++)
                {
                    children.Add(child.GetChild(i));
                }

                if (child.GetComponent<Collider>())
                {
                    if (isNoFriction)
                    {
                        child.GetComponent<Collider>().material = noFric;
                    }
                    else
                    {
                        child.GetComponent<Collider>().material = null;
                    }
                }
            }
        }

        private void ChangeTriggerOfItemHierarchy(bool isTrigger)
        {
            List<Transform> children = new List<Transform>();
            children.Add(itemInHand.transform);
            while (children.Count != 0)
            {
                Transform child = children[0];
                children.RemoveAt(0);
                for (int i = 0; i < child.childCount; i++)
                {
                    children.Add(child.GetChild(i));
                }

                if (child.GetComponent<Collider>())
                {
                    child.GetComponent<Collider>().isTrigger = isTrigger;
                }
            }
        }

        private void UseItemFunction(Vector3 pressPosition)
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCam.ScreenPointToRay(pressPosition), out hit, 5, onlyPickable + onlyPlacable))
            {
                if (hit.transform.GetComponent<Crafting>())
                {
                    hit.transform.GetComponent<Crafting>().Check4Craft();
                    SaveItem(hit.transform.gameObject);
                    SaveItem(hit.transform.gameObject);
                }
            }
        }

        public void BackpackCall()
        {

            if (anim.GetBool("isOpen"))
            {
                anim.SetBool("isOpen", false);
                // anim.Play("Backpack_toFront",0,Mathf.NegativeInfinity);
            }
            else
            {
                anim.SetBool("isOpen", true);
                //anim.Play("Backpack_toFront", 0, 0);
            }
            //anim.Play("Backpack_toFront", -1, 0);

        }

        // other stuff

        private void SaveItem(GameObject Item)
        {
            Debug.Log("saved");
            Debug.Log(Item.name);
        }

        GameObject findStorage;
        private void RemoveSavedItem(GameObject Item)
        {
            Debug.Log("Removed");
            Debug.Log(Item.name);

            if(Item.transform.parent != null)
            {
                findStorage = Item.transform.parent.gameObject;
                while (!findStorage.GetComponent<Storage>())
                {
                    if (findStorage.transform.parent == null) break;

                    findStorage = findStorage.transform.parent.gameObject; 
                }
                if(findStorage.GetComponent<Storage>())
                {
                    Item = findStorage;
                }
            } // finds storage device

            Debug.Log(Item.name);
        }

    }
}
