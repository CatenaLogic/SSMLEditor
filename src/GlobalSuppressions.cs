
// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("WpfAnalyzers.DependencyProperties", "WPF1010:Property '[property]' must notify when value changes.", Justification = "Don't enforce this")]
[assembly: SuppressMessage("WpfAnalyzers.DependencyProperties", "WPF1011:Implement INotifyPropertyChanged.", Justification = "Don't enforce this")]
[assembly: SuppressMessage("WpfAnalyzers.DependencyProperties", "WPF1013:Use [CallerMemberName].", Justification = "Not all class libraries support this (yet)")]

