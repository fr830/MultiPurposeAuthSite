﻿//**********************************************************************************
//* テンプレート
//**********************************************************************************

// 以下のLicenseに従い、このProjectをTemplateとして使用可能です。Release時にCopyright表示してSublicenseして下さい。
// https://github.com/OpenTouryoProject/MultiPurposeAuthSite/blob/master/license/LicenseForTemplates.txt

//**********************************************************************************
//* クラス名        ：IEmailSender
//* クラス日本語名  ：IEmailSender
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/30  西野 大介         新規
//**********************************************************************************

using System.Threading.Tasks;

namespace MultiPurposeAuthSite.Notifications
{
    /// <summary>IEmailSender</summary>
    public interface IEmailSender
    {
        Task SendAsync(string destination, string subject, string body);
    }
}
