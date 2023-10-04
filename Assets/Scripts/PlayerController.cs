using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Object References")]
    public GameObject ballPrefab;
    [SerializeField]
    private Transform shooter;
    private BallController ballController;
    [SerializeField]
    private TMPro.TextMeshProUGUI powerText;
    public Transform ballFollower;

    [Header("Shot Properties")]
    [SerializeField]
    private Transform shotPoint;
    [SerializeField]
    private float shotLoadSpeed;
    [SerializeField]
    private float shotPowerMax;
    [SerializeField]
    private float shotPowerMin;
    private float shotPower;
    private bool shotReleased = false;
    private bool shotDone = false;
    private int powerValue;
    private float launchPower;
    [SerializeField]
    private float rotationModifier;
    private Vector3 _direction;
    private GameObject spawnedBall;


    [Header("Ball Cam")]
    [SerializeField]
    private Camera ballCam;
    [SerializeField]
    private Vector3 ballCamOffset;







    // Start is called before the first frame update
    void Update()
    {
       
            if (!shotReleased)
            {
                if (Input.GetMouseButton(0))
                {

                    if (shotPower < shotPowerMin)
                    {
                        shotPower = shotPowerMin;
                    }
                    shotPower += (Time.deltaTime * shotLoadSpeed);
                    if (shotPower >= shotPowerMax)
                    {
                        shotPower = shotPowerMax;

                    }
                powerValue = Mathf.RoundToInt((Mathf.InverseLerp(shotPowerMin, shotPowerMax, shotPower)) * 100);
                powerText.text = powerValue.ToString();
                launchPower = shotPower;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    ReleaseShot();

                }
            }

        if (shotDone)
        {

            ballCam.transform.position = new Vector3(spawnedBall.transform.position.x + ballCamOffset.x, Mathf.Clamp(spawnedBall.transform.position.y + ballCamOffset.y, 0.2f, 100f), spawnedBall.transform.position.z + ballCamOffset.z);
            ballCam.transform.LookAt(spawnedBall.transform.position);

        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            shotDone = false;
            shotReleased = false;
            ballCam.gameObject.SetActive(false);
        }

        if(shotReleased && shotDone && !ballController.isGrounded())
        {
            
            spawnedBall.transform.Rotate(Vector3.up * launchPower * rotationModifier * Time.deltaTime);
        }


    }
        




    
    void ReleaseShot()
    {
        shotReleased = true;
        Vector3 shotPosition = Input.mousePosition;
        shotPosition.z = 10;
        _direction = Camera.main.ScreenToWorldPoint(shotPosition);

        spawnedBall = Instantiate(ballPrefab);
        ballController = spawnedBall.GetComponent<BallController>();
        spawnedBall.transform.position = shotPoint.transform.position;

        shooter.transform.position = shotPoint.position;
        shooter.transform.LookAt(_direction);
        ballCam.gameObject.SetActive(true);
    }

    void Shoot()
    {

        Rigidbody ballRb = spawnedBall.GetComponent<Rigidbody>();
        ballRb.AddForce(shooter.transform.forward * shotPower, ForceMode.Impulse);
        shotDone = true;
        shotPower = 0;
    }
    private void FixedUpdate()
    {
        if(shotReleased && !shotDone)
        {
            Shoot();
        }
    }
}
