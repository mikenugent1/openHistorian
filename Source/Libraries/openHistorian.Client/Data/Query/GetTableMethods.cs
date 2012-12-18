﻿//******************************************************************************************************
//  GetTableMethods.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/12/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace openHistorian.Data.Query
{
    public interface IDelinearlizedSignals
    {
        IList<string> ColumnHeaders { get; }
        KeyValuePair<object, IList<ISignalWithType>> ColumnGroups { get; }
    }

    public static class GetTableMethods
    {

        public static DataTable GetTable(this IHistorianDatabase database, ulong start, ulong stop, IDelinearlizedSignals signals)
        {
            return null;
        }

        public static DataTable GetTable(this IHistorianDatabase database, ulong start, ulong stop, IList<ISignalWithType> columns)
        {
            if (columns.Any((x) => !x.HistorianId.HasValue))
            {
                throw new Exception("All columns must be contained in the historian for this function to work.");
            }

            //ToDO: Consider requiring a name with the signal for the column header

            var results = database.GetSignals(start, stop, columns);
            int[] columnPosition = new int[columns.Count];
            object[] rowValues = new object[columns.Count + 1];
            SignalDataBase[] signals = new SignalDataBase[columns.Count];

            DataTable table = new DataTable();
            table.Columns.Add("Time", typeof(DateTime));
            foreach (var signal in columns)
            {
                table.Columns.Add("", typeof(double));
            }

            for (int x = 0; x < columns.Count; x++)
            {
                signals[x] = results[columns[x].HistorianId.Value];
            }


            while (true)
            {
                ulong minDate = ulong.MaxValue;
                for (int x = 0; x < columns.Count; x++)
                {
                    var signal = signals[x];
                    if (signal.Count < columnPosition[x])
                    {
                        minDate = Math.Min(minDate, signals[x].GetDate(columnPosition[x]));
                    }
                }

                rowValues[0] = null;
                for (int x = 0; x < columns.Count; x++)
                {
                    var signal = signals[x];
                    if (signal.Count < columnPosition[x] && minDate == signals[x].GetDate(columnPosition[x]))
                    {
                        ulong date;
                        double value;
                        signals[x].GetData(columnPosition[x], out date, out value);
                        rowValues[x + 1] = value;
                        columnPosition[x]++;
                    }
                    else
                    {
                        rowValues[x + 1] = null;
                    }
                }

                if (minDate == ulong.MaxValue && rowValues.All((x) => x == null))
                    return table;
                rowValues[0] = minDate;

                table.Rows.Add(rowValues);
            }
        }

    }
}
