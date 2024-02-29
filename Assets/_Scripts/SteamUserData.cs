using System.Collections;
using System.Collections.Generic;
using LOONACIA.Unity.Managers;
using Steamworks;
using UnityEngine;

public static class SteamUserData
{
	private static int _playCount;          // NUM_GAME_PLAYED
	private static int _clearCount;			// NUM_GAME_CLEARED
	public static bool IsWarriorRevived;    // API X
	public static bool IsArcherRevived;     // API X
	public static bool IsAssassinRevived;	// API X
	private static int _crowCount;          // NUM_CROWS
	private static int _oldClearCount;		// NUM_OLD_CLEARED
	private static int _cuteClearCount;     // NUM_CUTE_CLEARED
	public static int DamageMax;			// NUM_MAX_DAMAGE
	private static int _reviveCount;		// NUM_MAX_REVIVE
	public static bool IsWarriorMaxLv;		// API X
	public static bool IsArcherMaxLv;		// API X
	public static bool IsAssassinMaxLv;     // API X
	public static bool IsDamaged;			// API X
	public static bool IsHorsemanRevived;	// API X
	public static bool IsSuccubusRevived;	// API X
	public static bool IsTwoSwordRevived;	// API X
	public static bool IsLichRevived;		// API X
	public static bool IsGolemRevived;      // API X
	public static bool IsDemonRevived;      // API X


	public static void CheckUserStatsGameEnds(bool isCleared)
	{
		if (GameManager.Instance.GetPlayer().IsGodMode) return;

		Steamworks.SteamUserStats.AddStat("NUM_GAME_PLAYED", 1);
		if (isCleared)
		{
			Steamworks.SteamUserStats.AddStat("NUM_GAME_CLEARED", 1);

			if (ManagerRoot.Unit.CurCharacter == UnitManager.CharacterType.Necromancer)
			{
				Steamworks.SteamUserStats.AddStat("NUM_OLD_CLEARED", 1);
			}
			else
			{
				Steamworks.SteamUserStats.AddStat("NUM_CUTE_CLEARED", 1);
			}
		}

		LoadStats();

		GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_FIRST_PLAY"), _playCount, 1);

		if (isCleared)
		{
			GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_FIRST_CLEAR"), _clearCount, 1);

			CheckBothClear();
			CheckNoDamage();
			CheckWarriorOnly();
			CheckArcherOnly();
			CheckAssassinOnly();
			CheckReviveAll();
		}
	}

	public static void ResetAll()
	{
		_playCount = 0;
		_clearCount = 0;
		IsWarriorRevived = false;
		IsArcherRevived = false;
		IsAssassinRevived = false;
		_crowCount = 0;
		_oldClearCount = 0;
		_cuteClearCount = 0;
		DamageMax = 0;
		_reviveCount = 0;
		IsWarriorMaxLv = false;
		IsArcherMaxLv = false;
		IsAssassinMaxLv = false;
		IsDamaged = false;
		IsHorsemanRevived = false;
		IsSuccubusRevived = false;
		IsTwoSwordRevived = false;
		IsLichRevived = false;
		IsGolemRevived = false;
		IsDemonRevived = false;
	}

	public static void LoadStats()
	{
		_playCount = SteamUserStats.GetStatInt("NUM_GAME_PLAYED");
		_clearCount = SteamUserStats.GetStatInt("NUM_GAME_CLEARED");
		_crowCount = SteamUserStats.GetStatInt("NUM_CROWS");
		_oldClearCount = SteamUserStats.GetStatInt("NUM_OLD_CLEARED");
		_cuteClearCount = SteamUserStats.GetStatInt("NUM_CUTE_CLEARED");
	}
	
	private static void CheckWarriorOnly()
	{
		if(IsWarriorRevived && !IsArcherRevived && !IsAssassinRevived)
		{
			GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_WARRIOR_ONLY"), 1, 1);
		}
	}
	
	private static void CheckArcherOnly()
	{
		if (!IsWarriorRevived && IsArcherRevived && !IsAssassinRevived)
		{
			GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_ARCHER_ONLY"), 1, 1);
		}
	}
	
	private static void CheckAssassinOnly()
	{
		if (!IsWarriorRevived && !IsArcherRevived && IsAssassinRevived)
		{
			GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_GHOST_ONLY"), 1, 1);
		}
	}
	
	private static void CheckBothClear()
	{
		if(_oldClearCount >= 10 && _cuteClearCount >= 10)
		{
			GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_BOTH_CLEAR"), 1, 1);
		}
	}
	
	public static void CheckMasterAll()
	{
		if(IsWarriorMaxLv && IsArcherMaxLv && IsAssassinMaxLv)
		{
			GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_MASTER_ALL"), 1, 1);
		}
	}
	
	private static void CheckNoDamage()
	{
		if(!IsDamaged)
		{
			GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_NO_DAMAGED"), 1, 1);
		}
	}
	
	private static void CheckReviveAll()
	{
		if (!IsWarriorRevived || !IsArcherRevived || !IsAssassinRevived || !IsHorsemanRevived || !IsSuccubusRevived || !IsTwoSwordRevived || !IsLichRevived || !IsGolemRevived || !IsDemonRevived) return;

		GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_REVIVE_ALL"), 1, 1);
	}

	public static void CheckCrowNum()
	{
		if (GameManager.Instance.GetPlayer().IsGodMode) return;

		_crowCount = SteamUserStats.GetStatInt("NUM_CROWS");
		_crowCount++;
		SteamUserStats.SetStat("NUM_CROWS", _crowCount);

		GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_CROW_10"), _crowCount, 10);
		GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_CROW_100"), _crowCount, 100);
		GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_CROW_1000"), _crowCount, 1000);
	}
	
	public static void CheckReviveCount()
	{
		if (GameManager.Instance.GetPlayer().IsGodMode) return;

		_reviveCount++;
		var maxReviveCount = SteamUserStats.GetStatInt("NUM_MAX_REVIVE");
		
		if(_reviveCount > maxReviveCount)
		{
			SteamUserStats.SetStat("NUM_MAX_REVIVE", _reviveCount);
			
			GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_REVIVE_ALOT"), _reviveCount, 200);
		}
	}
	
	public static void CheckMaxDamage(int damage_)
	{
		if (GameManager.Instance.GetPlayer().IsGodMode) return;
		if (damage_ == 9999) return;
		
		var maxDamage = SteamUserStats.GetStatInt("NUM_MAX_DAMAGE");
		
		if(damage_ > maxDamage)
		{
			SteamUserStats.SetStat("NUM_MAX_DAMAGE", damage_);

			GameManager.Instance.SteamHandler.AchievementChanged(new Steamworks.Data.Achievement("ACV_HUGE_DAMAGE"), damage_, 5000);
		}
	}
	#region steamworks userstats 연결 전 임시
	public static void AddStat(string statName_, float amount_ = 1f)
	{
		float statFloat = PlayerPrefs.GetFloat(statName_);
		statFloat += amount_;
		SetStat(statName_, statFloat);
	}

	public static void AddStat(string statName_, int amount_ = 1)
	{
		int statInt = PlayerPrefs.GetInt(statName_);
		statInt += amount_;
		SetStat(statName_, statInt);
	}

	private static void SetStat(string name_, float value_)
	{
		PlayerPrefs.SetFloat(name_, value_);
	}

	private static void SetStat(string name_, int value_)
	{
		PlayerPrefs.SetInt(name_, value_);
	}
	#endregion
}
