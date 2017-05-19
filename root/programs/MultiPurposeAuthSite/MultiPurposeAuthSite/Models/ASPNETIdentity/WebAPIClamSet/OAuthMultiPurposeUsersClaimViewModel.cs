﻿//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
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
//* クラス名        ：OAuthMultiPurposeUsersClaimViewModel
//* クラス日本語名  ：OAuthのClaimを返すための汎用ViewModel（ライブラリ）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/04/24  西野 大介         新規
//**********************************************************************************

using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

/// <summary>MultiPurposeAuthSite.Models.ASPNETIdentity.TokensClaimSet</summary>
namespace MultiPurposeAuthSite.Models.ASPNETIdentity.TokensClaimSet
{
    /// <summary>OAuthのClaimを返すための汎用ViewModel（ライブラリ）</summary>
    public class OAuthMultiPurposeUsersClaimViewModel : OAuthBaseClaimViewModel
    {
        [JsonProperty(PropertyName = "addition", Order = 3)] // 末尾
        /// <summary>AdditionalInfo</summary>
        public object AdditionalInfo { get; set; }

    }
}