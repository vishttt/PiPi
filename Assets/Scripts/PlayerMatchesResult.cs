using System;
using System.Xml.Serialization;
using System.Collections.Generic;


[XmlRoot (ElementName = "Play")]
public class Play
{
	[XmlElement (ElementName = "FacebookSession")]
	public string FacebookSession { get; set; }

	[XmlElement (ElementName = "HostUserId")]
	public string HostUserId { get; set; }

	[XmlElement (ElementName = "HostFacebookId")]
	public string HostFacebookId { get; set; }

	[XmlElement (ElementName = "HostUsername")]
	public string HostUsername { get; set; }

	[XmlElement (ElementName = "HostUserScoreMulti")]
	public string HostUserScoreMulti { get; set; }

	[XmlElement (ElementName = "HostUserScore")]
	public string HostUserScore { get; set; }

	[XmlElement (ElementName = "HostUserRank")]
	public string HostUserRank { get; set; }

	[XmlElement (ElementName = "GuestUserId")]
	public string GuestUserId { get; set; }

	[XmlElement (ElementName = "GuestFacebookId")]
	public string GuestFacebookId { get; set; }

	[XmlElement (ElementName = "GuestUsername")]
	public string GuestUsername { get; set; }

	[XmlElement (ElementName = "GuestUserScoreMulti")]
	public string GuestUserScoreMulti { get; set; }

	[XmlElement (ElementName = "GuestUserScore")]
	public string GuestUserScore { get; set; }

	[XmlElement (ElementName = "GuestUserRank")]
	public string GuestUserRank { get; set; }

	[XmlElement (ElementName = "MatchResult")]
	public string MatchResult { get; set; }
}

[XmlRoot (ElementName = "PlayerMatches")]
public class PlayerMatchesResult
{
	[XmlElement (ElementName = "Header")]
	public string Header { get; set; }

	[XmlElement (ElementName = "Play")]
	public List<Play> Play { get; set; }
}