﻿// Copyright (c) 2019 DrBarnabus

namespace DacTools.Deployment.Core.Common
{
    public interface IBuildServer
    {
        bool CanApplyToCurrentContext();
        string GenerateSetProgressMessage(int current, int total, string message);
        string GenerateLogIssueWarningMessage(string issueMessage);
        string GenerateLogIssueErrorMessage(string issueMessage);
        string GenerateSetStatusSucceededWithIssuesMessage(string statusMessage);
        string GenerateSetStatusFailMessage(string statusMessage);
    }
}