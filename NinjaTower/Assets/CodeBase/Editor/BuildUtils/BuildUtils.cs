using System;

namespace Carotaa.Code.Editor
{
    public static class BuildUtils
    {
        public static void StartBuild()
        {
            var path = GetScriptArg("--path");

            EventTrack.LogTrace($"Build Success at {path}");
        }

        private static string GetScriptArg(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = args.Length - 2; i >= 0; i--)
                if (args[i] == name)
                    return args[i + 1];

            return null;
        }
    }
}