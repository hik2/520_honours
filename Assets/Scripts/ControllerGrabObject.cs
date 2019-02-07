using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ControllerGrabObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private GameObject collidingObject;
    private GameObject objectInHand;
    public spawnObjects spawnObjectsClass;

    private GameObject crossTwoDim, crossThreeDim, crossThreeSecDim;
    private float crossTwoDimX, crossTwoDimY, crossTwoDimZ;
    private float crossThreeDimX, crossThreeDimY, crossThreeDimZ;
    private float crossThreeSecDimX, crossThreeSecDimY, crossThreeSecDimZ;
    private float paperX, paperY, paperZ;

    private GameObject screwSmallBot, screwBigBot, nailSmallBot, nailBigBot, paper, screw, nail;
    private bool crossTwoDimColl, crossThreeDimColl, crossThreeSecDimColl;

    public bool colliding;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        crossTwoDim = GameObject.Find("cross2D");
        crossThreeDim = GameObject.Find("cross3D");
        crossThreeSecDim = GameObject.Find("cross3D_2");
        paper = GameObject.Find("paper");

        crossTwoDimX = crossTwoDim.transform.position.x;
        crossTwoDimY = crossTwoDim.transform.position.y;
        crossTwoDimZ = crossTwoDim.transform.position.z;

        crossThreeDimX = crossThreeDim.transform.position.x;
        crossThreeDimY = crossThreeDim.transform.position.y;
        crossThreeDimZ = crossThreeDim.transform.position.z;

        crossThreeSecDimX = crossThreeSecDim.transform.position.x;
        crossThreeSecDimY = crossThreeSecDim.transform.position.y;
        crossThreeSecDimZ = crossThreeSecDim.transform.position.z;

        paperX = paper.transform.position.x;
        paperY = paper.transform.position.y;
        paperZ = paper.transform.position.z;

        screw = GameObject.Find("screwBig");
        screwSmallBot = GameObject.Find("screwBig/screwSmall/screwSmallBottom");
        screwBigBot = GameObject.Find("screwBig/screwBigBottom");
        nail = GameObject.Find("nailBig");
        nailSmallBot = GameObject.Find("nailBig/nailSmall/nailSmallBottom");
        nailBigBot = GameObject.Find("nailBig/nailBigBottom");
    }

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);

        if (colliding == true)
        {
            // If screw/nail enters 2D cross area
            if (other.gameObject == crossTwoDim)
            {
                Debug.Log("In 2D cross area");
                crossTwoDimColl = true;
                crossThreeDimColl = false;
                crossThreeSecDimColl = false;
            }

            // If screw/nail enters 3D cross area
            if (other.gameObject == crossThreeDim)
            {
                Debug.Log("In 3D cross area");
                crossTwoDimColl = false;
                crossThreeDimColl = true;
                crossThreeSecDimColl = false;
            }

            // If screw/nail enters 3D_2 cross area
            if (other.gameObject == crossThreeSecDim)
            {
                Debug.Log("In 3D_2 cross area");
                crossTwoDimColl = false;
                crossThreeDimColl = false;
                crossThreeSecDimColl = true;
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }
        collidingObject = null;

        crossTwoDimColl = false;
        crossThreeDimColl = false;
        crossThreeSecDimColl = false;
    }

    private void GrabObject()
    {
        colliding = true;
        objectInHand = collidingObject;
        collidingObject = null;
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();

        // Hides the controller so it looks like the object picked up replaces the controller
        trackedObj.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        colliding = false;
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            // Write logs to text file log.txt in Assets/Objects folder
            StreamWriter writer = new StreamWriter("Assets/Objects/logs.txt", true);

            // Screw Distances
            if (objectInHand.name == "screwBig")
            {
                // Small screw xyz positions
                float smallScrewBotX = screwSmallBot.transform.position.x;
                float smallScrewBotY = screwSmallBot.transform.position.y;
                float smallScrewBotZ = screwSmallBot.transform.position.z;

                // Big screw xyz positions
                float bigScrewBotX = screwBigBot.transform.position.x;
                float bigScrewBotY = screwBigBot.transform.position.y;
                float bigScrewBotZ = screwBigBot.transform.position.z;

                // Small screw xyz rotations
                float screwRotX = screw.transform.localRotation.x;
                float screwRotY = screw.transform.localRotation.y;
                float screwRotZ = screw.transform.localRotation.z;

                // If screw placed within 2D cross area (left cross)
                if (crossTwoDimColl)
                {
                    // Distance of small screw from 2D cross
                    float smallScrewTwoDimDisX = smallScrewBotX - crossTwoDimX;
                    float smallScrewTwoDimDisY = smallScrewBotY - crossTwoDimY;
                    float smallScrewTwoDimDisZ = smallScrewBotZ - crossTwoDimZ;

                    // Distance of big screw from 2D cross
                    float bigScrewTwoDimDisX = bigScrewBotX - crossTwoDimX;
                    float bigScrewTwoDimDisY = bigScrewBotY - crossTwoDimY;
                    float bigScrewTwoDimDisZ = bigScrewBotZ - crossTwoDimZ;

                    writer.WriteLine("Small screw distance from 2D cross position: x= " + smallScrewTwoDimDisX + ", y= " + smallScrewTwoDimDisY + ", z= " + smallScrewTwoDimDisZ);
                    writer.WriteLine("Big screw distance from 2D cross position: x= " + bigScrewTwoDimDisX + ", y= " + bigScrewTwoDimDisY + ", z=" + bigScrewTwoDimDisZ);
                    writer.WriteLine("Screw local rotation: x= " + screwRotX + ", y= " + screwRotY + ", z= " + screwRotZ);
                }

                // If screw placed within 3D cross area (middle cross)
                if (crossThreeDimColl)
                {
                    // Distance of small screw from 3D cross
                    float smallScrewThreeDimDisX = smallScrewBotX - crossThreeDimX;
                    float smallScrewThreeDimDisY = smallScrewBotY - crossThreeDimY;
                    float smallScrewThreeDimDisZ = smallScrewBotZ - crossThreeDimZ;

                    // Distance of big screw from 3D cross
                    float bigScrewThreeDimDisX = bigScrewBotX - crossThreeDimX;
                    float bigScrewThreeDimDisY = bigScrewBotY - crossThreeDimY;
                    float bigScrewThreeDimDisZ = bigScrewBotZ - crossThreeDimZ;

                    writer.WriteLine("Small screw distance from 3D cross position: x= " + smallScrewThreeDimDisX + ", y= " + smallScrewThreeDimDisY + ", z= " + smallScrewThreeDimDisZ);
                    writer.WriteLine("Big screw distance from 3D cross position: x= " + bigScrewThreeDimDisX + ", y= " + bigScrewThreeDimDisY + ", z= " + bigScrewThreeDimDisZ);
                    writer.WriteLine("Screw local rotation: x= " + screwRotX + ", y= " + screwRotY + ", z= " + screwRotZ);
                }

                // If screw placed within 3D second cross area (right cross)
                if (crossThreeSecDimColl)
                {
                    // Distance of small screw from 3D second cross
                    float smallScrewThreeSecDimDisX = smallScrewBotX - crossThreeSecDimX;
                    float smallScrewThreeSecDimDisY = smallScrewBotY - crossThreeSecDimY;
                    float smallScrewThreeSecDimDisZ = smallScrewBotZ - crossThreeSecDimZ;

                    // Distance of big screw from 3D second cross
                    float bigScrewThreeSecDimDisX = bigScrewBotX - crossThreeSecDimX;
                    float bigScrewThreeSecDimDisY = bigScrewBotY - crossThreeSecDimY;
                    float bigScrewThreeSecDimDisZ = bigScrewBotZ - crossThreeSecDimZ;

                    writer.WriteLine("Small screw distance from 3D_2 cross position: x= " + smallScrewThreeSecDimDisX + ", y= " + smallScrewThreeSecDimDisY + ", z= " + smallScrewThreeSecDimDisZ);
                    writer.WriteLine("Big screw distance from 3D_2 cross position: x= " + bigScrewThreeSecDimDisX + ", y= " + bigScrewThreeSecDimDisY + ", z= " + bigScrewThreeSecDimDisZ);
                    writer.WriteLine("Screw local rotation: x= " + screwRotX + ", y= " + screwRotY + ", z= " + screwRotZ);
                }
                writer.WriteLine("\n");
            }

            // Nail Distances
            if (objectInHand.name == "nailBig")
            {
                // Small nail xyz positions
                float smallNailBotX = nailSmallBot.transform.position.x;
                float smallNailBotY = nailSmallBot.transform.position.y;
                float smallNailBotZ = nailSmallBot.transform.position.z;

                // Small nail xyz rotations
                float nailRotX = nail.transform.localRotation.x;
                float nailRotY = nail.transform.localRotation.y;
                float nailRotZ = nail.transform.localRotation.z;

                // Big nail xyz positions
                float bigNailBotX = nailBigBot.transform.position.x;
                float bigNailBotY = nailBigBot.transform.position.y;
                float bigNailBotZ = nailBigBot.transform.position.z;

                // If nail placed within 2D cross area (left cross)
                if (crossTwoDimColl)
                {
                    // Distance of small nail from 2D cross
                    float smallNailTwoDimDisX = smallNailBotX - crossTwoDimX;
                    float smallNailTwoDimDisY = smallNailBotY - crossTwoDimY;
                    float smallNailTwoDimDisZ = smallNailBotZ - crossTwoDimZ;

                    // Distance of big nail from 2D cross
                    float bigNailTwoDimDisX = bigNailBotX - crossTwoDimX;
                    float bigNailTwoDimDisY = bigNailBotY - crossTwoDimY;
                    float bigNailTwoDimDisZ = bigNailBotZ - crossTwoDimZ;

                    writer.WriteLine("Small nail distance from 2D cross position: x= " + smallNailTwoDimDisX + ", y= " + smallNailTwoDimDisY + ", z= " + smallNailTwoDimDisZ);
                    writer.WriteLine("Big nail distance from 2D cross position: x= " + bigNailTwoDimDisX + ", y= " + bigNailTwoDimDisY + ", z= " + bigNailTwoDimDisZ);
                    writer.WriteLine("Nail local rotation: x= " + nailRotX + ", y= " + nailRotY + ", z= " + nailRotZ);
                }

                // If nail placed within 3D cross area (middle cross)
                if (crossThreeDimColl)
                {
                    // Distance of small nail from 3D cross
                    float smallNailThreeDimDisX = smallNailBotX - crossThreeDimX;
                    float smallNailThreeDimDisY = smallNailBotY - crossThreeDimY;
                    float smallNailThreeDimDisZ = smallNailBotZ - crossThreeDimZ;

                    // Distance of big nail from 3D cross (middle cross)
                    float bigNailThreeDimDisX = bigNailBotX - crossThreeDimX;
                    float bigNailThreeDimDisY = bigNailBotY - crossThreeDimY;
                    float bigNailThreeDimDisZ = bigNailBotZ - crossThreeDimZ;

                    writer.WriteLine("Small nail distance from 3D cross position: x= " + smallNailThreeDimDisX + ", y= " + smallNailThreeDimDisY + ", z= " + smallNailThreeDimDisZ);
                    writer.WriteLine("Big nail distance from 3D cross position: x= " + bigNailThreeDimDisX + ", y= " + bigNailThreeDimDisY + ", z= " + bigNailThreeDimDisZ);
                    writer.WriteLine("Nail local rotation: x= " + nailRotX + ", y= " + nailRotY + ", z= " + nailRotZ);
                }

                // If nail placed within 3D second cross area (right cross)
                if (crossThreeSecDimColl)
                {
                    // Distance of small nail from 3D second cross
                    float smallNailThreeSecDimDisX = smallNailBotX - crossThreeSecDimX;
                    float smallNailThreeSecDimDisY = smallNailBotY - crossThreeSecDimY;
                    float smallNailThreeSecDimDisZ = smallNailBotZ - crossThreeSecDimZ;

                    // Distance of big nail from 3D second cross
                    float bigNailThreeSecDimDisX = bigNailBotX - crossThreeSecDimX;
                    float bigNailThreeSecDimDisY = bigNailBotY - crossThreeSecDimY;
                    float bigNailThreeSecDimDisZ = bigNailBotZ - crossThreeSecDimZ;

                    writer.WriteLine("Small nail distance from 3D_2 cross position: x= " + smallNailThreeSecDimDisX + ", y= " + smallNailThreeSecDimDisY + ", z= " + smallNailThreeSecDimDisZ);
                    writer.WriteLine("Big nail distance from 3D_2 cross position: x= " + bigNailThreeSecDimDisX + ", y= " + bigNailThreeSecDimDisY + ", z= " + bigNailThreeSecDimDisZ);
                    writer.WriteLine("Nail local rotation: x= " + nailRotX + ", y= " + nailRotY + ", z= " + nailRotZ);
                }
                writer.WriteLine("\n");
            }
            writer.Close();
        }
        objectInHand = null;

        // Unhides the controller so user can see controller again
        trackedObj.gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void Update() {
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            Rigidbody collidingBody = collidingObject.GetComponent<Rigidbody>();
            collidingBody.isKinematic = !collidingBody.isKinematic;
        }
    }
}
