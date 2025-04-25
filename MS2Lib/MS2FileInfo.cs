using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MiscUtils.IO;

namespace MS2Lib;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class MS2FileInfo : IMS2FileInfo, IEquatable<MS2FileInfo> {

    public MS2FileInfo(string id, string path) {
        Id = id;
        Path = path.Replace('\\', '/');
        RootFolderId = BuildRootFolderId(Path);
    }

    [ExcludeFromCodeCoverage]
    private string DebuggerDisplay =>
        $"Id = {Id}, Path = {Path}";
    public string Id { get; }
    public string Path { get; }
    public string RootFolderId { get; }

    private static string BuildRootFolderId(string path) {
        if (string.IsNullOrWhiteSpace(path)) {
            return string.Empty;
        }

        string rootDirectory = PathEx.GetRootDirectory(path);
        if (string.IsNullOrWhiteSpace(rootDirectory)) {
            return string.Empty;
        }

        var sb = new StringBuilder(rootDirectory.Length * 2);

        for (int i = 0; i < rootDirectory.Length; i++) {
            char c = rootDirectory[i];
            if (c == '_') {
                sb.Append(c);
                continue;
            }

            if (c >= '0' && c <= '9' ||
                c >= 'A' && c <= 'Z' ||
                c >= 'a' && c <= 'z') {
                // valid
                sb.Append((byte) (c - '0'));
            } else {
                throw new ArgumentException(nameof(path), $"Path contains an invalid character '{c}'.");
            }
        }

        return sb.ToString();
    }

    #region Equality
    public override bool Equals(object obj) {
        return Equals(obj as MS2FileInfo);
    }

    public virtual bool Equals(MS2FileInfo other) {
        return other != null &&
               Id == other.Id &&
               Path == other.Path;
    }

    bool IEquatable<IMS2FileInfo>.Equals(IMS2FileInfo other) {
        return Equals(other as MS2FileInfo);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Id, Path);
    }

    public static bool operator ==(MS2FileInfo left, MS2FileInfo right) {
        return EqualityComparer<MS2FileInfo>.Default.Equals(left, right);
    }

    public static bool operator !=(MS2FileInfo left, MS2FileInfo right) {
        return !(left == right);
    }
    #endregion
}
