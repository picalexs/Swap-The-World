using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;

public class CsprojFixer : AssetPostprocessor
{
    private static string OnGeneratedCSProject(string path, string content)
    {
        var document = XDocument.Parse(content);
        document.Root.Descendants()
            .Where(x => x.Name.LocalName == "Analyzer")
            .Where(x => x.Attribute("Include").Value.Contains("Unity.SourceGenerators"))
            .Remove();
        return document.Declaration + System.Environment.NewLine + document.Root;
    }
}
