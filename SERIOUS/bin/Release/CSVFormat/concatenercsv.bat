@ECHO OFF
SET "text=HeadX;HeadY;HeadZ;HipCenterX;HipCenterY;HipCenterZ;SpineX;SpineY;SpineZ;ShoulderCenterX;ShoulderCenterY;ShoulderCenterZ;ShoulderLeftX;ShoulderLeftY;ShoulderLeftZ;ElbowLeftX;ElbowLeftY;ElbowLeftZ;WristLeftX;WristLeftY;WristLeftZ;HandLeftX;HandLeftY;HandLeftZ;ShoulderRightX;ShoulderRightY;ShoulderRightZ;ElbowRightX;ElbowRightY;ElbowRightZ;WristRightX;WristRightY;WristRightZ;HandRightX;HandRightY;HandRightZ;HipLeftX;HipLeftY;HipLeftZ;KneeLeftX;KneeLeftY;KneeLeftZ;AnkleLeftX;AnkleLeftY;AnkleLeftZ;FootLeftX;FootLeftY;FootLeftZ;HipRightX;HipRightY;HipRightZ;KneeRightX;KneeRightY;KneeRightZ;AnkleRightX;AnkleRightY;AnkleRightZ;FootRightX;FootRightY;FootRightZ;IDPOS"
FART -c -i -r *.csv %text% " "
copy *.csv dataset.csv
type dataset.csv > temp.txt
echo %text% > dataset.csv
type temp.txt >> dataset.csv
del temp.txt
