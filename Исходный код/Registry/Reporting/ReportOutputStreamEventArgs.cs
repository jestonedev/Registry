﻿using System;

namespace Registry.Reporting
{
    public class ReportOutputStreamEventArgs : EventArgs
    {
        public string Text { get; set; }

        public ReportOutputStreamEventArgs(string text)
        {
            Text = text;
        }
    }
}
