// Copyright (c) 2022 DrBarnabus

using Cake.Core;
using Common;
using Common.Extensions;
using Release.Setup;

namespace Release
{
    public class BuildLifetime : BuildLifetimeBase<BuildContext>
    {
        public override void Setup(BuildContext context, ISetupContext setupContext)
        {
            base.Setup(context, setupContext);

            context.Credentials = Credentials.GetCredentials(context);

            context.StartGroup("Build Setup");
            LogBuildInformation(context);
            context.EndGroup();
        }
    }
}
