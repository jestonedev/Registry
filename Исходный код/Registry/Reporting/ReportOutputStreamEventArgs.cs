﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Registry.Reporting
{
    public class ReportOutputStreamEventArgs : EventArgs
    {
        public string Text { get; set; }

        public ReportOutputStreamEventArgs(string text)
        {
            this.Text = text;
        }
    }
}
