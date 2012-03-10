"Updating SpeechFamiliar"
sleep -s 2
Set-PSDebug -Trace 1
# copy "E:\Programs\speechfamiliar\bin\speechfamiliar.exe" "E:\Programs\Speechfamiliar\backup\bin\"
copy "E:\Programs\speechfamiliar\bin\*.dll" "E:\Programs\SpeechFamiliar\backup\bin\"
copy "E:\Programs\SpeechFamiliar\plugins\*.dll" "E:\Programs\SpeechFamiliar\backup\plugins\"

# copy "E:\Dev\SpeechFamiliar\SpeechFamiliar\test\bin\speechfamiliar.exe" "E:\Programs\speechfamiliar\bin\speechfamiliar.exe"
copy "E:\Dev\SilentOrb_Utility\*.dll" "E:\Programs\speechfamiliar\bin"
copy "E:\Dev\SpeechFamiliar\SpeechFamiliar\test\plugins\*.dll" "E:\Programs\speechfamiliar\plugins"
copy "E:\Dev\SpeechFamiliar\SpeechFamiliar\test\bin\*.dll" "E:\Programs\speechfamiliar\bin"
copy "E:\Programs\MetaHub\*.dll" "E:\Programs\speechfamiliar\MetaHub"
copy "E:\Programs\MetaHub\MetaHub.exe" "E:\Programs\speechfamiliar\MetaHub"
copy "E:\Programs\MetaHub\plugins\*.dll" "E:\Programs\speechfamiliar\MetaHub\plugins"
copy "E:\Programs\MetaHub\global\*.*" "E:\Programs\speechfamiliar\MetaHub\global"
copy "E:\Programs\MetaHub\xsl\*.*" "E:\Programs\speechfamiliar\MetaHub\xsl"

&"E:\Programs\SpeechFamiliar\bin\SpeechFamiliar.exe"
sleep -s 10