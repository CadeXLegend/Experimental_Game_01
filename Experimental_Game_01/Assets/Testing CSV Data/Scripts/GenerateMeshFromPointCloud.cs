using System.Collections.Generic;
using UnityEngine;
using TriangleNet.Meshing;
using TriangleNet.Geometry;
using TriangleNet.Topology;
using MyBox;
using System.Linq;
using DataConversion;
using System.Threading.Tasks;

public class GenerateMeshFromPointCloud : MonoBehaviour
{
    [SerializeField] private Object csvDataForMesh;
    private Vector3[] points;
    public enum PointState { FIXED, FREE };
    private PointState[] pointState;
    private Color[] colors;
    private Vector3[] normals;
    private int[] triangles;
    [SerializeField] private MeshFilter targetMesh;
    //low: chunkSize = 5000, resOfVerticeData = 1
    //medium: chunkSize = 1024, resOfVerticeData = 1
    //high: chunkSize = 80, resOfVerticeData = 1
    //very high: chunkSize = 32, resOfVerticeData = 1
    public enum MapDetail { Low, Medium, High, VeryHigh };
    public MapDetail detailOfMap = MapDetail.Medium;
    private readonly int[] detailPairs = {5000, 1024, 80, 32 };
    [SerializeField] private Gradient gradient;


    int detailCycleCounter = -1;
    private void TestThisKeys()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            detailCycleCounter = detailCycleCounter < 3 ? detailCycleCounter + 1 : 0;
            detailOfMap = (MapDetail)detailCycleCounter;
            BuildMeshFromData();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            detailOfMap = MapDetail.Low;
            BuildMeshFromData();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            detailOfMap = MapDetail.Medium;
            BuildMeshFromData();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            detailOfMap = MapDetail.High;
            BuildMeshFromData();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            detailOfMap = MapDetail.VeryHigh;
            BuildMeshFromData();
            return;
        }
    }

    private void Update()
    {
        TestThisKeys();
    }

    List<List<DataConversion.Point>> chunks1;
    List<List<DataConversion.Point>> chunks2;
    List<List<DataConversion.Point>> chunks3;
    List<List<DataConversion.Point>> chunks4;
    [ButtonMethod]
    public async virtual void BuildMeshFromData()
    {
        //convert the csv supplied to Point chunks
        if(chunks1 == null)
            chunks1 = await CSVToPointChunks.ConvertDataToPointChunksLossy(csvDataForMesh, detailPairs[0], detailPairs[0]);
        if (chunks2 == null)
            chunks2 = await CSVToPointChunks.ConvertDataToPointChunksLossy(csvDataForMesh, detailPairs[1], detailPairs[1]);
        if (chunks3 == null)
            chunks3 = await CSVToPointChunks.ConvertDataToPointChunksLossy(csvDataForMesh, detailPairs[2], detailPairs[2]);
        if (chunks4 == null)
            chunks4 = await CSVToPointChunks.ConvertDataToPointChunksLossy(csvDataForMesh, detailPairs[3], detailPairs[3]);
        //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //stopwatch.Start();
        if(detailOfMap == MapDetail.Low)
            points = chunks1.SelectMany(val => val.Select(v => v.ToVector3()).ToArray()).Cast<Vector3>().ToArray();
        if (detailOfMap == MapDetail.Medium)
            points = chunks2.SelectMany(val => val.Select(v => v.ToVector3()).ToArray()).Cast<Vector3>().ToArray();
        if (detailOfMap == MapDetail.High)
            points = chunks3.SelectMany(val => val.Select(v => v.ToVector3()).ToArray()).Cast<Vector3>().ToArray();
        if (detailOfMap == MapDetail.VeryHigh)
            points = chunks4.SelectMany(val => val.Select(v => v.ToVector3()).ToArray()).Cast<Vector3>().ToArray();

        pointState = new PointState[points.Length];
        for (int i = 0; i < points.Length; ++i)
        {
            pointState[i] = PointState.FIXED;
        }

        if (targetMesh.sharedMesh.isReadable)
        {
            targetMesh.sharedMesh.Clear();
            targetMesh.sharedMesh.MarkDynamic();
            CreateMesh();
            targetMesh.sharedMesh.MarkModified();
        }
        else
            Debug.Log("Make sure to make the mesh Read/Write!");
        //stopwatch.Stop();
        //Debug.Log($"Transforming Data from CSV to Terrain -> It Took: {stopwatch.ElapsedMilliseconds / 1000} seconds, {stopwatch.ElapsedMilliseconds} milliseconds");
        //stopwatch.Restart();
    }

    List<Vector3> newPoints;
    Vector3[] temp;
    PointState[] tempState;
    //Adds Centroids to the middle of the triangles and adds them to other separate array of vector3s
    private void addCentroids()
    {
        newPoints = new List<Vector3>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 other = points[triangles[i]];
            Vector3 b = points[triangles[i + 1]];
            Vector3 c = points[triangles[i + 2]];
            Vector3 centroid = (other + b + c) / 3;
            //the y value for the centroids is initially going to be an avg of the vertices of other triangle
            //centroid.y = 0;
            newPoints.Add(centroid);
        }
        //Creates new variable newLength to hold the variable length of the arrays and more centroids are added so that propper memory can be allocated
        int newLength = points.Length + newPoints.Count;
        temp = new Vector3[newLength];
        tempState = new PointState[newLength];
        for (int i = 0; i < points.Length; i++)
        {
            temp[i] = points[i];
            tempState[i] = pointState[i];
        }
        for (int i = 0; i < newPoints.Count; i++)
        {
            temp[i + points.Length] = newPoints[i];
            tempState[i + points.Length] = PointState.FREE;
        }
        points = temp;
        pointState = tempState;
    }

    List<Vertex> vertices;
    ICollection<Triangle> tri;
    GenericMesher gm;
    IMesh imesh;
    private void CreateMesh()
    {
        colors = new Color[points.Length];
        normals = new Vector3[points.Length];

        float minYValue = points.Min(v => v.y);
        float maxYValue = points.Max(v => v.y);
        for (int i = 0; i < points.Length; i++)
        {
            //colors[i] = new Color(cscale, 0, 1 - cscale);
            float height = Mathf.InverseLerp(minYValue, maxYValue, points[i].y);
            colors[i] = gradient.Evaluate(height);
            normals[i] = new Vector3(0, 1, 0);
        }

        if(gm == null)
            gm = new GenericMesher();

        vertices = new List<Vertex>();
        for (int i = 0; i < points.Length; i++)
        {
            Vertex v = new Vertex(points[i].x, points[i].z);
            vertices.Add(v);
        }

        imesh = gm.Triangulate(vertices);
        tri = imesh.Triangles;
        int ntri = tri.Count;
        triangles = new int[3 * ntri];
        int ctri = 0;
        foreach (Triangle triangle in tri)
        {
            triangles[ctri] = triangle.GetVertexID(2);
            triangles[ctri + 1] = triangle.GetVertexID(1);
            triangles[ctri + 2] = triangle.GetVertexID(0);
            ctri += 3;
        }

        targetMesh.sharedMesh.vertices = points;
        targetMesh.sharedMesh.triangles = triangles;
        targetMesh.sharedMesh.normals = normals;
        targetMesh.sharedMesh.colors = colors;
        ///ignoring normals for now, don't really need them atm
        //mesh.RecalculateNormals();
        ///if you need collision, un-comment RecalculateBounds();
        //mesh.RecalculateBounds();
    }

    List<int>[] nearby;
    float[] storage;
    private void Smooth(int ntrials, bool newCounts, bool smoothFixed)
    {
        storage = new float[points.Length];
        if (newCounts) nearby = new List<int>[points.Length];
        for (int iter = 0; iter < ntrials; iter++)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (iter == 0 && newCounts)
                    nearby[i] = getNearby(i, 5.0f * Mathf.Sqrt(1.0f / points.Length));
                if (pointState[i] == PointState.FREE || smoothFixed)
                {
                    int count = nearby[i].Count;
                    if (count > 0)
                    {
                        float sum = 0.0f;
                        for (int j = 0; j < nearby[i].Count; j++)
                        {
                            sum += points[nearby[i][j]].y;
                        }
                        sum /= (float)count;
                        storage[i] = sum;
                    }
                }
            }
            for (int i = 0; i < points.Length; i++)
            {
                if (pointState[i] == PointState.FREE || smoothFixed)
                {
                    points[i].y = storage[i];
                }
            }
        }
    }

    public List<int> getNearby(int base1, float radius)
    {
        List<int> returnvalues;
        returnvalues = new List<int>();

        bool foundNeighbors = false;
        int iter = 0;
        int itmax = 10;
        while (!foundNeighbors && iter++ < itmax)
        {
            returnvalues = new List<int>(); //reiterates the list
            float rsquare = radius * radius; //Creating variable rsquare will eliminate the need to run the multiplication each time through the for loop--> better speeds
                                             //To come up with an accurate average Y value, we need to test every vertex with every other vertex and make sure that the distance fits within a given radius.
                                             //But, if there are not enough data points, the approximation for the new Y-value of the centroid will not be accurate. j
                                             //Therefore, w.e need to create a system that will increase both the original vertex and each other vertex by one, making sure to never overlap.
            for (int other = 0; other < points.Length; other++) //increasing the comparison vertex number in the array by 1
            {
                if (other != base1) // We do not want to subtract the original point by itself bc that will just yield 0 and sway the average
                {
                    Vector3 delta = points[other] - points[base1];
                    if (delta.sqrMagnitude <= rsquare) //Checking to see if the vertex is within the radius
                    {
                        returnvalues.Add(other); //Adds the ="Other" Vector3 to the list returnValues
                    }
                }
            }
            if (returnvalues.Count < 3)
            {
                radius *= 2.0f; //exponentially increases the radius
            }
            else if (returnvalues.Count > 10)
            {
                radius *= 0.4f; //exponentially increases the radius
            }
            else

            {
                foundNeighbors = true; //ends while loop
            }
        }
        if (!foundNeighbors)
        {

        }
        return returnvalues;
    }
}
