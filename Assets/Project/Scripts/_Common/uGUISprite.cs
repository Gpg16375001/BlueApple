using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

using SmileLab;


/// <summary>
/// uGUIのスプライト読み込みロジック.
/// </summary>
[RequireComponent(typeof(Image))]
public class uGUISprite : ViewBase
{
    [SerializeField]
    private SpriteAtlas atlas;
    [SerializeField]
    private string spriteName;


    /// <summary>
    /// スプライト差し替え.
    /// </summary>
    public void ChangeSprite(string sptName)
    {
        var spt = atlas.GetSprite(sptName);
        this.GetComponent<Image>().sprite = spt;
        spriteName = sptName;
    }

    /// <summary>
    /// ローカルResourcesからアトラスをロード、差し替える.
    /// </summary>
    public void LoadAtlasFromResources(string atlasName, string sptName = null)
	{
		var loadAtlas = Resources.Load("Atlases/"+atlasName) as SpriteAtlas;
		atlas = loadAtlas;
		if(!string.IsNullOrEmpty(sptName)){
			this.ChangeSprite(sptName);
		}
	}

	void Awake()
	{
        if(!string.IsNullOrEmpty(spriteName)){
            ChangeSprite(spriteName);
        }
	}
}
