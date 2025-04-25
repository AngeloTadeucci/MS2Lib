using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MS2Lib;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class MS2SizeHeader : IMS2SizeHeader, IEquatable<MS2SizeHeader> {

    public MS2SizeHeader(long size) :
        this(size, size) { }

    public MS2SizeHeader(long compressedSize, long size) :
        this(compressedSize, compressedSize, size) { }

    public MS2SizeHeader(long encodedSize, long compressedSize, long size) {
        EncodedSize = encodedSize;
        CompressedSize = compressedSize;
        Size = size;
    }

    [ExcludeFromCodeCoverage]
    protected virtual string DebuggerDisplay
        => $"{EncodedSize}->{CompressedSize}->{Size}";
    public long EncodedSize { get; }
    public long CompressedSize { get; }
    public long Size { get; }

    #region Equality
    public override bool Equals(object obj) {
        return Equals(obj as MS2SizeHeader);
    }

    public virtual bool Equals(MS2SizeHeader other) {
        return other != null &&
               EncodedSize == other.EncodedSize &&
               CompressedSize == other.CompressedSize &&
               Size == other.Size;
    }

    bool IEquatable<IMS2SizeHeader>.Equals(IMS2SizeHeader other) {
        return Equals(other as MS2SizeHeader);
    }

    public override int GetHashCode() {
        return HashCode.Combine(EncodedSize, CompressedSize, Size);
    }

    public static bool operator ==(MS2SizeHeader left, MS2SizeHeader right) {
        return EqualityComparer<MS2SizeHeader>.Default.Equals(left, right);
    }

    public static bool operator !=(MS2SizeHeader left, MS2SizeHeader right) {
        return !(left == right);
    }
    #endregion
}
