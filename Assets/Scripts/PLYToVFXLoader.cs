using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

public class PLYToVFXLoader : MonoBehaviour
{
    public VisualEffect visualEffect;
    public TextAsset plyFile; // 에셋에 넣은 .ply 텍스트 파일

    private GraphicsBuffer buffer;

    void Start()
    {
        var points = LoadAsciiPLY(plyFile.text);
        buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, points.Count, sizeof(float) * 3);
        buffer.SetData(points.ToArray());

        visualEffect.SetInt("pointCount", points.Count);
        visualEffect.SetGraphicsBuffer("PLYFile", buffer);
        
    }

    void OnDestroy()
    {
        if (buffer != null) buffer.Release();
    }

    List<Vector3> LoadAsciiPLY(string content)
    {
        var reader = new StringReader(content);
        string line;
        bool headerEnded = false;
        int vertexCount = 0;
        List<Vector3> result = new List<Vector3>();

        var format = CultureInfo.InvariantCulture.NumberFormat;

        while ((line = reader.ReadLine()) != null)
        {
            if (!headerEnded)
            {
                if (line.StartsWith("element vertex"))
                {
                    var parts = line.Split(' ');
                    vertexCount = int.Parse(parts[2]);
                }

                if (line.StartsWith("end_header"))
                {
                    headerEnded = true;
                }

                continue;
            }

            if (vertexCount-- > 0)
            {
                var tokens = line.Trim().Split(new[] {' ', '\t'}, System.StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length >= 3)
                {
                    Debug.Log($"Parsing line: {line}");
                    Debug.Log($"Tokens: {string.Join(", ", tokens)}");

                    float x = float.Parse(tokens[0], format);
                    float y = float.Parse(tokens[1], format);
                    float z = float.Parse(tokens[2], format);
                    result.Add(new Vector3(x, y, z));
                }
            }
        }

        return result;
    }
}