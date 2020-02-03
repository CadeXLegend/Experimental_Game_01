namespace DataConversion
{
    /// <summary>
    /// A simple 3D Co-ordinate Structure with built-in Vector3 Conversion.
    /// </summary>
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

        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(x, y, z);
        }
    }
}
