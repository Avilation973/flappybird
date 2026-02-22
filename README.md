# Flappy Bird (C# WinForms)

Bu depo C# ile yazılmış basit bir Flappy Bird klonu içerir.

## Çalıştırma (Windows + .NET SDK 8)

```powershell
dotnet run
```

## EXE üretme

Aşağıdaki komut tek dosya Windows EXE üretir:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Çıktı yolu:

`bin\Release\net8.0-windows\win-x64\publish\FlappyBird.exe`

## Android (APK) notu

Bu proje WinForms olduğu için doğrudan APK üretemez. APK için .NET MAUI veya Unity gibi Android hedefleyen bir proje gerekir.
