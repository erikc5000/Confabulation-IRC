using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Confabulation.Controls
{
	public class ControlResources
	{
		public static ComponentResourceKey VistaToolBarButtonKey
		{
			get { return new ComponentResourceKey(typeof(ControlResources), "vistaToolBarButtonStyle"); }
		}

		public static ComponentResourceKey VistaToolBarKey
		{
			get { return new ComponentResourceKey(typeof(ControlResources), "vistaToolBarStyle"); }
		}

		public static ComponentResourceKey VistaToolBarBackgroundKey
		{
			get { return new ComponentResourceKey(typeof(ControlResources), "vistaToolBarBackgroundBrush"); }
		}

		public static ComponentResourceKey VistaMenuKey
		{
			get { return new ComponentResourceKey(typeof(ControlResources), "vistaMenuStyle"); }
		}
	}
}
