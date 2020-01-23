@ECHO OFF
SET base=%~dp0
RD /S /Q %base%obj
docfx %base%docfx.json --serve
