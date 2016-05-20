using System;
using System.Collections;
using System.Collections.Generic;
namespace CollectionsApplication
{

class DictionaryProgram
{
    static void Main(string[] args)
    {
        Dictionary<int, string> dt = new Dictionary<int, string>();
        dt.Add(1, "One");
        dt.Add(2, "Two");
        dt.Add(1, "Three");
        foreach (KeyValuePair<int, String> kv in dt)
        {
            Console.WriteLine(kv.Key + " " + kv.Value);
        }
    }
}

}