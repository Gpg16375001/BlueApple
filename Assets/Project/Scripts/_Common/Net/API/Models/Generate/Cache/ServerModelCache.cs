using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SmileLab.Net.API;

public static partial class ServerModelCache
{
	static partial void CreateTables() 
	{
		CardData.CreateTable();
		UserData.CreateTable();
		WeaponData.CreateTable();
		MagikiteData.CreateTable();
		ConsumerData.CreateTable();
		FormationData.CreateTable();
		MaterialData.CreateTable();
		QuestAchievement.CreateTable();
	}

	static public void CacheSet(this IEnumerable<CardData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Save (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<CardData>>();
			}
		);
	}

	static public void CacheSet(this CardData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Save (con, trans);
				ServerModelCache.Modify<IEnumerable<CardData>>();
			}
		);
	}
	static public void CacheDelete(this IEnumerable<CardData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Delete (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<CardData>>();
			}
		);
	}

	static public void CacheDelete(this CardData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Delete (con, trans);
				ServerModelCache.Modify<IEnumerable<CardData>>();
			}
		);
	}

	static public void CacheSet(this IEnumerable<UserData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Save (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<UserData>>();
			}
		);
	}

	static public void CacheSet(this UserData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Save (con, trans);
				ServerModelCache.Modify<IEnumerable<UserData>>();
			}
		);
	}
	static public void CacheDelete(this IEnumerable<UserData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Delete (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<UserData>>();
			}
		);
	}

	static public void CacheDelete(this UserData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Delete (con, trans);
				ServerModelCache.Modify<IEnumerable<UserData>>();
			}
		);
	}

	static public void CacheSet(this IEnumerable<WeaponData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Save (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<WeaponData>>();
			}
		);
	}

	static public void CacheSet(this WeaponData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Save (con, trans);
				ServerModelCache.Modify<IEnumerable<WeaponData>>();
			}
		);
	}
	static public void CacheDelete(this IEnumerable<WeaponData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Delete (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<WeaponData>>();
			}
		);
	}

	static public void CacheDelete(this WeaponData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Delete (con, trans);
				ServerModelCache.Modify<IEnumerable<WeaponData>>();
			}
		);
	}

	static public void CacheSet(this IEnumerable<MagikiteData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Save (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<MagikiteData>>();
			}
		);
	}

	static public void CacheSet(this MagikiteData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Save (con, trans);
				ServerModelCache.Modify<IEnumerable<MagikiteData>>();
			}
		);
	}
	static public void CacheDelete(this IEnumerable<MagikiteData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Delete (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<MagikiteData>>();
			}
		);
	}

	static public void CacheDelete(this MagikiteData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Delete (con, trans);
				ServerModelCache.Modify<IEnumerable<MagikiteData>>();
			}
		);
	}

	static public void CacheSet(this IEnumerable<ConsumerData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Save (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<ConsumerData>>();
			}
		);
	}

	static public void CacheSet(this ConsumerData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Save (con, trans);
				ServerModelCache.Modify<IEnumerable<ConsumerData>>();
			}
		);
	}
	static public void CacheDelete(this IEnumerable<ConsumerData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Delete (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<ConsumerData>>();
			}
		);
	}

	static public void CacheDelete(this ConsumerData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Delete (con, trans);
				ServerModelCache.Modify<IEnumerable<ConsumerData>>();
			}
		);
	}

	static public void CacheSet(this IEnumerable<FormationData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Save (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<FormationData>>();
			}
		);
	}

	static public void CacheSet(this FormationData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Save (con, trans);
				ServerModelCache.Modify<IEnumerable<FormationData>>();
			}
		);
	}
	static public void CacheDelete(this IEnumerable<FormationData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Delete (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<FormationData>>();
			}
		);
	}

	static public void CacheDelete(this FormationData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Delete (con, trans);
				ServerModelCache.Modify<IEnumerable<FormationData>>();
			}
		);
	}

	static public void CacheSet(this IEnumerable<MaterialData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Save (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<MaterialData>>();
			}
		);
	}

	static public void CacheSet(this MaterialData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Save (con, trans);
				ServerModelCache.Modify<IEnumerable<MaterialData>>();
			}
		);
	}
	static public void CacheDelete(this IEnumerable<MaterialData> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Delete (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<MaterialData>>();
			}
		);
	}

	static public void CacheDelete(this MaterialData self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Delete (con, trans);
				ServerModelCache.Modify<IEnumerable<MaterialData>>();
			}
		);
	}

	static public void CacheSet(this IEnumerable<QuestAchievement> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Save (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<QuestAchievement>>();
			}
		);
	}

	static public void CacheSet(this QuestAchievement self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Save (con, trans);
				ServerModelCache.Modify<IEnumerable<QuestAchievement>>();
			}
		);
	}
	static public void CacheDelete(this IEnumerable<QuestAchievement> self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				foreach(var x in self) {
					x.Delete (con, trans);
				}
				ServerModelCache.Modify<IEnumerable<QuestAchievement>>();
			}
		);
	}

	static public void CacheDelete(this QuestAchievement self)
	{
		var connection = ServerModelCache.GetConnection ();
		connection.MakeTransaction (
			(con, trans) => {
				self.Delete (con, trans);
				ServerModelCache.Modify<IEnumerable<QuestAchievement>>();
			}
		);
	}

}
