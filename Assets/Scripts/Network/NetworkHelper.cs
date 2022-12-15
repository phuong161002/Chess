using System.Text;
using System.IO;
using Newtonsoft.Json;

public static class NetworkHelper
{
    public static string StreamToString(MemoryStream ms, Encoding encoding)
    {
        string readString = "";
        if (encoding == Encoding.UTF8)
        {
            using (var reader = new StreamReader(ms, encoding))
            {
                readString = reader.ReadToEnd();
            }
        }
        return readString;
    }

    public static T ParseObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static string ParseString<T>(T data)
    {
        return JsonConvert.SerializeObject(data);
    }
}
