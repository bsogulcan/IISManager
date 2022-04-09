using System;
using Microsoft.Web.Administration;

namespace IISManager.Managers
{
    public static class SiteObjectStateConverter
    {
        public static string GetString(ObjectState objectState)
        {
            return objectState switch
            {
                ObjectState.Starting => "Starting",
                ObjectState.Started => "Started",
                ObjectState.Stopping => "Stopping",
                ObjectState.Stopped => "Stopped",
                ObjectState.Unknown => "Unknown",
            };
        }
    }
}