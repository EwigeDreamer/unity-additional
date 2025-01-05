#if ODIN_INSPECTOR

using System;
using System.Globalization;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace ED.Additional.Editor
{
    public class DateTimeDrawer : OdinValueDrawer<DateTime>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            ValueEntry.SmartValue = DateTimeField(label, ValueEntry.SmartValue);
            ValueEntry.ApplyChanges();
        }
        
        private static DateTime DateTimeField(GUIContent label, DateTime value)
        {
            var dateTime = value;
            SirenixEditorGUI.BeginIndentedHorizontal();

            EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));

            var year = SirenixEditorFields.IntField(dateTime.Year);
            var month = SirenixEditorFields.Dropdown(dateTime.Month - 1, DateTimeFormatInfo.CurrentInfo.MonthNames) + 1;
            var day = SirenixEditorFields.IntField(dateTime.Day);

            GUILayout.Space(20);

            var hour = SirenixEditorFields.IntField(dateTime.Hour);
            var min = SirenixEditorFields.IntField(dateTime.Minute);
            var sec = SirenixEditorFields.IntField(dateTime.Second);

            if (year != dateTime.Year)
            {
                var diff = year - dateTime.Year;
                dateTime = dateTime.AddYears(diff);
            }

            if (month != dateTime.Month)
            {
                var diff = month - dateTime.Month;
                dateTime = dateTime.AddMonths(diff);
            }

            if (day != dateTime.Day)
            {
                var diff = day - dateTime.Day;
                dateTime = dateTime.AddDays(diff);
            }

            if (hour != dateTime.Hour)
            {
                var diff = hour - dateTime.Hour;
                dateTime = dateTime.AddHours(diff);
            }

            if (min != dateTime.Minute)
            {
                var diff = min - dateTime.Minute;
                dateTime = dateTime.AddMinutes(diff);
            }

            if (sec != dateTime.Second)
            {
                var diff = sec - dateTime.Second;
                dateTime = dateTime.AddSeconds(diff);
            }

            if (SirenixEditorGUI.IconButton(EditorIcons.Refresh))
            {
                dateTime = DateTime.Now;
                //year = date_time.Year;
                //month = date_time.Month;
                //day = date_time.Day;
                //hour = date_time.Hour;
                //min = date_time.Minute;
                //sec = date_time.Second;
            }

            SirenixEditorGUI.EndIndentedHorizontal();

            return dateTime;
        }
    }
}

#endif