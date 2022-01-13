using UnityEngine;

public class Movement : MonoBehaviour
{
    private bool isPortalUsable=true, isStarted, isInPortal;
    [SerializeField]private GameObject playerMesh;
    [SerializeField]private GameObject portalIn, portalOut, portalInPrewiev;
    private Vector3 camOffset, portalInOffset, portalOutOffset;
    [SerializeField]private Transform cam, finishCollider;
    [SerializeField]private float energyForPortal, portalSpeed;
    [SerializeField]private GameObject UI;
    [SerializeField] private ParticleSystem inExplosion, outExplosion;

    private void Start()
    {
        portalInOffset = portalIn.transform.position - playerMesh.transform.position;
        portalOutOffset = portalOut.transform.position - playerMesh.transform.position;
        camOffset = cam.position - playerMesh.transform.position;
    }
    
    private void Update()
    {
        outExplosion.gameObject.transform.position = portalOut.transform.position;
        if (energyForPortal >= 100)
        {
            energyForPortal = 100;
        }
        if (energyForPortal <= 0)
        {
            isPortalUsable = false;
            energyForPortal = 0;
        }
        if (energyForPortal > 0 && !UI.GetComponent<gameUI>().m_isDead)
        {
            isPortalUsable = true;
        }
        UI.GetComponent<gameUI>().energySlider.value = energyForPortal;
    }
    
    private void FixedUpdate()
    {
        cam.transform.position = playerMesh.transform.position + camOffset;
        EditorMovementInput();
        MobileMovement();
    }

    private void MobileMovement()
    {
        Touch touch;
        if(Input.touchCount>0)
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && isPortalUsable && !isInPortal)
            {
                PortalMovingDown();
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                UsingPortal();
            }
        }
        else if(Input.touchCount <= 0)
        {
            energyForPortal += 0.2f ; 
        }
    }
    void EditorMovementInput()
    {
        if (Input.GetKey(KeyCode.Mouse0) && isPortalUsable && !isInPortal)
        {
            PortalMovingDown();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            UsingPortal();
        }
        
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            energyForPortal += 0.2f ;
        }
    }
    private void PortalMovingDown()
    {
        portalIn.GetComponent<Collider>().enabled = false;
        portalOut.GetComponent<Collider>().enabled = false;
        
        portalOut.SetActive(true);
        if (energyForPortal > 0 && portalOut.transform.position.y > finishCollider.position.y)
        {
            portalOut.transform.Translate(Vector3.down * portalSpeed * Time.fixedDeltaTime, Space.World);
            energyForPortal--; 
        }
    }

    private void UsingPortal()
    {
        portalIn.GetComponent<Collider>().enabled = true;
        portalOut.GetComponent<Collider>().enabled = true;
        
        isPortalUsable = false;
        portalIn.SetActive(true);
        portalInPrewiev.SetActive(false);
        portalOut.transform.SetParent(null);
        portalIn.transform.SetParent(null);
    }

    private void OnTriggerEnter(Collider collisionInfo)
    {
        switch (collisionInfo.tag)
        {
            case "Portal":
                isInPortal = true;
                isPortalUsable = false;
                playerMesh.SetActive(false);
                inExplosion.Play();
                Physics.gravity = new Vector3(0,-98.1f*1.4f, 0);
                GetComponent<Rigidbody>().mass += 100;
                break;
            
            case "PortalOut":
                GetComponent<Rigidbody>().mass -= 100;
                outExplosion.Play();
                Physics.gravity = new Vector3(0,-9.812f*2,0);
                playerMesh.SetActive(true);
                portalOut.SetActive(false);
                portalIn.SetActive(false);
                portalInPrewiev.SetActive(true);
                isInPortal = false;
                isPortalUsable = true;
                portalOut.transform.SetParent(gameObject.transform);
                portalIn.transform.SetParent(gameObject.transform);
                portalOut.transform.position = playerMesh.transform.position + portalOutOffset;
                portalIn.transform.position = playerMesh.transform.position + portalInOffset;
                break;
            
            case "Block":
                if (!isInPortal)
                {
                    GetComponent<Animator>().Play("Fail");
                    UI.GetComponent<gameUI>().m_isDead = true;
                    isPortalUsable = false;
                    collisionInfo.transform.GetChild(0).gameObject.SetActive(true);
                    portalInPrewiev.SetActive(false);
                    portalOut.SetActive(false);
                    portalIn.SetActive(false);
                }
                if (isInPortal) UI.GetComponent<gameUI>().m_isDead = false;
                //GetComponent<Rigidbody>().useGravity = false;
                break;
            
            case "Finish":
                UI.GetComponent<gameUI>().m_isFinished = true;
                isPortalUsable = false;
                break;
            
            default:
                break;
        }
    }
}