﻿using System;
using System.Collections.Generic;
using Entities;

namespace LNU.Courses.WebUI.Models
{
    public static class staticData
    {
        public static DateTime StartTime;
        public static DateTime firstDeadLineTime;
        public static DateTime lastDeadLineTime;
        public static List<int> disciplinesID = new List<int>();
        public static bool checkInList(int id)
        {
            bool result=false;
            foreach(var item in disciplinesID)
            {
                if (item == id)
                {
                    result = true;
                    return result;
                }
                else
                    result = false;

            }
            return result;
        }
       
    }
}