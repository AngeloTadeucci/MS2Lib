using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MS2Lib;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class MS2FileHeader : IMS2FileHeader, IEquatable<MS2FileHeader> {

    public MS2FileHeader(long size, uint id, long offset, CompressionType compressionType = CompressionType.None) :
        this(new MS2SizeHeader(size), id, offset, compressionType) { }

    public MS2FileHeader(long encodedSize, long compressedSize, long size, uint id, long offset, CompressionType compressionType = CompressionType.None) :
        this(new MS2SizeHeader(encodedSize, compressedSize, size), id, offset, compressionType) { }

    public MS2FileHeader(IMS2SizeHeader size, uint id, long offset, CompressionType compressionType = CompressionType.None) {
        Size = size ?? throw new ArgumentNullException(nameof(size));
        Id = id;
        Offset = offset;
        CompressionType = compressionType;
    }

    [ExcludeFromCodeCoverage]
    protected virtual string DebuggerDisplay
        => $"Offset = {Offset}, {Size.EncodedSize}->{Size.CompressedSize}->{Size.Size}";
    public uint Id { get; }
    public long Offset { get; }
    public CompressionType CompressionType { get; }
    public IMS2SizeHeader Size { get; }

    #region Equality
    public override bool Equals(object obj) {
        return Equals(obj as MS2FileHeader);
    }

    public virtual bool Equals(MS2FileHeader other) {
        return other != null &&
               Id == other.Id &&
               Offset == other.Offset &&
               CompressionType == other.CompressionType &&
               EqualityComparer<IMS2SizeHeader>.Default.Equals(Size, other.Size);
    }

    bool IEquatable<IMS2FileHeader>.Equals(IMS2FileHeader other) {
        return Equals(other as MS2FileHeader);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Id, Offset, CompressionType, Size);
    }

    public static bool operator ==(MS2FileHeader left, MS2FileHeader right) {
        return EqualityComparer<MS2FileHeader>.Default.Equals(left, right);
    }

    public static bool operator !=(MS2FileHeader left, MS2FileHeader right) {
        return !(left == right);
    }
    #endregion
}
