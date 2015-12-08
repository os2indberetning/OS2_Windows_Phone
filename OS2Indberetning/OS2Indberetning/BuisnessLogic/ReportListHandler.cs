﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OS2Indberetning.Model;

namespace OS2Indberetning.BuisnessLogic
{
    public static class ReportListHandler
    {
        /// <summary>
        /// Adds report to the stored report list
        /// </summary>
        /// <param name="report">the DriveReport to be added to the list</param>
        /// <returns>true on success, false on failure</returns>
        public static async Task<bool> AddReportToList(DriveReport report)
        {
            try
            {
                var content = await FileHandler.ReadFileContent(Definitions.ReportsFileName, Definitions.ReportsFolderName);

                var list = JsonConvert.DeserializeObject<List<DriveReport>>(content);
                if (list == null)
                {
                    list = new List<DriveReport>();
                }

                list.Add(report);

                var toBeWritten = JsonConvert.SerializeObject(list);

                var test = await FileHandler.WriteFileContent(Definitions.ReportsFileName, Definitions.ReportsFolderName, toBeWritten);

                return test;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Fetches the report list from the stored file
        /// </summary>
        /// <returns>returns the stored report list</returns>
        public static async Task<List<DriveReport>> GetReportList()
        {
            try
            {
                var content = await FileHandler.ReadFileContent(Definitions.ReportsFileName, Definitions.ReportsFolderName);

                var list = JsonConvert.DeserializeObject<List<DriveReport>>(content);
                if (list == null)
                {
                    list = new List<DriveReport>();
                }

                return list;
            }
            catch (Exception e)
            {
                return new List<DriveReport>(); ;
            }
        }

        /// <summary>
        /// Removes specific report from the stored list
        /// </summary>
        /// <param name="report">the DriveReport to be removed from the list</param>
        /// <returns>the DriveReport list after removal of the specific DriveReport/returns>
        public static async Task<List<DriveReport>> RemoveReportFromList(DriveReport report)
        {
            try
            {
                var content = await FileHandler.ReadFileContent(Definitions.ReportsFileName, Definitions.ReportsFolderName);

                var list = JsonConvert.DeserializeObject<List<DriveReport>>(content);

                var item = list.FindIndex(x => x.Date == report.Date && x.Route.TotalDistance == report.Route.TotalDistance);
                list.RemoveAt(item);

                var toBeWritten = JsonConvert.SerializeObject(list);

                await FileHandler.WriteFileContent(Definitions.ReportsFileName, Definitions.ReportsFolderName, toBeWritten);
               
                return list;
            }
            catch (Exception e)
            {
                return new List<DriveReport>(); ;
            }
        }
    }
}
