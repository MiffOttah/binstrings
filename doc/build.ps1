$base = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

# DocFX is complete garbage, but apparently it's the only publicly available
# software for processing XML comments. Since I don't want to document the
# code twice, we'll use this script to partially clean up DocFX's mess.

# If the obj directory is allowed to stick around, DocFX will pull data such
# as documentation and page contents from a cache in the directory instead of
# re-reading it from the source files. We have to remove the obj directory to
# force it to do a clean build.
Remove-Item -Recurse "$base\obj" -ErrorAction SilentlyContinue

# Let's actually build the damn thing.
& docfx "$base\docfx.json"

# DocFX AS USUAL is ignoring the templates specified in the JSON file.
# This software is complete awful, but let's just hack up the CSS is makes
# to add a glorious dark theme.
Add-Content "$base\_site\styles\main.css" -Value (Get-Content "$base\templates\darkfx\styles\main.css")

# Becuase JavaScript is all the rage with the kids these days, DocFX uses
# AJAX to load content, despite the fact that it's generationg what's
# essentially a static site. Since that means we can't access it from the
# filesystem, start a HTTP server to serve what SHOULD be static HTML, but
# is actually AJAX soup.
# The browser is opened beforehand though, since the server process is blocking.
Start-Process "http://127.0.0.1:8000/"
& python -m http.server 8000 -b 127.0.0.1 -d "$base\_site"
