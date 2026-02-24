using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;

namespace RevitBootstrap
{
	public class Bootstrapper : IExternalApplication
	{
		private IExternalApplication _panel;

		public Result OnStartup(UIControlledApplication application)
		{
			const string panelDllPath = @"J:\Drawings REVIT FAMILIES\02 NEW\Aris\Addins\PetersimeV2\DLLs\RevitPanel.dll";

			try
			{
				if (!File.Exists(panelDllPath))
				{
					TaskDialog.Show("Petersime Bootstrap",
						"RevitPanel.dll not found at:\n" + panelDllPath +
						"\n\nMake sure the J: drive is mapped.");
					return Result.Failed;
				}

				byte[] assemblyBytes = File.ReadAllBytes(panelDllPath);
				Assembly panelAssembly = Assembly.Load(assemblyBytes);

				Type panelType = panelAssembly.GetType("RevitPanel.Panel");
				if (panelType == null)
				{
					TaskDialog.Show("Petersime Bootstrap",
						"Could not find RevitPanel.Panel in the loaded assembly.");
					return Result.Failed;
				}

				_panel = (IExternalApplication)Activator.CreateInstance(panelType);
				return _panel.OnStartup(application);
			}
			catch (Exception ex)
			{
				TaskDialog.Show("Petersime Bootstrap",
					"Failed to load RevitPanel.dll:\n" + ex.Message);
				return Result.Failed;
			}
		}

		public Result OnShutdown(UIControlledApplication application)
		{
			if (_panel != null)
				return _panel.OnShutdown(application);

			return Result.Succeeded;
		}
	}
}
