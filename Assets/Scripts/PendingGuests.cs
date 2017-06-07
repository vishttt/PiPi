using System;
using System.Xml.Serialization;
using System.Collections.Generic;


[XmlRoot (ElementName = "PendingGuests")]
public class PendingGuests
{
	[XmlElement (ElementName = "Header")]
	public string Header { get; set; }

	[XmlElement (ElementName = "FacebookId")]
	public List<string> FacebookId { get; set; }
}