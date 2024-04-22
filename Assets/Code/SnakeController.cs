using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{

    // Settings
    public float MoveSpeed = 5;
    public float SteerSpeed = 180;
    public float BodySpeed = 5;
    public int Gap = 10;

    // References
    public GameObject BodyPrefab;
    public GameObject ApplePrefab;  // Reference to the apple prefab
    public Transform Floor;        // Reference to the floor to define spawn boundaries

    // Lists
    private List<GameObject> BodyParts = new List<GameObject>();
    private List<Vector3> PositionsHistory = new List<Vector3>();

    // Apple Instance
    private GameObject currentApple;

    // Start is called before the first frame update
    void Start()
    {
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        SpawnApple();  // Spawn an apple when the game starts
    }

    // Update is called once per frame
    void Update()
    {

        // Move forward
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;

        // Steer
        float steerDirection = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * steerDirection * SteerSpeed * Time.deltaTime);

        // Store position history
        PositionsHistory.Insert(0, transform.position);

        // Move body parts
        int index = 0;
        foreach (var body in BodyParts)
        {
            Vector3 point = PositionsHistory[Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)];
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * BodySpeed * Time.deltaTime;
            body.transform.LookAt(point);
            index++;
        }

        // Check if the snake eats an apple
        if (currentApple != null && Vector3.Distance(transform.position, currentApple.transform.position) < 1f)
        {
            EatApple();
        }
    }

    private void GrowSnake()
    {
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);
    }

    private void SpawnApple()
    {
        if (currentApple != null)
        {
            Destroy(currentApple);  // Remove the old apple
        }

        Vector3 floorSize = Floor.localScale;  // Assuming the floor's scale is the boundary
        Vector3 spawnPos = new Vector3(
            Random.Range(-floorSize.x / 2, floorSize.x / 2),
            0.5f,  // Half a unit above the floor to avoid z-fighting
            Random.Range(-floorSize.z / 2, floorSize.z / 2)
        ) + Floor.position;

        currentApple = Instantiate(ApplePrefab, spawnPos, Quaternion.identity);
    }

    private void EatApple()
    {
        GrowSnake();
        SpawnApple();
    }
}
