using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Wheels Colliders")] 
    public WheelCollider frontRightWheelCollider;
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider backRightWheelCollider;
    public WheelCollider backLeftWheelCollider;

    [Header("Wheels Transforms")]
    public Transform frontRightWheelTransform;
    public Transform frontLeftWheelTransform;
    public Transform backRightWheelTransform;
    public Transform backLeftWheelTransform;

    [Header("Vehicle Door / Seat")]
    public Transform vehicleDoor;   // Punto donde se teletransporta el jugador al entrar
    public Transform driverSeat;    // Punto donde el jugador se sienta

    [Header("Vehicle Engine")] 
    public float accelerationForce = 1500f; 
    public float brakingForce = 2000f;      
    private float currentBrakeForce = 0f;
    private float currentAcceleration = 0f;

    [Header("Vehicle Steering")] 
    public float wheelsTorque = 30f; 
    private float currentTurnAngle = 0f;

    [Header("Vehicle Security")] 
    public PlayerScript player;  
    public float enterRadius = 5f;
    private bool isDriving = false;

    [Header("Disable Things When Driving")] 
    public GameObject AimCam;
    public GameObject AimCanvas;
    public GameObject ThirdPersonCam;
    public GameObject ThirdPersonCanvas;
    public GameObject PlayerCharacter;

    [Header("Vehicle Hit Var")] 
    public Camera cam;
    public float hitRange = 2f;

    private float giveDamageOf = 100f;
    
    public GameObject goreEffect;
    public GameObject DestroyEffect;
    
    public static ObjectivesComplete Instance; // Singleton moderno


    private Rigidbody rb;

    private void Start()
    {
        // Asegurar Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.mass = 1200f;
        rb.linearDamping = 0.05f; 
        rb.angularDamping = 0.05f;
        rb.useGravity = true;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Si está cerca del carro
        if (!isDriving && distance <= enterRadius && Input.GetKeyDown(KeyCode.F))
        {
            EnterVehicle();
        }
        else if (isDriving && Input.GetKeyDown(KeyCode.G))
        {
            ExitVehicle();
        }

        // Si está conduciendo
        if (isDriving)
        {
            HandleMotor();
            HandleSteering();
            UpdateWheels();
            HitZombies();
        }
    }

    void EnterVehicle()
    {
        isDriving = true;

        // Teletransportar jugador al asiento
        player.transform.position = driverSeat.position;
        player.gameObject.SetActive(false);

        // Desactivar cámaras del jugador
        ThirdPersonCam.SetActive(false);
        ThirdPersonCanvas.SetActive(false);
        AimCam.SetActive(false);
        AimCanvas.SetActive(false);

        // Desactivar modelo del jugador
        PlayerCharacter.SetActive(false);
        
            // ---------- MARCAR OBJETIVO 3 COMO COMPLETADO ----------
            if (ObjectivesComplete.Instance != null)
            {
                ObjectivesComplete.Instance.CompleteObjective(3); // 3 = "Find vehicle"
            }
            else
            {
                Debug.LogWarning("ObjectivesComplete instance not found when entering vehicle.");
            }

        // ------------------------------------------------------
    }

    void ExitVehicle()
    {
        isDriving = false;

        // Mover jugador cerca de la puerta
        player.transform.position = vehicleDoor.position;
        player.gameObject.SetActive(true);

        // Reactivar cámaras del jugador
        ThirdPersonCam.SetActive(true);
        ThirdPersonCanvas.SetActive(true);
        AimCam.SetActive(true);
        AimCanvas.SetActive(true);

        // Reactivar modelo del jugador
        PlayerCharacter.SetActive(true);
    }

    void HandleMotor()
    {
        currentAcceleration = accelerationForce * Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space))
            currentBrakeForce = brakingForce;
        else
            currentBrakeForce = 0f;

        frontRightWheelCollider.motorTorque = currentAcceleration;
        frontLeftWheelCollider.motorTorque = currentAcceleration;
        backRightWheelCollider.motorTorque = currentAcceleration;
        backLeftWheelCollider.motorTorque = currentAcceleration;

        ApplyBraking();
    }

    void ApplyBraking()
    {
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        backRightWheelCollider.brakeTorque = currentBrakeForce;
        backLeftWheelCollider.brakeTorque = currentBrakeForce;
    }

    void HandleSteering()
    {
        currentTurnAngle = wheelsTorque * Input.GetAxis("Horizontal");

        frontRightWheelCollider.steerAngle = currentTurnAngle;
        frontLeftWheelCollider.steerAngle = currentTurnAngle;
    }

    void UpdateWheels()
    {
        UpdateWheelPose(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelPose(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelPose(backRightWheelCollider, backRightWheelTransform);
        UpdateWheelPose(backLeftWheelCollider, backLeftWheelTransform);
    }

    void UpdateWheelPose(WheelCollider collider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        transform.position = position;
        transform.rotation = rotation;
    }

    void HitZombies()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, hitRange))
        {
            Debug.Log(hitInfo.transform.name);
         
            
         
            Zombie1 zombie1 = hitInfo.transform.GetComponent<Zombie1>();
            Zombie2 zombie2 = hitInfo.transform.GetComponent<Zombie2>();
            ObjectToHit objectToHit = hitInfo.transform.GetComponent<ObjectToHit>();
            
        
           
            if (zombie1 != null)
            {
                zombie1.zombieHitDamage(giveDamageOf);
                zombie1.GetComponent<CapsuleCollider>().enabled = false;
                GameObject goreEffectGo = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(goreEffectGo, 1f); 
            }
            else if (zombie2 != null)
            {
                zombie2.zombieHitDamage(giveDamageOf);
                zombie2.GetComponent<CapsuleCollider>().enabled = false;
                GameObject goreEffectGo = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(goreEffectGo, 1f); 
            }

            else if (objectToHit != null)
            {
                objectToHit.ObjectHitDamage(giveDamageOf);
                GameObject WoodGo = Instantiate(DestroyEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(WoodGo, 1f);
            }
            
        }
    }
}
