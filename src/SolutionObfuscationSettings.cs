using System.Reflection;

// Disable obfuscation of views
[assembly: Obfuscation(Feature = "apply to type *.Views.*: all", Exclude = true, ApplyToMembers = true)]
