using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.CodeGen;

internal class GeneratorDiagnostics
{
    public class Ids
    {
        public const string Prefix = "JAVAGEN";
        public const string InvalidJavaImportAttributeUsage = Prefix + "1070";
        public const string ConfigurationNotSupported = Prefix + "1073";
        public const string JavaImportRequiresAllowUnsafeBlocks = Prefix + "1074";
    }

    private const string Category = "JavaImportGenerator";

    public static readonly DiagnosticDescriptor InvalidImportAttributedMethodSignature =
        new DiagnosticDescriptor(
        Ids.InvalidJavaImportAttributeUsage,
        GetResourceString(nameof(SR.InvalidJavaImportAttributeUsageTitle)),
        GetResourceString(nameof(SR.InvalidJavaImportAttributedMethodSignatureMessage)),
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: GetResourceString(nameof(SR.InvalidJavaImportAttributedMethodDescription)));

    public static readonly DiagnosticDescriptor InvalidImportAttributedMethodContainingTypeMissingModifiers =
    new DiagnosticDescriptor(
    Ids.InvalidJavaImportAttributeUsage,
    GetResourceString(nameof(SR.InvalidJavaImportAttributeUsageTitle)),
    GetResourceString(nameof(SR.InvalidAttributedMethodContainingTypeMissingModifiersMessage)),
    Category,
    DiagnosticSeverity.Error,
    isEnabledByDefault: true,
    description: GetResourceString(nameof(SR.InvalidJavaImportAttributedMethodDescription)));

    public static readonly DiagnosticDescriptor ReturnConfigurationNotSupported =
        new DiagnosticDescriptor(
            Ids.ConfigurationNotSupported,
            GetResourceString(nameof(SR.ConfigurationNotSupportedTitle)),
            GetResourceString(nameof(SR.ConfigurationNotSupportedMessageReturn)),
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: GetResourceString(nameof(SR.ConfigurationNotSupportedDescription)));


    public static readonly DiagnosticDescriptor JSImportRequiresAllowUnsafeBlocks =
               new DiagnosticDescriptor(
                   Ids.JavaImportRequiresAllowUnsafeBlocks,
                   GetResourceString(nameof(SR.JavaImportRequiresAllowUnsafeBlocksTitle)),
                   GetResourceString(nameof(SR.JavaImportRequiresAllowUnsafeBlocksMessage)),
                   Category,
                   DiagnosticSeverity.Error,
                   isEnabledByDefault: true,
                   description: GetResourceString(nameof(SR.JavaImportRequiresAllowUnsafeBlocksDescription)));

    private static LocalizableResourceString GetResourceString(string resourceName)
    {
        return new LocalizableResourceString(resourceName, SR.ResourceManager, typeof(Simple.CodeGen.SR));
    }

}