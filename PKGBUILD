pkgname=dnd-helper
pkgver=0.1.1
pkgrel=1
pkgdesc="A simple dnd combat helper app"
arch=('x86_64')
depends=()
makedepends=('dotnet-sdk')
source=()

build() {
    cp -r "$startdir"/*.fs "$startdir"/*.fsproj "$startdir"/*.json "$srcdir/" 2>/dev/null || true
    cd "$srcdir"
    
    dotnet clean
    dotnet publish -c Release -r linux-x64 --self-contained true -o published
}

package() {
    mkdir -p "$pkgdir/usr/share/dnd-helper"
    
    cp -r "$srcdir/published/"* "$pkgdir/usr/share/dnd-helper/"
    
    install -Dm755 /dev/stdin "$pkgdir/usr/bin/dnd-helper" << 'EOF'
#!/bin/bash
exec /usr/share/dnd-helper/dnd-helper "$@"
EOF
}
