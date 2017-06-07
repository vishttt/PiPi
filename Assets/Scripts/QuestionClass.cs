using System;
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot (ElementName = "Question")]
public class Question
{
	[XmlElement (ElementName = "QuestionId")]
	public List<string> QuestionId { get; set; }

	[XmlElement (ElementName = "QuestionDesc")]
	public List<string> QuestionDesc { get; set; }

	[XmlElement (ElementName = "CAnswer")]
	public List<string> CAnswer { get; set; }

	[XmlElement (ElementName = "FAnswer1")]
	public List<string> FAnswer1 { get; set; }

	[XmlElement (ElementName = "FAnswer2")]
	public List<string> FAnswer2 { get; set; }

	[XmlElement (ElementName = "FAnswer3")]
	public List<string> FAnswer3 { get; set; }

	[XmlElement (ElementName = "CategoryId")]
	public List<string> CategoryId { get; set; }

	[XmlElement (ElementName = "Image")]
	public List<string> Image { get; set; }
}

[XmlRoot (ElementName = "MQuestion")]
public class MQuestion
{
	[XmlElement (ElementName = "Sessionid")]
	public string Sessionid { get; set; }

	[XmlElement (ElementName = "ExpiryDate")]
	public string ExpiryDate { get; set; }

	[XmlElement (ElementName = "Question")]
	public Question Question { get; set; }
}