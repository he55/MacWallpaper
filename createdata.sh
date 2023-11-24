INPUT="/Library/Application Support/com.apple.idleassetsd/Customer"
OUTPUT="data"

mkdir $OUTPUT
cd $OUTPUT

cp $INPUT/entries.json entries.json
plistutil -f json -i $INPUT/TVIdleScreenStrings.bundle/en.lproj/Localizable.nocache.strings -o en.json
plistutil -f json -i $INPUT/TVIdleScreenStrings.bundle/zh_CN.lproj/Localizable.nocache.strings -o zh_CN.json
