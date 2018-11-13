using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class PurchaseLimitationExtensions
{
	static Dictionary<PurchaseLimitationEnum, int> dict = new Dictionary<PurchaseLimitationEnum, int>()
	{
		{ PurchaseLimitationEnum.unlimited, 0 },
		{ PurchaseLimitationEnum.once_per_day, 1 },
		{ PurchaseLimitationEnum.once_per_player, 1 },
		{ PurchaseLimitationEnum.three_per_day, 3 },
		{ PurchaseLimitationEnum.five_per_day, 5 },
		{ PurchaseLimitationEnum.timelimit, 0 },
		{ PurchaseLimitationEnum.three_per_player, 3 },
		{ PurchaseLimitationEnum.five_per_player, 5 },
		{ PurchaseLimitationEnum.once_per_month, 1 },
		{ PurchaseLimitationEnum.three_per_month, 3 },
		{ PurchaseLimitationEnum.five_per_month, 5 },
		{ PurchaseLimitationEnum.ten_per_month, 10 },
	};

	public static int Denominator( this PurchaseLimitation self ) {
		return dict.ContainsKey( self.Enum ) ? dict[self.Enum] : -1;
	}
}
