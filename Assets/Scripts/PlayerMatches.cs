using System;
using System.Xml.Serialization;
using System.Collections.Generic;


[XmlRoot (ElementName = "Match")]
public class Match
{
	[XmlElement (ElementName = "FacebookSession")]
	public string FacebookSession { get; set; }

	[XmlElement (ElementName = "Expiry")]
	public string Expiry { get; set; }

	[XmlElement (ElementName = "Remainingtime")]
	public string Remainingtime { get; set; }

	[XmlElement (ElementName = "PlaySide")]
	public string PlaySide { get; set; }

	[XmlElement (ElementName = "UserId")]
	public string UserId { get; set; }

	[XmlElement (ElementName = "UserFacebookId")]
	public string UserFacebookId { get; set; }

	[XmlElement (ElementName = "UserName")]
	public string UserName { get; set; }

	[XmlElement (ElementName = "UserScore")]
	public string UserScore { get; set; }

	[XmlElement (ElementName = "UserRank")]
	public string UserRank { get; set; }

	[XmlElement (ElementName = "OpUserId")]
	public string OpUserId { get; set; }

	[XmlElement (ElementName = "OpUserFacebookId")]
	public string OpUserFacebookId { get; set; }

	[XmlElement (ElementName = "OpUserName")]
	public string OpUserName { get; set; }

	[XmlElement (ElementName = "OpUserScore")]
	public string OpUserScore { get; set; }

	[XmlElement (ElementName = "OpUserRank")]
	public string OpUserRank { get; set; }
}

[XmlRoot (ElementName = "PlayerMatches")]
public class PlayerMatches
{
	[XmlElement (ElementName = "Header")]
	public string Header { get; set; }

	[XmlElement (ElementName = "Match")]
	public List<Match> Match { get; set; }
}
