@echo off
setlocal ENABLEDELAYEDEXPANSION

REM ================================
REM COMMON
REM ================================
set REGION=ap-southeast-1
set S3_BUCKET=my-bucket
set S3_KEY=deploy/app.zip
set LOCAL_FILE=C:\temp\app.zip
set INSTANCE_ID=i-0123456789abcdef0

REM ================================
REM STEP 1: PC -> S3 (Key A)
REM ================================
echo Uploading file to S3...

set AWS_ACCESS_KEY_ID=AKIA_S3_XXXXXXXX
set AWS_SECRET_ACCESS_KEY=S3_SECRET_XXXXXXXX
set AWS_DEFAULT_REGION=%REGION%
set AWS_SESSION_TOKEN=

aws s3 cp "%LOCAL_FILE%" "s3://%S3_BUCKET%/%S3_KEY%"
IF ERRORLEVEL 1 (
    echo ❌ Upload failed
    exit /b 1
)

REM ================================
REM STEP 2: SSM -> EC2 (Key B)
REM ================================
echo Sending SSM command to EC2...

set AWS_ACCESS_KEY_ID=AKIA_EC2_XXXXXXXX
set AWS_SECRET_ACCESS_KEY=EC2_SECRET_XXXXXXXX
set AWS_DEFAULT_REGION=%REGION%
set AWS_SESSION_TOKEN=

aws ssm send-command ^
  --instance-ids "%INSTANCE_ID%" ^
  --document-name "AWS-RunPowerShellScript" ^
  --comment "Copy from S3 to C:\\temp" ^
  --parameters commands="if (!(Test-Path C:\temp)) { New-Item -ItemType Directory C:\temp }; aws s3 cp s3://%S3_BUCKET%/%S3_KEY% C:\temp\app.zip"

IF ERRORLEVEL 1 (
    echo ❌ SSM command failed
    exit /b 1
)

echo ✅ Done
pause 
