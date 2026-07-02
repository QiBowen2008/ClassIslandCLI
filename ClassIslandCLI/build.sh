#!/bin/bash
set -euo pipefail

PROJECT_NAME="ClassIslandCLI"

# ---- 从 .csproj 提取版本号 ----
VERSION=$(grep -oP '<AssemblyVersion>\K[^<]+' ClassIslandCLI.csproj | head -1)
if [ -z "$VERSION" ]; then
    echo "[ERROR] Unable to extract version from ClassIslandCLI.csproj"
    exit 1
fi

OUTPUT_DIR="publish"
RELEASE_DIR="releases"
HOST_OS="$(uname -s)"
HOST_ARCH="$(uname -m)"

echo "=========================================="
echo "  ClassIslandCLI AOT Build"
echo "  Version : $VERSION"
echo "  Host OS : $HOST_OS"
echo "  Host CPU: $HOST_ARCH"
echo "=========================================="

# ---- 清理旧构建 ----
rm -rf "$OUTPUT_DIR" "$RELEASE_DIR"
mkdir -p "$OUTPUT_DIR" "$RELEASE_DIR"

# ---- 根据宿主 OS 决定构建哪些 RID ----
# Linux  不能构建 macOS  目标
# macOS  不能构建 Linux  目标
case "$HOST_OS" in
    Linux)  RIDS=("linux-x64" "linux-arm64") ;;
    Darwin) RIDS=("osx-x64"  "osx-arm64")  ;;
    *)
        echo "[ERROR] Unsupported host OS: $HOST_OS"
        exit 1
        ;;
esac

for RID in "${RIDS[@]}"; do
    echo ""
    echo "========================================"
    echo "  Building $RID ..."
    echo "========================================"

    # 判断是否需要交叉编译并设置 OBJCOPY（仅 Linux）
    TARGET_ARCH="${RID##*-}"               # "linux-arm64" -> "arm64"
    case "$TARGET_ARCH" in
        x64)   NEEDLE="x86_64"  OBJCOPY_TOOL="x86_64-linux-gnu-objcopy"     ;;
        arm64) NEEDLE="aarch64" OBJCOPY_TOOL="aarch64-linux-gnu-objcopy"    ;;
        *)     NEEDLE=""        OBJCOPY_TOOL="" ;;
    esac

    if [ "$HOST_OS" = "Linux" ] && [ -n "$NEEDLE" ] && [ "$HOST_ARCH" != "$NEEDLE" ]; then
        if command -v "$OBJCOPY_TOOL" &>/dev/null; then
            export OBJCOPY="$OBJCOPY_TOOL"
            echo $OBJCOPY
            echo "[INFO] Cross-compiling -> OBJCOPY=$OBJCOPY"
        elif [ -f "/usr/bin/$OBJCOPY_TOOL" ]; then
            export OBJCOPY="/usr/bin/$OBJCOPY_TOOL"
            echo "[INFO] Cross-compiling -> OBJCOPY=$OBJCOPY"
        else
            echo "[ERROR] $OBJCOPY_TOOL not found. Install with:"
            echo "        sudo apt install binutils-$OBJCOPY_TOOL"
            exit 1
        fi
    fi

    dotnet publish -c Release -r "$RID" -o "$OUTPUT_DIR/$RID" --self-contained true
    echo "[OK] $RID build succeeded."

    echo "Packaging $RID ..."
    tar -czf "$RELEASE_DIR/${PROJECT_NAME}-${VERSION}-${RID}.tar.gz" -C "$OUTPUT_DIR/$RID" .
    echo "[OK] Package: $RELEASE_DIR/${PROJECT_NAME}-${VERSION}-${RID}.tar.gz"

    unset OBJCOPY 2>/dev/null || true
done

echo ""
echo "=========================================="
echo "  Build Complete!"
echo "=========================================="
echo ""
ls -lh "$RELEASE_DIR/"
