using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class ParseXML : MonoBehaviour
{
	private static string filePath;

	public static void SetFilePath(string fp)
    {
		filePath = fp;
    }

    public static XmlNode getScene(string sceneID)
    {
        XmlDocument storyFile = new XmlDocument();
        storyFile.Load(filePath);

        XmlNode scene = storyFile.SelectSingleNode(string.Format("//scene[@id='{0}']", sceneID));
        Debug.Log(scene.FirstChild.InnerText);
        return scene;
    }
}
