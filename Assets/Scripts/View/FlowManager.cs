using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    [SerializeField] private GameObject[] graphicTemplates;
    [SerializeField] private Color[] lineColors;
    [SerializeField] private Transform[] spawnTransforms;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private float lineLength = 0.5f;
    [SerializeField] private GameObject winMessage;
    [SerializeField] private GameObject loseMessage;
    [SerializeField] private float gameTime = 60f;
    [SerializeField] private GameObject _gameScene;
    [SerializeField] private TextMeshProUGUI[] timerText;
    [SerializeField] private AudioClip lineSegmentSound;
    [SerializeField] private AudioSource audioSource;
    
    private List<GameObject> spawnedPoints = new List<GameObject>();
    private Dictionary<int, List<Vector2>> lines = new Dictionary<int, List<Vector2>>();
    private HashSet<GameObject> connectedPairs = new HashSet<GameObject>();
    private LineRenderer currentLineRenderer;
    private Vector2 lastPoint;
    private bool isDrawingLine;
    private int currentPairIndex = -1;
    private GameObject startingPoint;
    private float timer;
    private bool gameEnded;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = lineSegmentSound;
        
        timer = gameTime;
        SpawnPoints(10); 
        winMessage.SetActive(false);
        loseMessage.SetActive(false);
        SetLineLengthBasedOnScreenSize();
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        foreach (var textTimer in timerText)
        {
            textTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
    }
    
    private void SetLineLengthBasedOnScreenSize()
    {
        int gridWidth = Screen.width;
        int verticalSpan = Screen.height;
        
        if ((gridWidth == 1440 && verticalSpan == 2560) || (gridWidth == 1080 && verticalSpan == 1920) || (gridWidth == 720 && verticalSpan == 1280))
        {
            lineLength = 0.85f;
        }
        else if (gridWidth == 1440 && verticalSpan == 2960)
        {
            lineLength = 0.78f;
        }
        else if (gridWidth == 1080 && verticalSpan == 2160)
        {
            lineLength = 0.8f;
        }
        else
        {
            lineLength = 0.76f;
        }
    }
    
    private void Update()
    {
        UpdateTimerUI();
        if (gameEnded) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            LoseGame();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                if (spawnedPoints.Contains(hitObject) && !connectedPairs.Contains(hitObject))
                {
                    StartDrawingLine(hitObject);
                }
            }
        }

        if (isDrawingLine && Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            UpdateLine(mousePos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDrawingLine();
        }
    }

    private void SpawnPoints(int numberOfPoints)
    {
        if (numberOfPoints % 2 != 0 || spawnTransforms.Length < numberOfPoints)
        {
            return;
        }

        string[] spawnVariations = new string[]
        {
            "000011204030000000240003550000",
            "100203000000100340425000500000",
            "100002310400005034000250000000",
            "120020300000040040501500300000",
            "000010230410030500205400000000",
            "102340100050000000005234000000",
            "100203004001002300040500500000",
            "150050000120003404002000300000",
            "120000003210000435000004500000"
        };

        
        string variation = spawnVariations[Random.Range(0, spawnVariations.Length)];
        List<int> usedIndices = new List<int>();
        spawnedPoints.Clear();

        for (int tierIdentifier = 0; tierIdentifier < variation.Length; tierIdentifier++)
        {
            int prefabIndex = variation[tierIdentifier] - '0';

            if (prefabIndex == 0)
                continue;
            
            Vector2Int pointPos = new Vector2Int(tierIdentifier % 5, tierIdentifier / 5);

            if (!usedIndices.Contains(pointPos.x + pointPos.y * 5))
            {
                GameObject pointObj = Instantiate(graphicTemplates[prefabIndex - 1], spawnTransforms[pointPos.x + pointPos.y * 5].position, Quaternion.identity, this.transform);
                pointObj.transform.localScale = new Vector3(0.169f, 0.145f, 1f);
                usedIndices.Add(pointPos.x + pointPos.y * 5);
                spawnedPoints.Add(pointObj);
            }
        }
    }

    private void StartDrawingLine(GameObject point)
    {
        if (connectedPairs.Contains(point)) return;
        
        int prefabIndex = -1;
        for (int indexData = 0; indexData < graphicTemplates.Length; indexData++)
        {
            if (point.CompareTag("Point" + indexData))
            {
                prefabIndex = indexData;
                break;
            }
        }

        if (prefabIndex == -1)
        {
            return;
        }

        currentPairIndex = prefabIndex;
        startingPoint = point;
        
        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(point.transform);
        currentLineRenderer = lineObject.AddComponent<LineRenderer>();

        currentLineRenderer.positionCount = 2;
        currentLineRenderer.SetPosition(0, point.transform.position);
        currentLineRenderer.SetPosition(1, point.transform.position);
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        Color opaqueColor = lineColors[currentPairIndex]; 
        opaqueColor.a = 1f;
        currentLineRenderer.startColor = opaqueColor;
        currentLineRenderer.endColor = opaqueColor;

        lines[currentPairIndex] = new List<Vector2> { point.transform.position };
        lastPoint = point.transform.position;
        isDrawingLine = true;
    }

    

    private void UpdateLine(Vector2 newPoint)
{
    if (currentLineRenderer == null) return;

    Vector2 adjustedPoint = AdjustLinePosition(newPoint);
    currentLineRenderer.startWidth = lineWidth;
    currentLineRenderer.endWidth = lineWidth;

    if (IsLineInvalid(adjustedPoint))
    {
        DestroyLine();
        return;
    }

    RaycastHit2D hit = Physics2D.Raycast(adjustedPoint, Vector2.zero);
    if (hit.collider != null)
    {
        GameObject hitObject = hit.collider.gameObject;

        if (hitObject == startingPoint) return;

        if (!hitObject.CompareTag(startingPoint.tag))
        {
            DestroyLine();
            return;
        }

        if (connectedPairs.Contains(hitObject))
        {
            DestroyLine();
            return;
        }

        int hitPairIndex = -1;
        for (int pairIndex = 0; pairIndex < graphicTemplates.Length; pairIndex++)
        {
            if (hitObject.CompareTag("Point" + pairIndex))
            {
                hitPairIndex = pairIndex;
                break;
            }
        }

        if (hitPairIndex == currentPairIndex)
        {
            if (Vector2.Distance(lines[currentPairIndex][lines[currentPairIndex].Count - 1], adjustedPoint) <= lineLength)
            {
                Vector2 squareEdgePoint = hitObject.transform.position;
                lastPoint = squareEdgePoint;
                currentLineRenderer.positionCount++;
                currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, squareEdgePoint);
                lines[currentPairIndex].Add(squareEdgePoint);
                connectedPairs.Add(startingPoint);
                connectedPairs.Add(hitObject);
                PlayLineSegmentSound();
                CheckForVictory();
                isDrawingLine = false;
                StopDrawingLine();
                return;
            }
            else
            {
                DestroyLine();
                return;
            }
        }
        else
        {
            DestroyLine();
            return;
        }
    }

    float distanceToLastPoint = Vector2.Distance(lastPoint, adjustedPoint);
    if (distanceToLastPoint >= lineLength)
    {
        Vector2 direction = (adjustedPoint - lastPoint).normalized;
        Vector2 newLinePoint = lastPoint + direction * lineLength;

        lastPoint = newLinePoint;
        currentLineRenderer.positionCount++;
        currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, newLinePoint);
        lines[currentPairIndex].Add(newLinePoint);

        PlayLineSegmentSound();
    }

    if (currentLineRenderer.positionCount > 2 && IsLineInvalid(lastPoint))
    {
        DestroyLine();
    }
}

    private void PlayLineSegmentSound()
{
    if (audioSource != null && lineSegmentSound != null)
    {
        audioSource.PlayOneShot(lineSegmentSound);
    }
}


    private void CheckForVictory()
    {
        if (connectedPairs.Count == spawnedPoints.Count)
        {
            AchieveVictory();
        }
    }

    private Vector2 AdjustLinePosition(Vector2 newPoint)
    {
        Vector2 direction = newPoint - lastPoint;
        Vector2 adjustedPoint;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            adjustedPoint = new Vector2(newPoint.x, lastPoint.y);
        }
        else
        {
            adjustedPoint = new Vector2(lastPoint.x, newPoint.y);
        }

        foreach (Transform spawnPoint in spawnTransforms)
        {
            if (Vector2.Distance(adjustedPoint, spawnPoint.position) <= lineLength)
            {
                return adjustedPoint;
            }
        }

        return lastPoint;
    }

    private bool IsLineInvalid(Vector2 newPoint)
    {
        foreach (var line in lines.Values)
        {
            if (line == lines[currentPairIndex])
                continue;

            for (int indexArray = 1; indexArray < line.Count; indexArray++)
            {
                if (LineSegmentsIntersect(line[indexArray - 1], line[indexArray], lastPoint, newPoint))
                {
                    return true;
                }
            }
        }

        return false;
    }


    private bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
        if (denominator == 0)
        {
            return false;
        }

        float ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
        float ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

        return (ua >= 0 && ua <= 1) && (ub >= 0 && ub <= 1);
    }

    private void StopDrawingLine()
    {
        if (isDrawingLine)
        {
            isDrawingLine = false;
            DestroyLine();
        }
    }

    private void DestroyLine()
    {
        if (currentLineRenderer != null)
        {
            Destroy(currentLineRenderer.gameObject);
            currentLineRenderer = null;
        }

        if (currentPairIndex >= 0 && lines.ContainsKey(currentPairIndex))
        {
            lines[currentPairIndex].Clear();
        }
    }

    private void AchieveVictory()
    {
        gameEnded = true;
        winMessage.SetActive(true);
        _gameScene.SetActive(false);
        Time.timeScale = 0f;
    }

    private void LoseGame()
    {
        Time.timeScale = 0f;
        gameEnded = true;
        loseMessage.SetActive(true);
        _gameScene.SetActive(false);
    }
}