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

namespace Xml2CSharp
{
	[XmlRoot(ElementName = "decision")]
	public class Decision
	{
		[XmlElement(ElementName = "id")]
		public string Id { get; set; }
		[XmlElement(ElementName = "text")]
		public string Text { get; set; }
		[XmlElement(ElementName = "destination")]
		public string Destination { get; set; }
	}

	[XmlRoot(ElementName = "scene")]
	public class Scene
	{
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "text")]
		public string Text { get; set; }
		[XmlElement(ElementName = "decision")]
		public Decision Decision { get; set; }
	}

	[XmlRoot(ElementName = "story")]
	public class Story
	{
		[XmlElement(ElementName = "scene")]
		public List<Scene> Scene { get; set; }
	}

}
