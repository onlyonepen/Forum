using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMoverManagerScript : MonoBehaviour
{
    [HideInInspector]
    public GameObject TargetObject;
    public float MaxDistance = 2;
    public Camera ARCamera;
    bool IsInMoveState = false;
    public GameObject PainterManager;
    public GameObject PanelPullButton;
    public Slider MoveAwaySlider;

    private float initialDistance;
    private Vector3 initialScale;

    RaycastHit hit;
    Vector3 Point;
    LayerMask NotObject;
    Touch Touch;
    Vector2 TouchPosition;
    quaternion RotationY;
    float RotateSpeedModifier = 0.01f;
    GameObject EmptyObject;

    // Start is called before the first frame update
    void Start()
    {
        TargetObject = GameObject.FindGameObjectWithTag("TargetObject");
        NotObject =~ TargetObject.layer;
        EmptyObject = new GameObject("MovePoint");
        EmptyObject.transform.parent = ARCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (IsInMoveState == true)
        {
            //MoveObjectLogic
            //Ray ray = ARCamera.ScreenPointToRay(new Vector3(-1 / 2, -1 / 2, 0));
            //if (Physics.Raycast(ARCamera.ScreenPointToRay(new Vector3(-1 / 2, -1 / 2, 0))))
            //{
            //    TargetObject.transform.position = hit.point;
            //    Debug.Log("Placed");
            //}
            //else if (!Physics.Raycast(ARCamera.ScreenPointToRay(new Vector3(-1 / 2, -1 / 2, 0))))
            //{
            //    TargetObject.transform.position = (ARCamera.transform.eulerAngles.normalized * MaxDistance) + ARCamera.transform.position;
            //    Debug.Log("Float Place");
            //}
            //Debug.Log(TargetObject.transform.position);
            

            //RotateObjectLogic
            if (Input.touchCount > 0)
            {
                Touch = Input.GetTouch(0);
                Debug.Log("Touch");
                if (Touch.phase == TouchPhase.Moved)
                {
                    RotationY = quaternion.Euler(0f, -Touch.deltaPosition.x * RotateSpeedModifier, 0f);
                    TargetObject.transform.rotation = RotationY * TargetObject.transform.rotation;
                    Debug.Log("Rotate " +  RotationY);
                }
            }

            //ScalingObjectLogic
            if (Input.touchCount == 2)
            {
                var touchZero = Input.GetTouch(0);
                var touchOne = Input.GetTouch(1);

                // if one of the touches Ended or Canceled do nothing
                if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled
                   || touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
                {
                    return;
                }

                // It is enough to check whether one of them began since we
                // already excluded the Ended and Canceled phase in the line before
                if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
                {
                    // track the initial values
                    initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                    initialScale = TargetObject.transform.localScale;
                }
                // else now is any other case where touchZero and/or touchOne are in one of the states
                // of Stationary or Moved
                else
                {
                    // otherwise get the current distance
                    var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                    // A little emergency brake ;)
                    if (Mathf.Approximately(initialDistance, 0)) return;

                    // get the scale factor of the current distance relative to the inital one
                    var factor = currentDistance / initialDistance;

                    // apply the scale
                    // instead of a continuous addition rather always base the 
                    // calculation on the initial and current value only
                    TargetObject.transform.localScale = initialScale * factor;
                }
            }

            //MoveAwayScript
            MaxDistance = MoveAwaySlider.value;
            EmptyObject.transform.localPosition = new Vector3(0 , 0 , MaxDistance);


            TargetObject.transform.position = EmptyObject.transform.position;
        }
    }

    public void IsMovingSwitch()
    {
        IsInMoveState = !IsInMoveState;
        if (IsInMoveState)
        {
            Debug.Log("Able to move around");
            PainterManager.SetActive(false);
            PanelPullButton.GetComponent<Button>().enabled = false;
        }
        if (!IsInMoveState)
        {
            TargetObject.transform.parent = null;
            Debug.Log("Disable Movement");
            PainterManager.SetActive(true);
            PanelPullButton.GetComponent<Button>().enabled = true;
        }
    }

}
