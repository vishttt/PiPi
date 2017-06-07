using UnityEngine;
using System.Collections;

[System.Serializable]
public class Layout
{
	public LayoutStates layoutName;
	public GameObject layoutGameobject;
}

public enum LayoutStates
{
	_login,
	_selectGame,
	_MultiplayerSelectionPanel,
	_selectCategory,
	_singlePlayer,
	_SinglePlayerWinPanal,
	_MultiPlayerLobbyPanel,
	_MultiPlayerPanel,
	_MultiPlayerWinPanel,
	_MultiPlayerWin2Panel,
	_leaderboardPanel,
	_PlayerStatisticsPanel,
	_SittingsPanel,
	_MultiplayerLoading,
	_loading,
	_FriendsListPanel
}