﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowComponentHSL : ComponentHSL
{
	public WindowComponentHSL(Type type, string name)
		:
	base(type, name)
	{}
	public override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(WindowComponentHSL);
	}
	public override void applyHSL(Vector3 hsl, bool done) 
	{
		txUIStaticTexture window = (mComponentOwner) as txUIStaticTexture;
		 window.setHSLOffset(hsl);	 
	}

}
