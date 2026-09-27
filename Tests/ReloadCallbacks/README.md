# Multiplayer reload callback regression

Run from the ModReloader directory:

```powershell
dotnet msbuild ModReloader.csproj -t:Compile -p:Configuration=Debug
dotnet run --project Tests/ReloadCallbacks/ReloadCallbacks.csproj -- 'D:\Steam\steamapps\common\tModLoader' '.' 'obj/Debug/net8.0/ModReloader.dll'
```

The runner uses the compiled mod and tModLoader's actual NetReload continuation.
It checks that unload removes only Mod Reloader's own callbacks, server-required
reloads cannot enqueue another auto-join, and ordinary developer auto-join still
works. It starts no game or network connection. The original DLL fails the first
assertion; the fixed DLL passes all 14 checks.

The failure chain is: a server-required reload installs OnSuccessfulLoad; the old
Unload sets the whole field to null; the client returns to the menu without
finishing or closing its connection; joining again can start another client loop
against tModLoader's shared connection/read buffer. Fully restart the client after
installing the fix to remove any network threads left over by the old build.
