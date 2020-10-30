﻿using OpenBots.Core.Infrastructure;
using Microsoft.Office.Interop.Excel;
using System;

namespace OpenBots.Core.Utilities.CommonUtilities
{
    public static class InstanceMethods
    {
        public static void AddAppInstance(this object appObject, IEngine engine, string instanceName)
        {

            if (engine.AppInstances.ContainsKey(instanceName) && engine.EngineSettings.OverrideExistingAppInstances)
                engine.AppInstances.Remove(instanceName);

            else if (engine.AppInstances.ContainsKey(instanceName) && !engine.EngineSettings.OverrideExistingAppInstances)
            {
                throw new Exception("App Instance already exists and override has been disabled in engine settings! " +
                    "Enable override existing app instances or use unique instance names!");
            }

            try
            {
                engine.AppInstances.Add(instanceName, appObject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object GetAppInstance(this string instanceName, IEngine engine)
        {
            try
            {
                if (engine.AppInstances.TryGetValue(instanceName, out object appObject))
                    return appObject;

                throw new Exception("App Instance '" + instanceName + "' not found!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveAppInstance(this string instanceName, IEngine engine)
        {
            try
            {
                if (engine.AppInstances.ContainsKey(instanceName))
                    engine.AppInstances.Remove(instanceName);
                else
                    throw new Exception("App Instance '" + instanceName + "' not found!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool InstanceExists(this string instanceName, IEngine engine)
        {
            try
            {
                if (engine.AppInstances.TryGetValue(instanceName, out object appObject))
                {
                    string appstring = appObject.ToString();
                    Type appType = appObject.GetType();
                    var prop = appType.GetProperties();

                    if (appType.ToString() == "Microsoft.Office.Interop.Excel.ApplicationClass")
                    {
                        
                    }
                        return true;
                }
                return false;                    
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
