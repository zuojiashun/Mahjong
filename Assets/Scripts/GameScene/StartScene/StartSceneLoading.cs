﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LogoSceneLoading : SceneProcedure
{
	protected Dictionary<LAYOUT_TYPE, LayoutLoadInfo> mLoadInfo;
	protected int mLoadedCount;
	public LogoSceneLoading()
	{ }
	public LogoSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{
		mLoadInfo = new Dictionary<LAYOUT_TYPE, LayoutLoadInfo>();
		mLoadInfo.Add(LAYOUT_TYPE.LT_LOGIN, new LayoutLoadInfo(LAYOUT_TYPE.LT_LOGIN, 0));
		mLoadInfo.Add(LAYOUT_TYPE.LT_REGISTER, new LayoutLoadInfo(LAYOUT_TYPE.LT_REGISTER, 0));
		// 由于使用了较多的NGUI控件,所以禁用全局触摸检测,只加载不显示
		mLoadInfo.Add(LAYOUT_TYPE.LT_GLOBAL_TOUCH, new LayoutLoadInfo(LAYOUT_TYPE.LT_GLOBAL_TOUCH, 100));
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		mLoadedCount = 0;
		foreach (var item in mLoadInfo)
		{
			LayoutTools.LOAD_LAYOUT_ASYNC(item.Key, item.Value.mOrder, onLayoutLoaded);
		}
		// 开始加载关键帧资源,音效资源,布局使用预设资源
		mKeyFrameManager.loadAll(true);
		mAudioManager.loadAll(true);
		mLayoutPrefabManager.loadAll(true);
	}
	protected override void onUpdate(float elapsedTime)
	{
		if (mLoadedCount == mLoadInfo.Count && mKeyFrameManager.isLoadDone() && mAudioManager.isLoadDone() && mLayoutPrefabManager.isLoadDone())
		{
			// 加载结束后进入登录流程
			CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure(true, true);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_LOGIN;
			mCommandSystem.pushDelayCommand(cmd, mGameScene);
		}
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		;
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
	//----------------------------------------------------------------------------------------------------------------------
	protected void onLayoutLoaded(GameLayout layout)
	{
		mLoadInfo[layout.getType()].mLayout = layout;
		++mLoadedCount;
	}
}