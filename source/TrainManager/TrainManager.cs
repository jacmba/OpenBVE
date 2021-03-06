﻿using System;
using LibRender2;
using OpenBveApi.Hosts;

namespace TrainManager
{
	/// <summary>The base train manager class</summary>
	public abstract class TrainManagerBase
	{
		public static HostInterface currentHost;
		public static BaseRenderer Renderer;
		internal static Random RandomNumberGenerator = new Random();
	}
}
