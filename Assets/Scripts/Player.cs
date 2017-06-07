using System;
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot (ElementName = "Player")]
public class Player
{
	[XmlElement (ElementName = "Header")]
	public string Header { get; set; }

	[XmlElement (ElementName = "PlayerID")]
	public string PlayerID { get; set; }

	[XmlElement (ElementName = "PlayerName")]
	public string PlayerName { get; set; }

	[XmlElement (ElementName = "TotalScore")]
	public string TotalScore { get; set; }

	[XmlElement (ElementName = "WrongAnswersNbr")]
	public string WrongAnswersNbr { get; set; }

	[XmlElement (ElementName = "CorrectAnswersNbr")]
	public string CorrectAnswersNbr { get; set; }

	[XmlElement (ElementName = "TotalWonPlays")]
	public string TotalWonPlays { get; set; }

	[XmlElement (ElementName = "TotalLostPlays")]
	public string TotalLostPlays { get; set; }

	[XmlElement (ElementName = "UserRank")]
	public string UserRank { get; set; }

	[XmlElement (ElementName = "FbId")]
	public string FbId { get; set; }

	[XmlElement (ElementName = "image")]
	public string Image { get; set; }
}

[XmlRoot (ElementName = "Leaderboard")]
public class Leaderboard
{
	[XmlElement (ElementName = "Header")]
	public string Header { get; set; }

	[XmlElement (ElementName = "PlayerId")]
	public List<string> PlayerId { get; set; }

	[XmlElement (ElementName = "PlayerRank")]
	public List<string> PlayerRank { get; set; }

	[XmlElement (ElementName = "PlayerName")]
	public List<string> PlayerName { get; set; }

	[XmlElement (ElementName = "TotalScore")]
	public List<string> TotalScore { get; set; }
}

[XmlRoot (ElementName = "playerStatistics")]
public class PlayerStatistics
{
	[XmlElement (ElementName = "Header")]
	public string Header { get; set; }

	[XmlElement (ElementName = "PlayerID")]
	public string PlayerID { get; set; }

	[XmlElement (ElementName = "PlayerName")]
	public string PlayerName { get; set; }

	[XmlElement (ElementName = "TotalScore")]
	public string TotalScore { get; set; }

	[XmlElement (ElementName = "WrongAnswersNbr")]
	public string WrongAnswersNbr { get; set; }

	[XmlElement (ElementName = "CorrectAnswersNbr")]
	public string CorrectAnswersNbr { get; set; }

	[XmlElement (ElementName = "TotalWonPlays")]
	public string TotalWonPlays { get; set; }

	[XmlElement (ElementName = "TotalLostPlays")]
	public string TotalLostPlays { get; set; }

	[XmlElement (ElementName = "UserRank")]
	public string UserRank { get; set; }

	[XmlElement (ElementName = "image")]
	public string Image { get; set; }
}