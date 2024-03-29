﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LvpStudio.DrawingCommands
{
    // Displays CommandRecords and limits them to MAX_CHILDREN_COUNT
    class CommandDisplay : StackPanel
    {
        const int MAX_CHILDREN_COUNT = 6;

        public CommandDisplay(CommandHistory history)
        {
            history.Executed += HistoryExecuted;
            history.Undid += History_Undid;
            history.Redid += History_Redid;

            IsHitTestVisible = false;
        }

        private void HistoryExecuted(CanvasCommand command)
            => AddRecord(CommandRecord.CreateNew(command));

        private void History_Undid(CanvasCommand command)
            => AddRecord(CommandRecord.CreateNewUndid(command));

        private void History_Redid(CanvasCommand command)
            => AddRecord(CommandRecord.CreateNewRedid(command));

        void AddRecord(CommandRecord record)
        {
            Children.Add(record);
            if (Children.Count > MAX_CHILDREN_COUNT)
                Children.RemoveRange(0, Children.Count - MAX_CHILDREN_COUNT);
        }
    }
}
