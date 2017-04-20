call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\vsvars32.bat"
devenv Game.sln /build "Release"
call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\vsvars32.bat" 
devenv Game.sln /build "Debug"
move  Game\bin\Debug\Game.dll   ..\InfiniteWar\Assets\Patcher\Prefabs\code\gamedebug.bytes
move  Game\bin\Debug\Game.pdb  ..\InfiniteWar\Assets\Patcher\Prefabs\code\gamepdb.bytes
move  Game\bin\Release\Game.dll  ..\InfiniteWar\Assets\Patcher\Prefabs\code\game.bytes
pause