using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataConversion
{
    /// <summary>
    /// Converts .CSV files formatted with Eastings, Northings, Elevation (x, z, y) to Point[] Chunks.
    /// </summary>
    public static class CSVToPointChunks
    {
        /// <summary>
        /// Converts given data from a string path into a set of Point[].
        /// </summary>
        /// <param name="data">The path of the .csv file.</param>
        /// <param name="chunkSize">The size of each Point[] chunk.</param>
        /// <returns></returns>
        public static async Task<List<Point[]>> ConvertDataToPointChunks(string data, int chunkSize)
        {
            if (data == null)
                throw new NullReferenceException();
            if (chunkSize <= 0)
                throw new ArgumentOutOfRangeException("Must be greater than zero.");

            List<Point[]> chunks = new List<Point[]>();
            Point[] chunk = new Point[chunkSize];

            int count = 0;
            using (StreamReader reader = new StreamReader(data))
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
            return chunks;
        }
        /// <summary>
        /// Converts given data from a string path into a set of Point[].
        /// </summary>
        /// <param name="data">The .csv file when stored in Unity's Asset paths.</param>
        /// <param name="chunkSize">The size of each Point[] chunk.</param>
        /// <returns></returns>
        public static async Task<List<Point[]>> ConvertDataToPointChunks(UnityEngine.Object data, int chunkSize)
        {
            if (data == null)
                throw new NullReferenceException();
            if (chunkSize <= 0)
                throw new ArgumentOutOfRangeException("Must be greater than zero.");

            List<Point[]> chunks = new List<Point[]>();
            ///System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            ///stopwatch.Start();
            Point[] chunk = new Point[chunkSize];

            int count = 0;
            byte[] dataReadable = (data as UnityEngine.TextAsset).bytes;
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

        /// <summary>
        /// Converts given data from a string path into a set of Point[].
        /// </summary>
        /// <param name="data">The .csv file when stored in Unity's Asset paths.</param>
        /// <param name="chunkSize">The size of each Point[] chunk.</param>
        /// <param name="lossIncrement">The amount of lines to skip before reading a line.</param>
        /// <returns></returns>
        public static async Task<Point[]> ConvertDataToPointChunksLossy(UnityEngine.Object data, int lossIncrement)
        {
            if (data == null)
                throw new NullReferenceException();
            if (lossIncrement <= 0)
                throw new ArgumentOutOfRangeException("Must be greater than zero.");

            Point[] chunk = new Point[0];

            int count = 0;
            byte[] dataReadable = (data as UnityEngine.TextAsset).bytes;
            using (StreamReader reader = new StreamReader(new MemoryStream(dataReadable)))
            {
                int lineCount = 0;
                string line = string.Empty;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineCount++;
                }
                chunk = new Point[lineCount / lossIncrement];
                //we want to skip the first line as it is the title line
                await reader.ReadLineAsync();

                //while the stream is open, let's read, format, and convert the data
                Point valuesToVector3;
                line = string.Empty;
                int lossCounter = 0;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lossCounter++;
                    if (lossIncrement % lossCounter != 0)
                        continue;

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
            return chunk;
        }

        /// <summary>
        /// Converts given data from a string path into a set of Point[].
        /// </summary>
        /// <param name="data">The path of the .csv file.</param>
        /// <param name="chunkSize">The size of each Point[] chunk.</param>
        /// <param name="lossIncrement">The amount of lines to skip before reading a line.</param>
        /// <returns></returns>
        public static async Task<List<Point[]>> ConvertDataToPointChunksLossy(string data, int chunkSize, int lossIncrement)
        {
            if (data == null)
                throw new NullReferenceException();
            if (chunkSize <= 0)
                throw new ArgumentOutOfRangeException("Must be greater than zero.");
            if (lossIncrement <= 0)
                throw new ArgumentOutOfRangeException("Must be greater than zero.");

            List<Point[]> chunks = new List<Point[]>();
            Point[] chunk = new Point[chunkSize];

            int count = 0;
            using (StreamReader reader = new StreamReader(data))
            {
                //we want to skip the first line as it is the title line
                await reader.ReadLineAsync();

                //while the stream is open, let's read, format, and convert the data
                Point valuesToVector3;
                string line = string.Empty;
                int lossCounter = 0;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lossCounter++;
                    if (lossIncrement % lossCounter != 0)
                        continue;

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
            return chunks;
        }


        /////THIS ONE WORKED AND IT'S THE BEST (50%+ FASTER)/////
        /// <summary>
        /// Converts given data from a string path into a set of Point[].
        /// </summary>
        /// <param name="data">The .csv file when stored in Unity's Asset paths.</param>
        /// <param name="chunkSize">The size of each Point[] chunk.</param>
        /// <param name="lossIncrement">The amount of lines to skip before reading a line.</param>
        /// <returns></returns>
        public static async Task<List<List<Point>>> ConvertDataToPointChunksLossy(UnityEngine.Object data, int chunkSize, int lossIncrement)
        {
            if (data == null)
                throw new NullReferenceException();
            if (chunkSize <= 0)
                throw new ArgumentOutOfRangeException("Must be greater than zero.");
            if (lossIncrement <= 0)
                throw new ArgumentOutOfRangeException("Must be greater than zero.");

            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();
            List<List<Point>> chunks = new List<List<Point>>();
            List<Point> chunk = new List<Point>();
            //int debugAmountOfFileLines = 0;
            int count = 0;
            byte[] dataReadable = (data as UnityEngine.TextAsset).bytes;
            using (StreamReader reader = new StreamReader(new MemoryStream(dataReadable)))
            {
                //we want to skip the first line as it is the title line
                await reader.ReadLineAsync();

                //while the stream is open, let's read, format, and convert the data
                Point valuesToVector3;
                string line = string.Empty;
                int lossCounter = 0;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    //debugAmountOfFileLines++;
                    lossCounter++;
                    if (lossCounter % lossIncrement != 0)
                        continue;

                    if (count == chunkSize)
                    {
                        chunks.Add(chunk);
                        count = 0;
                        chunk = new List<Point>();
                    }

                    //the way the CSV is formatted is seperation by comma per value, per line
                    string[] values = line.Split(',');

                    //the 0th is x, 2nd is y, 1st is z
                    //this is because the csv is organized:
                    //eastings, northings, elevation
                    //eastings = x, northings = z, elevation = y
                    valuesToVector3 = new Point(values[0], values[2], values[1]);
                    //now that we've encapsulated the values in a Vector3, let's add it to the list we wish to store each value in
                    chunk.Add(valuesToVector3);
                    count++;
                }

                if (chunks.Count < 1)
                    chunks.Add(chunk);
            }
            //UnityEngine.Debug.Log(debugAmountOfFileLines);
            //let's check the last chunk to make sure there isn't any empty space and resize it
            //chunks[chunks.Count - 1] = chunks[chunks.Count - 1].Where(value => !value.Equals(null)).ToArray();

            //currently takes 2 seconds
            //stopwatch.Stop();
            //UnityEngine.Debug.Log($"Transforming Data from CSV to Chunks of Vector3 (Chunk Size: {chunkSize}) -> It Took: {stopwatch.ElapsedMilliseconds / 1000} seconds, {stopwatch.ElapsedMilliseconds} milliseconds");
            //stopwatch.Reset();
            return chunks;
        }
    }
}
