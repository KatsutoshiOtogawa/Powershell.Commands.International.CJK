using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace Powershell.Commands.International.CJK
{
    public enum CJK
    {
        Kanji,
        Kana,
        Hiragana,
        Hungul
    }
    // string input = "カキクケコ";
    // string output = KanaConverter.KatakanaToHiragana(input);
    // Console.WriteLine(output);
    /// <summary>
    /// Defines the implementation of the 'Get-WinEnviromentVariable' cmdlet.
    /// This cmdlet get the content from EnvironemtVariable.
    /// </summary>
    [Cmdlet(VerbsData.ConvertTo, "Width", DefaultParameterSetName = "DefaultSet", HelpUri = "https://github.com/KatsutoshiOtogawa/PowerShell.Commands.True.Deal.EnvironmentVariable/blob/master/PowerShell.Commands.True.Deal.EnvironmentVariable/Help/Get-WinEnvironmentVariable.md")]
    [OutputType(typeof(PSObject), ParameterSetName = new[] { "DefaultSet" })]
    // [OutputType(typeof(string))]
    [OutputType(typeof(string), ParameterSetName = new[] { "RawSet" })]
    public class ConvertToFullWidthCommand : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the path parameter to the command.
        /// </summary>
        [Parameter(Position = 0, ParameterSetName = "str",
                   Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] Str
        {
            get
            {
                return _strs;
            }

            set
            {
                _strs = value;
            }
        }

        private string[] _strs;

        /// <summary>
        /// Gets or sets the cjk.
        /// </summary>
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "str")]
        [ValidateNotNullOrEmpty]
        public CJK cjk { get; set; }

        /// <summary>
        /// Gets or sets the LocalId. LocalId is 
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = "str")]
        [ValidateNotNullOrEmpty]
        public int? LocaleId { get; set; } = null;


        /// <summary>
        /// Gets or sets the EnvironmentVariableTarget.
        /// </summary>
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "DefaultSet")]
        [Parameter(Position = 1, Mandatory = false, ParameterSetName = "RawSet")]
        [ValidateNotNullOrEmpty]
        public EnvironmentVariableTarget Target { get; set; } = EnvironmentVariableTarget.Process;

        /// <summary>
        /// Gets or sets property that sets delimiter.
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = "DefaultSet")]
        [ValidateNotNullOrEmpty]
        public char? Delimiter { get; set; } = null;

        /// <summary>
        /// Gets or sets raw parameter. This will allow EnvironmentVariable return text or file list as one string.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "RawSet")]
        public SwitchParameter Raw { get; set; }

        private static readonly List<string> DetectedDelimiterEnvrionmentVariable = new List<string> { "Path", "PATHEXT", "PSModulePath" };

        /// <summary>
        /// This method implements the ProcessRecord method for Get-WinEnvironmentVariable command.
        /// Returns the Specify Name EnvironmentVariable content as text format.
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (string str in Str)
            {
                Collection<string> result = null;

                try
                {
                    if (LocaleId is null)
                    {
                        Strings.StrConv(str, VbStrConv.Wide);
                    }
                    else
                    {
                        Strings.StrConv(str, VbStrConv.Wide, (int)LocaleId);
                    }
                }
                catch
                {

                }
                /*
                Collection<PathInfo> result = null;
                try
                {
                    result = SessionState.Path.GetResolvedPSPathFromPSPath(path, CmdletProviderContext);

                    if (_relative)
                    {
                        foreach (PathInfo currentPath in result)
                        {
                            // When result path and base path is on different PSDrive
                            // (../)*path should not go beyond the root of base path
                            if (currentPath.Drive != SessionState.Path.CurrentLocation.Drive &&
                                SessionState.Path.CurrentLocation.Drive != null &&
                                !currentPath.ProviderPath.StartsWith(
                                    SessionState.Path.CurrentLocation.Drive.Root, StringComparison.OrdinalIgnoreCase))
                            {
                                WriteObject(currentPath.Path, enumerateCollection: false);
                                continue;
                            }

                            string adjustedPath = SessionState.Path.NormalizeRelativePath(currentPath.Path,
                                SessionState.Path.CurrentLocation.ProviderPath);
                            // Do not insert './' if result path is not relative
                            if (!adjustedPath.StartsWith(
                                    currentPath.Drive?.Root ?? currentPath.Path, StringComparison.OrdinalIgnoreCase) &&
                                !adjustedPath.StartsWith('.'))
                            {
                                adjustedPath = SessionState.Path.Combine(".", adjustedPath);
                            }

                            WriteObject(adjustedPath, enumerateCollection: false);
                        }
                    }
                }
                catch (PSNotSupportedException notSupported)
                {
                    WriteError(
                        new ErrorRecord(
                            notSupported.ErrorRecord,
                            notSupported));
                    continue;
                }
                catch (DriveNotFoundException driveNotFound)
                {
                    WriteError(
                        new ErrorRecord(
                            driveNotFound.ErrorRecord,
                            driveNotFound));
                    continue;
                }
                catch (ProviderNotFoundException providerNotFound)
                {
                    WriteError(
                        new ErrorRecord(
                            providerNotFound.ErrorRecord,
                            providerNotFound));
                    continue;
                }
                catch (ItemNotFoundException pathNotFound)
                {
                    WriteError(
                        new ErrorRecord(
                            pathNotFound.ErrorRecord,
                            pathNotFound));
                    continue;
                }

                if (!_relative)
                {
                    WriteObject(result, enumerateCollection: true);
                }
                */
            }
            var localId = 0x411;


            PSObject env;
            PSNoteProperty envname;
            PSNoteProperty envvalue;
            PSNoteProperty envtype;

            if (string.IsNullOrEmpty(Name))
            {
                foreach (DictionaryEntry kvp in Environment.WinGetEnvironmentVariables(Target))
                {
                    env = new PSObject();
                    envname = new PSNoteProperty("Name", kvp.Key.ToString());
                    envtype = Target switch
                    {
                        EnvironmentVariableTarget.Process => new PSNoteProperty("RegistryValueKind", RegistryValueKind.None),
                        _ => new PSNoteProperty("RegistryValueKind", Environment.WinGetEnvironmentValueKind(kvp.Key.ToString()!, Target))
                    };
                    envvalue = new PSNoteProperty("Value", kvp.Value?.ToString());
                    env.Properties.Add(envname);
                    env.Properties.Add(envtype);
                    env.Properties.Add(envvalue);

                    this.WriteObject(env, true);
                }
                return;
            }
            var contentList = new List<string>();

            // try catch IOExceptionがありうる。環境変数が無い場合の
            string? textContent = Environment.WinGetEnvironmentVariable(Name, Target);
            if (string.IsNullOrEmpty(textContent))
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    WinEnvironmentVariableResources.EnvironmentVariableNotFoundOrEmpty,
                    Name
                );

                ArgumentException argumentException = new ArgumentException(message);
                ErrorRecord errorRecord = new(
                    argumentException,
                    "EnvironmentVariableNotFoundOrEmpty",
                    ErrorCategory.ObjectNotFound,
                    Name);
                ThrowTerminatingError(errorRecord);
                return;
            }

            if (ParameterSetName == "RawSet")
            {
                contentList.Add(textContent);
                this.WriteObject(textContent, true);
                return;
            }
            else
            {
                if (DetectedDelimiterEnvrionmentVariable.Contains(Name))
                {
                    Delimiter = Path.PathSeparator;
                }

                contentList.AddRange(textContent.Split(Delimiter.ToString() ?? string.Empty, StringSplitOptions.None));
            }

            env = new PSObject();
            envname = new PSNoteProperty("Name", Name);
            envtype = Target switch
            {
                EnvironmentVariableTarget.Process => new PSNoteProperty("RegistryValueKind", RegistryValueKind.None),
                _ => new PSNoteProperty("RegistryValueKind", Environment.WinGetEnvironmentValueKind(Name, Target))
            };
            envvalue = new PSNoteProperty("Value", contentList);

            env.Properties.Add(envname);
            env.Properties.Add(envtype);
            env.Properties.Add(envvalue);

            this.WriteObject(env, true);
        }
    }

}