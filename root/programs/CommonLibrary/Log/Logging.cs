﻿//**********************************************************************************
//* Copyright (C) 2017 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：Logging
//* クラス日本語名  ：Logging（ライブラリ）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/06/14  西野 大介         新規
//**********************************************************************************

using MultiPurposeAuthSite.Co;
using MultiPurposeAuthSite.Data;

using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

using Touryo.Infrastructure.Public.Log;

namespace MultiPurposeAuthSite.Log
{
    /// <summary>Logging</summary>
    /// <remarks>
    /// DebugTraceは、別フラグ（EnabeDebugTraceLog）で制御
    /// </remarks>
    public class Logging
    {
        #region ACCESS
        /// <summary>MyDebugTrace</summary>
        /// <param name="log">string</param>
        public static void MyDebugTrace(string log)
        {
            // デバッグ時にログ出力
            if (Config.IsDebug)
            {
                Debug.WriteLine(log);
            }

            // プロビジョニング、プロダクト環境
            if (Config.EnabeDebugTraceLog)
            {
                LogIF.DebugLog("ACCESS", log);
            }
        }

        /// <summary>MyDebugLogForEx</summary>
        /// <param name="log">string</param>
        public static void MyDebugLogForEx(Exception ex)
        {
            if (Config.IsDebug)
            {
                // デバッグ時
                Debug.WriteLine(ex.ToString());
                LogIF.DebugLog("ACCESS", ex.ToString());
            }
            else
            {
                // プロビジョニング、プロダクト環境
                LogIF.DebugLog("ACCESS", ex.ToString());
            }
        }
        #endregion

        #region SQLTRACE
        /// <summary>MyDebugSQLTrace</summary>
        /// <param name="log">string</param>
        public static void MyDebugSQLTrace(string log)
        {
            // デバッグ時
            if (Config.IsDebug)
            {
                Debug.WriteLine(log);
            }

            // プロビジョニング、プロダクト環境
            if (Config.EnabeDebugTraceLog)
            {
                LogIF.DebugLog("SQLTRACE", log);
            }
        }

        /// <summary>MySQLLogForEx</summary>
        /// <param name="log">string</param>
        public static void MySQLLogForEx(Exception ex)
        {
            if (Config.IsDebug)
            {
                // デバッグ時
                Debug.WriteLine(ex.ToString());
                LogIF.DebugLog("SQLTRACE", ex.ToString());
            }
            else
            {
                // プロビジョニング、プロダクト環境
                LogIF.DebugLog("SQLTRACE", ex.ToString());
            }

            if (ex is StopUserStoreException)
            {
                // ユーザストアを停止させる例外

                // スタックトレースを保って ex を throw
                //string trace = Environment.StackTrace;
                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }
        #endregion

        #region OPERATION
        /// <summary>MyOperationTrace</summary>
        /// <param name="log">string</param>
        public static void MyOperationTrace(string log)
        {
            if (Config.IsDebug)
            {
                // デバッグ時
                Debug.WriteLine(log);
                LogIF.DebugLog("OPERATION", log);

            }
            else
            {
                // 本番時
                LogIF.DebugLog("OPERATION", log);
            }
        }
        #endregion
        
        /// <summary>GetParametersString</summary>
        /// <param name="parameters">string[]</param>
        /// <returns>Parameters string</returns>
        public static string GetParametersString(ParameterInfo[] parameters)
        {
            string s = "";

            if (Config.IsDebug)
            {
                s += "(";
                for (int i = 0; i < parameters.Length; i++)
                {
                    s += parameters[i].Name + ", ";
                }
                if (s.Length >= 2)
                {
                    s = s.Substring(0, s.Length - 2);
                }
                s += ")";
            }

            return s;
        }
    }
}