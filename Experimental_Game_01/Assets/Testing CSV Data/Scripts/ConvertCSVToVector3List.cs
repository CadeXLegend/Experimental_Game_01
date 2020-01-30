using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MyBox;
using System.Linq;
using System.Threading.Tasks;

public struct Point
{
    public float x;
    public float y;
    public float z;

    public Point(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Point(string x, string y, string z)
    {
        this.x = float.Parse(x, System.Globalization.CultureInfo.InvariantCulture);
        this.y = float.Parse(y, System.Globalization.CultureInfo.InvariantCulture);
        this.z = float.Parse(z, System.Globalization.CultureInfo.InvariantCulture);
    }

    public Vector3 AsVector3()
    {
        return new Vector3(x, y, z);
    }
}

public class ConvertCSVToVector3List : MonoBehaviour
{    
    //mark the string as verbatim
    //private readonly string directoryOfFile = @"D:\Desktop\Topo Scan.csv";
    //private readonly string directoryOfFile2 = @"D:\Desktop\Scan_2020_01_29__02_18.csv";
    //private List<BasicVector3[]> chunks = new List<BasicVector3[]>();
    //[SerializeField] private int chunkSize = 4096;
    [SerializeField] private Object data;

    [ButtonMethod]
    public async virtual Task<List<Point[]>> ConvertDataToVector3(int chunkSize)
    {
        List<Point[]> chunks = new List<Point[]>();
        ///System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        ///stopwatch.Start();
        Point[] chunk = new Point[chunkSize];

        int count = 0;
        byte[] dataReadable = (data as TextAsset).bytes;
        using (StreamReader reader = new StreamReader(new MemoryStream(dataReadable)))
        {
            //we want to skip the first line as it is the title line
            await reader.ReadLineAsync();

            //while the stream is open, let's read, format, and convert the data
            Point valuesToVector3;
            string line = string.Empty;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (count == chunkSize)
                {
                    chunks.Add(chunk);
                    count = 0;
                    chunk = new Point[chunkSize];
                }

                //the way the CSV is formatted is seperation by comma per value, per line
                string[] values = line.Split(',');

                //the 0th is x, 2nd is y, 1st is z
                //this is because the csv is organized:
                //eastings, northings, elevation
                //eastings = x, northings = z, elevation = y
                valuesToVector3 = new Point(values[0], values[2], values[1]);
                //now that we've encapsulated the values in a Vector3, let's add it to the list we wish to store each value in
                chunk[count] = valuesToVector3;
                count++;
            }
        }

        //let's check the last chunk to make sure there isn't any empty space and resize it
        chunks[chunks.Count - 1] = chunks[chunks.Count - 1].Where(value => !value.Equals(null)).ToArray();

        //currently takes 2 seconds
        ///stopwatch.Stop();
        ///Debug.Log($"Transforming Data from CSV to Chunks of Vector3 (Chunk Size: {chunkSize}) -> It Took: {stopwatch.ElapsedMilliseconds / 1000} seconds");
        ///stopwatch.Reset();
        return chunks;
    }

    public virtual Point[] GetResolutionAdjustedVertices(List<Point[]> chunks, int resolution)
    {
        List<Point> resolutionAdjustedVertices = new List<Point>();
        for (int i = 0; i < chunks.Count; ++i)
        {
            //generate chunks with the resolution given
            for (int j = 0; j < resolution; ++j)
            {
                resolutionAdjustedVertices.Add(chunks[i][j]);
            }
        }

        return resolutionAdjustedVertices.ToArray();
    }

    /* FINISHED TESTING WITH THIS
    [ButtonMethod]
    //this is just a temp debugging function (to be removed)
    public virtual void DebugChunks()
    {
        foreach (BasicVector3[] chunk in chunks)
        {
            string chunkData = "<b>Chunk:</b>";
            chunkData += "\r\n<color=red><b>{</b></color>\r\n";
            foreach (BasicVector3 value in chunk)
            {
                chunkData += $"\r\n {value}";
            }
            chunkData += "\r\n<color=red><b>}</b></color>\r\n";
            Debug.Log($"{chunkData}\r\n\r\n");
        }
    }
    
    [SerializeField] private int resolutionToGenerate;
    [SerializeField] private Material m;
    GameObject parentRoot;
    [ButtonMethod]
    public virtual void VisualizePoints()
    {
        if (chunks == null)
            return;

        parentRoot = new GameObject("Generated Terrain");

        for (int i = 0; i < chunks.Count; ++i)
        {
            //generate chunks with the resolution given
            for (int j = 0; j < resolutionToGenerate; ++j)
            {
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.position = chunks[i][j];
                //plane.transform.localScale = new Vector3(1f, 1f, 1f);
                plane.transform.parent = parentRoot.transform;
                plane.GetComponent<MeshRenderer>().material = m;
            }
        }

        parentRoot.transform.position = new Vector3(0, 0, 0);
    }

    [ButtonMethod]
    public virtual void CleanVisualizedPoints()
    {
        DestroyImmediate(parentRoot);
    }

    private Mesh mesh;
    [ButtonMethod]
    public virtual void TestTriangulatingPoints()
    {
        List<Vector3> resolutionAdjustedVertices = GetResolutionAdjustedVertices(resolutionToGenerate).ToList();

        int size = resolutionAdjustedVertices.Count * 3 - 6;
        int[] triangles = new int[size];
        for (int i = 0; i < resolutionAdjustedVertices.Count - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        parentRoot = new GameObject("Generated Terrain");
        parentRoot.AddComponent(typeof(MeshFilter));
        parentRoot.AddComponent(typeof(MeshRenderer));

        MeshFilter meshFilter = parentRoot.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = parentRoot.GetComponent<MeshRenderer>();
        mesh = new Mesh
        {
            vertices = resolutionAdjustedVertices.ToArray(),
            triangles = triangles,
        };
        Vector2[] uvs = Unwrapping.GeneratePerTriangleUV(mesh);
        mesh.uv = uvs;
        parentRoot.transform.position = new Vector3(0, 0, 0);
        meshFilter.mesh = mesh;
        meshRenderer.material = m;
    }
    */
}
