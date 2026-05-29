using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class ARObjectSpawner : MonoBehaviour
{
    [Header("3D Objects (Prefabs)")]
    public GameObject[] objectPrefabs;

    [Header("Colors")]
    public Color[] availableColors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.white,
        new Color(1f, 0.5f, 0f)
    };

    public Button[] shapeButtons;
    public Button[] colorButtons;

    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private int currentShapeIndex = 0;
    private int currentColorIndex = 0;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Start()
    {
        for (int i = 0; i < shapeButtons.Length; i++)
        {
            int index = i;
            shapeButtons[i].onClick.AddListener(() => SelectShape(index));
        }
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int index = i;
            colorButtons[i].GetComponent<Image>().color = availableColors[i];
            colorButtons[i].onClick.AddListener(() => SelectColor(index));
        }
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    SpawnObject(hits[0].pose.position);
                }
            }
        }
    }

    void SpawnObject(Vector3 position)
    {
        if (objectPrefabs.Length == 0) return;
        GameObject newObject = Instantiate(objectPrefabs[currentShapeIndex], position, Quaternion.identity);
        Renderer renderer = newObject.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = availableColors[currentColorIndex];
        newObject.AddComponent<RotateObject>();
        spawnedObjects.Add(newObject);
    }

    public void SelectShape(int index) { currentShapeIndex = index; }
    public void SelectColor(int index) { currentColorIndex = index; }

    public void ClearAll()
    {
        foreach (GameObject obj in spawnedObjects) Destroy(obj);
        spawnedObjects.Clear();
    }
}

public class RotateObject : MonoBehaviour
{
    public float rotateSpeed = 50f;
    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}