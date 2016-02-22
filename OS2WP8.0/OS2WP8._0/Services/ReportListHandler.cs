/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OS2Indberetning.Model;

namespace OS2Indberetning.BuisnessLogic
{
    /// <summary>
    /// Class to handle saved DriveReports through the use of FileHandler
    /// </summary>
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
                if(test)
                    Definitions.storedReportsCount = list.Count;

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
        /// <returns>the DriveReport list after removal of the specific DriveReport</returns>
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
                Definitions.storedReportsCount = list.Count;
                return list;
            }
            catch (Exception e)
            {
                return new List<DriveReport>(); ;
            }
        }

        /// <summary>
        /// Removes all reports from the stored list
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public static async Task<bool> DeleteEntireList()
        {
            try
            {
                var list = new List<DriveReport>();

                var toBeWritten = JsonConvert.SerializeObject(list);

                await FileHandler.WriteFileContent(Definitions.ReportsFileName, Definitions.ReportsFolderName, toBeWritten);
                Definitions.storedReportsCount = list.Count;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the amount of stored reports
        /// </summary>
        /// <returns>integer representing the amount of stored reports</returns>
        public static async Task GetCount()
        {
            try
            {
                var content = await FileHandler.ReadFileContent(Definitions.ReportsFileName, Definitions.ReportsFolderName);

                var list = JsonConvert.DeserializeObject<List<DriveReport>>(content);

                Definitions.storedReportsCount = list.Count;
            }
            catch (Exception e)
            {
                Definitions.storedReportsCount = 0;
            }
        }
    }
}
