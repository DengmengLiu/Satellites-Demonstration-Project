using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LowOrbitSatelliteSpawner : MonoBehaviour
{
    public GameObject satellitePrefab; // 卫星的预制体
    public int orbitalPlanes = 10; // 轨道平面的数量
    public int satellitesPerPlane = 10; // 每个轨道平面的卫星数量
    public float orbitRadius = 1300f; // 轨道半径
    public float orbitalInclination = 53f; // 轨道倾角
    public float polarReductionFactor = 0.1f; // 极地区域的卫星数量减少因子
    public float equatorialIncreaseFactor = 1.5f; // 赤道区域的卫星数量增加因子
    public Material hLineMaterial; // 用于通信链路的材质
    public Material vLineMaterial; // 用于通信链路的材质
    public float verticalLinkDelay = 5f; // 纵向连接的时间间隔（单位：秒）
    public float lineWidth = 5f; // 通信链路的宽度
    public float signalSpeed = 1f; // 信号传输速度
    

    private List<List<GameObject>> satelliteGrid = new List<List<GameObject>>(); // 用于保存卫星的二维列表
    private List<LineRenderer> communicationLines = new List<LineRenderer>();

    void Start()
    {
        Vector3 centerPoint = transform.position;
        float inclinationStep = (2 * orbitalInclination) / (orbitalPlanes - 1);

        // 创建卫星并保存
        for (int plane = 0; plane < orbitalPlanes; plane++)
        {
            float planeInclination = -orbitalInclination + inclinationStep * plane;
            float inclinationRad = Mathf.Deg2Rad * planeInclination;

            // 根据倾角调整卫星数量
            int adjustedSatellitesPerPlane = GetAdjustedSatellitesPerPlane(planeInclination);

            List<GameObject> planeSatellites = new List<GameObject>();

            // 预计算角度列表
            List<float> precomputedAngles = new List<float>(adjustedSatellitesPerPlane);
            for (int i = 0; i < adjustedSatellitesPerPlane; i++)
            {
                float angle = (360f / adjustedSatellitesPerPlane) * i;
                precomputedAngles.Add(angle);
            }

            for (int i = 0; i < adjustedSatellitesPerPlane; i++)
            {
                float angleRad = Mathf.Deg2Rad * precomputedAngles[i];

                Vector3 position = new Vector3(
                    orbitRadius * Mathf.Cos(angleRad) * Mathf.Cos(inclinationRad),
                    orbitRadius * Mathf.Sin(inclinationRad),
                    orbitRadius * Mathf.Sin(angleRad) * Mathf.Cos(inclinationRad)
                );

                GameObject satellite = SpawnSatellite(centerPoint, position);
                planeSatellites.Add(satellite);
            }

            satelliteGrid.Add(planeSatellites);
        }

        // 创建通信链路
        CreateCommunicationLinks();
    }

    int GetAdjustedSatellitesPerPlane(float planeInclination)
    {
        int adjustedSatellitesPerPlane = satellitesPerPlane;
        if (Mathf.Abs(planeInclination) > 45f) // 极地区域
        {
            adjustedSatellitesPerPlane = Mathf.CeilToInt(satellitesPerPlane * polarReductionFactor);
        }
        else if (Mathf.Abs(planeInclination) < 15f) // 赤道区域
        {
            adjustedSatellitesPerPlane = Mathf.CeilToInt(satellitesPerPlane * equatorialIncreaseFactor);
        }
        return adjustedSatellitesPerPlane;
    }

    GameObject SpawnSatellite(Vector3 center, Vector3 position)
    {
        GameObject satellite = Instantiate(satellitePrefab, center + position, Quaternion.identity);
        satellite.transform.SetParent(transform);
        satellite.transform.LookAt(center);
        return satellite;
    }

    void CreateCommunicationLinks()
    {
        // 先创建横向连接
        CreateHorizontalLinks();

        // 在指定延迟后创建纵向连接
        StartCoroutine(CreateVerticalLinksWithDelay(verticalLinkDelay));
    }

    void CreateHorizontalLinks()
    {
        for (int plane = 0; plane < satelliteGrid.Count; plane++)
        {
            List<GameObject> currentPlaneSatellites = satelliteGrid[plane];
            int planeCount = currentPlaneSatellites.Count;

            for (int i = 0; i < planeCount; i++)
            {
                GameObject satelliteA = currentPlaneSatellites[i];
                if (satelliteA == null) continue;

                // 连接到同一轨道平面的下一个卫星（形成环状）
                GameObject satelliteB = currentPlaneSatellites[(i + 1) % planeCount];
                if (satelliteB != null)
                {
                    CreateLineWithDelay(satelliteA, satelliteB, hLineMaterial, "HorizontalCommunicationLine");
                }
            }
        }
    }

    IEnumerator CreateVerticalLinksWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int plane = 0; plane < satelliteGrid.Count - 1; plane++)
        {
            List<GameObject> currentPlaneSatellites = satelliteGrid[plane];
            List<GameObject> nextPlaneSatellites = satelliteGrid[plane + 1];
            int planeCount = Mathf.Min(currentPlaneSatellites.Count, nextPlaneSatellites.Count);

            for (int i = 0; i < planeCount; i++)
            {
                GameObject satelliteA = currentPlaneSatellites[i];
                GameObject satelliteB = nextPlaneSatellites[i];

                if (satelliteA != null && satelliteB != null)
                {
                    CreateLineWithDelay(satelliteA, satelliteB, vLineMaterial, "VerticalCommunicationLine");
                }
            }
        }
    }

    void CreateLineWithDelay(GameObject satelliteA, GameObject satelliteB, Material material, string lineName)
    {
        if (satelliteA == null || satelliteB == null)
            return;

        GameObject lineObject = new GameObject(lineName);
        lineObject.transform.SetParent(transform);
        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.material = new Material(material); // 为每条线创建独立材质实例
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 2;
        line.SetPosition(0, satelliteA.transform.position);
        line.SetPosition(1, satelliteA.transform.position); // 起始时起点和终点重合
        communicationLines.Add(line);
        StartCoroutine(ExtendLine(line, satelliteA.transform, satelliteB.transform));
    }

    IEnumerator ExtendLine(LineRenderer line, Transform satelliteA, Transform satelliteB)
    {
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * signalSpeed;
            line.SetPosition(0, satelliteA.position);
            line.SetPosition(1, Vector3.Lerp(satelliteA.position, satelliteB.position, progress));
            yield return null;
        }

        // After fully extending, continue updating to follow the satellites' positions
        while (true)
        {
            line.SetPosition(0, satelliteA.position);
            line.SetPosition(1, satelliteB.position);
            yield return null;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 开始同时从所有卫星发出信号
            foreach (LineRenderer line in communicationLines)
            {
                Vector3 startPosition = line.GetPosition(0);
                Vector3 endPosition = line.GetPosition(1);
                //StartCoroutine(ExtendLine(line, startPosition, endPosition));
            }
        }
    }
}
